using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Audio.OpenAL;
using SE2.Utils;
using SE2.Graphics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace SE2
{
    public class Window: GameWindow
    {
        private DebugProc _debugProc;

        internal static bool DEBUG;

        internal Camera2D camera;
        internal List<Scene> scenes;
        internal int currentScene = -1;
        internal ImGuiController imguiController;
        public Color backgroundColor;
        public Managers.FontManager fontManager;
        public Managers.ShaderManager shaderManager;
        public Managers.SoundManager soundManager;
        public Managers.TextureManager textureManager;
        public Managers.InputManager inputManager;

        public Window(int width, int height, string title, Color bgColor, bool debug = false, bool vsync = true): base(GameWindowSettings.Default, new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = title,
                Flags = ContextFlags.ForwardCompatible
            })
        {
            if (!vsync)
                VSync = VSyncMode.Off;
            else
                VSync = VSyncMode.On;

            DEBUG = debug;

            if (DEBUG)
                Trace.Listeners.Add(new ConsoleTraceListener(false));
            Trace.Listeners.Add(new TextWriterTraceListener("se2logs.log"));
            Trace.Listeners.Add(new ImGuiTraceListener());
            Trace.AutoFlush = true;

            DebugInfo.GL_VERSION = new Version(GL.GetString(StringName.Version).Split(" ")[0]);
            DebugInfo.RENDERER_NAME = GL.GetString(StringName.Renderer);
            DebugInfo.RENDERER_VENDOR = GL.GetString(StringName.Vendor);
            DebugInfo.GLSL_VERSION = string.IsNullOrEmpty(GL.GetString(StringName.ShadingLanguageVersion)) ? new Version() : new Version(GL.GetString(StringName.ShadingLanguageVersion).Split(" ")[0]);
            for (var i = 0; i < GL.GetInteger(GetPName.NumExtensions); i++)
                DebugInfo.SUPPORTED_EXTENSIONS.Add(GL.GetString(StringNameIndexed.Extensions, i));

            Trace.WriteLineIf(DEBUG, $"[DEBUG] SharpEngine2 Version : {DebugInfo.SE2_VERSION}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] OpenGL Version : {DebugInfo.GL_VERSION}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] GLSL Version : {DebugInfo.GLSL_VERSION}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] Renderer Name : {DebugInfo.RENDERER_NAME}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] Renderer Vendor : {DebugInfo.RENDERER_VENDOR}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] Number of Extensions : {DebugInfo.SUPPORTED_EXTENSIONS.Count}");

            string deviceName = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);
            foreach (string d in ALC.GetStringList(GetEnumerationStringList.DeviceSpecifier))
            {
                if (d.Contains("OpenAL Soft"))
                    deviceName = d;
            }

            var device = ALC.OpenDevice(deviceName);
            var context = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(context);

            ALC.GetInteger(device, AlcGetInteger.MajorVersion, 1, out int alcMajorVersion);
            ALC.GetInteger(device, AlcGetInteger.MinorVersion, 1, out int alcMinorVersion);
            DebugInfo.ALC_VERSION = new Version(alcMajorVersion, alcMinorVersion);
            DebugInfo.AL_RENDERER = AL.Get(ALGetString.Renderer);
            DebugInfo.AL_VENDOR = AL.Get(ALGetString.Vendor);
            DebugInfo.AL_VERSION = new Version(AL.Get(ALGetString.Version));

            Trace.WriteLineIf(DEBUG, $"[DEBUG] OpenALC Version : {DebugInfo.ALC_VERSION}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] OpenAL Version : {DebugInfo.AL_VERSION}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] OpenAL Renderer Name : {DebugInfo.AL_RENDERER}");
            Trace.WriteLineIf(DEBUG, $"[DEBUG] OpenAL Renderer Vendor : {DebugInfo.AL_VENDOR}");

            camera = new Camera2D(Vector3.Zero)
            {
                window = this
            };
            scenes = new List<Scene>();
            backgroundColor = bgColor;

            Trace.WriteLineIf(DEBUG, $"[DEBUG] Window Initialized");

            fontManager = new Managers.FontManager();
            shaderManager = new Managers.ShaderManager();
            soundManager = new Managers.SoundManager();
            textureManager = new Managers.TextureManager();
            inputManager = new Managers.InputManager(this);

            shaderManager.AddShader("sprite", new Shader(Shaders.GetBasicSpriteShaderVertex(), Shaders.GetBasicSpriteShaderFragment()));
            shaderManager.AddShader("text", new Shader(Shaders.GetBasicFontShaderVertex(), Shaders.GetBasicFontShaderFragment()));
            shaderManager.AddShader("shape", new Shader(Shaders.GetShapeShaderVertex(), Shaders.GetShapeShaderFragment()));
        }

        public Window(int width, int height, string title, bool debug = false, bool vsync = true): this(width, height, title, Color.DARK_GRAY, debug, vsync) { }

        public virtual Camera2D GetCamera() => camera;
        public virtual void SetCurrentScene(Scene scene) => currentScene = scenes.IndexOf(scene);
        public virtual void SetCurrentScene(int scene) => currentScene = scene;

        public virtual void AddScene(Scene scene)
        {
            scene.SetWindow(this);
            scenes.Add(scene);
            SetCurrentScene(scene);
        }

        public virtual void RemoveScene(Scene scene)
        {
            scene.SetWindow(null);
            scenes.Remove(scene);
        }

        public Scene GetCurrentScene() => scenes[currentScene];

        protected override void OnLoad()
        {
            base.OnLoad();

            if (DEBUG)
            {
                if (DebugInfo.GL_VERSION < new Version(4, 3) && !DebugInfo.SUPPORTED_EXTENSIONS.Contains("GL_KHR_debug"))
                    Trace.WriteLine("[WARNING] OpenGL debug output is unavailable");
                else
                {
                    Trace.WriteLine("[DEBUG] Enabling OpenGL debug output");

                    GL.Enable(EnableCap.DebugOutput);
                    GL.Enable(EnableCap.DebugOutputSynchronous);

                    _debugProc = DebugCallback;
                    GL.DebugMessageCallback(_debugProc, IntPtr.Zero);

                    GL.DebugMessageInsert(DebugSourceExternal.DebugSourceApplication, DebugType.DebugTypeMarker, 0, DebugSeverity.DebugSeverityNotification, -1, "Debug output enabled");
                }
            }

            GL.Enable(EnableCap.DepthTest); 
            GL.Enable(EnableCap.Blend);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            imguiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            Graphics.Renderers.TextRenderer.Load();
            Graphics.Renderers.RectRenderer.Load();
            Graphics.Renderers.CircleRenderer.Load();
            Graphics.Renderers.PolygonRenderer.Load();
            Trace.WriteLineIf(DEBUG, $"[DEBUG] Window loaded");

            foreach (Scene w in scenes)
                w.Load();
            Trace.WriteLineIf(DEBUG, $"[DEBUG] Scenes loaded");
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            float[] color = backgroundColor.Normalized();
            GL.ClearColor(color[0], color[1], color[2], color[3]);

            DebugInfo.Draw();

            imguiController.Update(this, (float)args.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (currentScene != -1)
                scenes[currentScene].Render();

            if (DEBUG)
            {
                RenderImGui();
                imguiController.Render();
            }

            SwapBuffers();
        }

        public virtual void RenderImGui()
        {
            ImGuiWindow.Render(this);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            imguiController.PressChar((char)e.Unicode);
            scenes[currentScene].OnTextInput((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            imguiController.MouseScroll(e.Offset);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            DebugInfo.Update(e.Time);

            if (!IsFocused)
                return;

            if (currentScene != -1)
                scenes[currentScene].Update(e.Time);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            camera.Update(ClientSize);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            imguiController.WindowResized(ClientSize.X, ClientSize.Y);
            scenes[currentScene].OnResize(new Utils.Vec2(e.Size.X, e.Size.Y));
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (Scene w in scenes)
                w.Unload();

            Graphics.Renderers.SpriteRenderer.Unload();
            Graphics.Renderers.SpriteSheetRenderer.Unload();
            Graphics.Renderers.TextRenderer.Unload();
            Graphics.Renderers.RectRenderer.Unload();
            Graphics.Renderers.CircleRenderer.Unload();
            Graphics.Renderers.PolygonRenderer.Unload();
            shaderManager.Unload();
            soundManager.Unload();
            textureManager.Unload();

            Trace.WriteLineIf(DEBUG, $"[DEBUG] Window unloaded");

            base.OnUnload();
        }

        public override void Run()
        {
            foreach (Scene w in scenes)
                w.Init();

            Trace.WriteLineIf(DEBUG, $"[DEBUG] Scenes Initialized");

            base.Run();
        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            switch(severity)
            {
                case DebugSeverity.DontCare | DebugSeverity.DebugSeverityNotification:
                    Trace.WriteLineIf(DEBUG, $"[DEBUG] {type} | {messageString}");
                    break;
                case DebugSeverity.DebugSeverityLow:
                    Trace.WriteLine($"[INFO] {type} | {messageString}");
                    break;
                case DebugSeverity.DebugSeverityMedium:
                    Trace.WriteLine($"[WARNING] {type} | {messageString}");
                    break;
                case DebugSeverity.DebugSeverityHigh:
                    Trace.WriteLine($"[ERROR] {type} | {messageString}");
                    break;
            }

            if (type == DebugType.DebugTypeError)
                throw new Exception(messageString);
        }
    }
}
