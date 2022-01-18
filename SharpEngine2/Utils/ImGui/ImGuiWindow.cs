using ImGuiNET;
using System.Collections.Generic;

namespace SE2.Utils
{
    internal class ImGuiWindow
    {
        private static bool _showConsole = false;

        internal static void Render()
        {
            {
                ImGui.Begin("Debug", ImGuiWindowFlags.MenuBar);
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Show"))
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

                float framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");

                ImGui.End();
            }

            if(_showConsole)
            {
                ImGui.Begin("Logs", ref _showConsole);
                foreach (string s in ImGuiTraceListener.logs)
                    ImGui.Text(s);
                ImGui.End();
            }
        }
    }
}
