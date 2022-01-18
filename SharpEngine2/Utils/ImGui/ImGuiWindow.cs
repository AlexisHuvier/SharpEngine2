using ImGuiNET;
using System.Numerics;

namespace SE2.Utils
{
    internal class ImGuiWindow
    {
        private static bool _showConsole = false;

        private static Vector3 _clearColor;
        private static bool _vsync;
        private static string _title;

        private static void SetupVariables(Window win)
        {
            _clearColor = new Vector3(win.backgroundColor.Normalized()[0], win.backgroundColor.Normalized()[1], win.backgroundColor.Normalized()[2]);
            _vsync = win.VSync == OpenTK.Windowing.Common.VSyncMode.On ? true : false;
            _title = win.Title;
        }

        private static void ApplyVariables(Window win)
        {
            win.backgroundColor = new Color((int)(_clearColor.X * 255), (int)(_clearColor.Y * 255), (int)(_clearColor.Z * 255));
            win.VSync = _vsync ? OpenTK.Windowing.Common.VSyncMode.On : OpenTK.Windowing.Common.VSyncMode.Off;
            win.Title = _title;
        }

        internal static void Render(Window win)
        {
            ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Once);
            ImGui.SetNextWindowCollapsed(true, ImGuiCond.Once);

            SetupVariables(win);

            {
                ImGui.Begin("Debug", ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Window"))
                    {
                        ImGui.MenuItem("Logs", null, ref _showConsole);
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }

                if (ImGui.CollapsingHeader("Debug Constants"))
                {
                    ImGui.Text($"SharpEngine2 Version : {DebugInfo.SE2_VERSION}");
                    ImGui.Text($"OpenGL Version : {DebugInfo.GL_VERSION}");
                    ImGui.Text($"GLSL Version : {DebugInfo.GLSL_VERSION}");
                    ImGui.Text($"Renderer Name : {DebugInfo.RENDERER_NAME}");
                    ImGui.Text($"Renderer Vendor : {DebugInfo.RENDERER_VENDOR}");
                    ImGui.Text($"Number of Extensions : {DebugInfo.SUPPORTED_EXTENSIONS.Count}");

                    ImGui.Text($"OpenALC Version : {DebugInfo.ALC_VERSION}");
                    ImGui.Text($"OpenAL Version : {DebugInfo.AL_VERSION}");
                    ImGui.Text($"OpenAL Renderer Name : {DebugInfo.AL_RENDERER}");
                    ImGui.Text($"OpenAL Renderer Vendor : {DebugInfo.AL_VENDOR}");
                    ImGui.Separator();
                }

                if(ImGui.CollapsingHeader("Window Parameters"))
                {
                    ImGui.ColorEdit3("Background Color", ref _clearColor);
                    ImGui.Checkbox("VSync", ref _vsync);
                    ImGui.InputText("Title", ref _title, 50);
                    ImGui.Separator();
                }

                if(ImGui.CollapsingHeader("Global Game Informations"))
                {
                    ImGui.Text($"Number of Scenes : {win.scenes.Count}");
                    ImGui.Text($"Index of Current Scene : {win.currentScene}");
                    ImGui.Text($"Number of Entities in Current Scene : {win.scenes[win.currentScene].entities.Count}");
                    ImGui.Text($"Number of Widgets in Current Scene : {win.scenes[win.currentScene].widgets.Count}");
                    ImGui.Text($"Camera Position : {win.camera.Position}");
                    ImGui.Text($"Camera Following Entity : {win.camera.follow != null}");
                    ImGui.Separator();
                }

                float framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");

                ImGui.End();
            }

            if (_showConsole)
                RenderConsole(win);



            ApplyVariables(win);
        }

        internal static void RenderConsole(Window win)
        {
            ImGui.Begin("Logs", ref _showConsole);
            foreach (string s in ImGuiTraceListener.logs)
                ImGui.Text(s);
            ImGui.End();
        }

        internal static void RenderEntityInfo(Window win)
        {

        }

        internal static void RenderWidgetsInfo(Window win)
        {

        }
    }
}
