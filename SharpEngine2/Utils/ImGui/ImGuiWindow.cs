using ImGuiNET;
using System.Linq;
using System.Numerics;
using SE2.Components;

namespace SE2.Utils
{
    internal class ImGuiWindow
    {
        private static bool _showConsole = false;
        private static bool _showEntity = false;
        private static bool _showWidget = false;

        private static Vector3 _clearColor;
        private static bool _vsync;
        private static string _title;

        private static int _currentSelectedEntity = 0;

        private static int _currentSelectedWidget = 0;

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
                        ImGui.MenuItem("Entity", null, ref _showEntity);
                        ImGui.MenuItem("Widget", null, ref _showWidget);
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

            if (_showEntity)
                RenderEntityInfo(win);

            if (_showWidget)
                RenderWidgetsInfo(win);

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
            ImGui.Begin("Entity", ref _showEntity);
            ImGui.ListBox("", 
                ref _currentSelectedEntity, 
                win.scenes[win.currentScene].entities.Select(x => $"Entity n°{win.scenes[win.currentScene].entities.IndexOf(x) + 1}").ToArray(),
                win.scenes[win.currentScene].entities.Count);
            ImGui.Separator();

            foreach(Component c in win.scenes[win.currentScene].entities[_currentSelectedEntity].components)
            {
                if (c.GetType() == typeof(BasicPhysicsComponent))
                {
                    if (ImGui.CollapsingHeader("BasicPhysicsComponent"))
                    {
                        ImGui.DragInt("Max Gravity", ref ((BasicPhysicsComponent)c).maxGravity);
                        ImGui.DragInt("Grounded Gravity", ref ((BasicPhysicsComponent)c).groundedGravity);
                        ImGui.DragInt("Time Gravity", ref ((BasicPhysicsComponent)c).timeGravity);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(ControlComponent))
                {
                    if (ImGui.CollapsingHeader("ControlComponent"))
                    {
                        ImGui.DragInt("Speed", ref ((ControlComponent)c).speed);
                        ImGui.DragInt("Jump Force", ref ((ControlComponent)c).jumpForce);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(RectCollisionComponent))
                {
                    if (ImGui.CollapsingHeader("RectCollisionComponent"))
                    {
                        ImGui.DragFloat("Size X", ref ((RectCollisionComponent)c).size.x);
                        ImGui.DragFloat("Size Y", ref ((RectCollisionComponent)c).size.y);
                        ImGui.DragFloat("Size Z", ref ((RectCollisionComponent)c).size.z);
                        ImGui.Separator();
                        ImGui.DragFloat("Offset X", ref ((RectCollisionComponent)c).offset.x);
                        ImGui.DragFloat("Offset Y", ref ((RectCollisionComponent)c).offset.y);
                        ImGui.DragFloat("Offset Z", ref ((RectCollisionComponent)c).offset.z);
                        ImGui.Separator();
                        ImGui.Checkbox("Solid", ref ((RectCollisionComponent)c).solid);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(SphereCollisionComponent))
                {
                    if (ImGui.CollapsingHeader("SphereCollisionComponent"))
                    {
                        ImGui.DragFloat("Radius", ref ((SphereCollisionComponent)c).radius);
                        ImGui.Separator();
                        ImGui.DragFloat("Offset X", ref ((SphereCollisionComponent)c).offset.x);
                        ImGui.DragFloat("Offset Y", ref ((SphereCollisionComponent)c).offset.y);
                        ImGui.DragFloat("Offset Z", ref ((SphereCollisionComponent)c).offset.z);
                        ImGui.Separator();
                        ImGui.Checkbox("Solid", ref ((SphereCollisionComponent)c).solid);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(SpriteComponent))
                {
                    if (ImGui.CollapsingHeader("SpriteComponent"))
                    {
                        ImGui.InputText("Texture", ref ((SpriteComponent)c).texture, 40);
                        ImGui.InputText("Shader Name", ref ((SpriteComponent)c).shaderName, 40);
                        ImGui.Separator();
                        ImGui.Checkbox("Flip X", ref ((SpriteComponent)c).flipX);
                        ImGui.Checkbox("Flip Y", ref ((SpriteComponent)c).flipY);
                        ImGui.Separator();
                        ImGui.Checkbox("Displayed", ref ((SpriteComponent)c).displayed);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(SpriteSheetComponent))
                {
                    if (ImGui.CollapsingHeader("SpriteSheetComponent"))
                    {
                        ImGui.InputText("Texture", ref ((SpriteSheetComponent)c).texture, 40);
                        ImGui.InputText("Shader Name", ref ((SpriteSheetComponent)c).shaderName, 40);
                        ImGui.InputText("Current Animation Name", ref ((SpriteSheetComponent)c).currentAnim, 40);
                        ImGui.Separator();
                        ImGui.DragFloat("Sprite Size X", ref ((SpriteSheetComponent)c).spriteSize.x);
                        ImGui.DragFloat("Sprite Size Y", ref ((SpriteSheetComponent)c).spriteSize.y);
                        ImGui.DragInt("Time Frame MS", ref ((SpriteSheetComponent)c).timerFrameMS);
                        ImGui.Separator();
                        ImGui.Checkbox("Flip X", ref ((SpriteSheetComponent)c).flipX);
                        ImGui.Checkbox("Flip Y", ref ((SpriteSheetComponent)c).flipY);
                        ImGui.Separator();
                        ImGui.Checkbox("Displayed", ref ((SpriteSheetComponent)c).displayed);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(TextComponent))
                {
                    if (ImGui.CollapsingHeader("TextComponent"))
                    {
                        ImGui.InputText("Text", ref ((TextComponent)c).text, 400);
                        ImGui.InputText("Shader Name", ref ((TextComponent)c).shaderName, 40);
                        ImGui.InputText("Font Name", ref ((TextComponent)c).font, 40);
                        ImGui.Separator();
                        ImGui.DragInt("Color Red", ref ((TextComponent)c).color.internalR, 1f, 0, 255);
                        ImGui.DragInt("Color Green", ref ((TextComponent)c).color.internalG, 1f, 0, 255);
                        ImGui.DragInt("Color Blue", ref ((TextComponent)c).color.internalB, 1f, 0, 255);
                        ImGui.DragInt("Color Alpha", ref ((TextComponent)c).color.internalA, 1f, 0, 255);
                        ImGui.Separator();
                        ImGui.Checkbox("Displayed", ref ((TextComponent)c).displayed);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(TileMapComponent))
                {
                    if (ImGui.CollapsingHeader("TileMapComponent"))
                    {
                        ImGui.Checkbox("Displayed", ref ((TileMapComponent)c).displayed);
                        ImGui.Separator();
                    }
                }
                else if (c.GetType() == typeof(TransformComponent))
                {
                    if(ImGui.CollapsingHeader("TransformComponent"))
                    {
                        ImGui.DragFloat("Position X", ref ((TransformComponent)c).position.x);
                        ImGui.DragFloat("Position Y", ref ((TransformComponent)c).position.y);
                        ImGui.DragFloat("Position Z", ref ((TransformComponent)c).position.z);
                        ImGui.Separator();
                        ImGui.DragFloat("Scale X", ref ((TransformComponent)c).scale.x, 0.05f);
                        ImGui.DragFloat("Scale Y", ref ((TransformComponent)c).scale.y, 0.05f);
                        ImGui.DragFloat("Scale Z", ref ((TransformComponent)c).scale.z, 0.05f);
                        ImGui.Separator();
                        ImGui.DragInt("Rotation", ref ((TransformComponent)c).rotation);
                        ImGui.Separator();
                    }
                }
                else
                {
                    if (ImGui.CollapsingHeader(c.ToString()))
                        ImGui.Separator();
                }
            }

            ImGui.End();
        }

        internal static void RenderWidgetsInfo(Window win)
        {
            ImGui.Begin("Widget", ref _showWidget);
            ImGui.End();
        }
    }
}
