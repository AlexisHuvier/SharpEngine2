using ImGuiNET;
using System.Linq;
using System.Numerics;
using SE2.Components;
using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace SE2.Utils
{
    internal class ImGuiWindow
    {
        private static bool _showConsole = false;
        private static bool _showEntity = false;
        private static bool _showWidget = false;
        private static bool _showJoystick = false;

        private static Vector3 _clearColor;
        private static bool _vsync;
        private static string _title;

        private static int _currentSelectedEntity = 0;
        private static string _newTag = "";
        private static int _currentSelectedTag = 0;
        private static string _newComponent = "";
        private static int _tempRotation = 0;
        private static Vector2 _tempPosition = new Vector2(0);

        private static int _currentSelectedWidget = 0;
        private static string _newWidget = "";

        private static int _currentSelectedJoystick = 0;

        private static void SetupVariables(Window win)
        {
            _clearColor = new Vector3(win.backgroundColor.Normalized()[0], win.backgroundColor.Normalized()[1], win.backgroundColor.Normalized()[2]);
            _vsync = win.VSync == OpenTK.Windowing.Common.VSyncMode.On;
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
                    ImGui.MenuItem("Logs", null, ref _showConsole);
                    ImGui.MenuItem("Entity", null, ref _showEntity);
                    ImGui.MenuItem("Widget", null, ref _showWidget);
                    ImGui.MenuItem("Joystick", null, ref _showJoystick);
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
                    ImGui.DragFloat("Camera Zoom", ref win.camera.zoom, 0.1f, -5f, 5f);
                    ImGui.Separator();
                }

                float framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");

                ImGui.End();
            }

            if (_showConsole)
                RenderConsole(win);

            if (_showJoystick)
                RenderJoystick(win);

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

        internal static void RenderJoystick(Window win)
        {
            ImGui.Begin("Joystick", ref _showJoystick);
            ImGui.ListBox("",
                ref _currentSelectedJoystick,
                win.inputManager.GetConnectedJoysticks().Select(x => $"Joytick {x} ({win.inputManager.GetJoystickName(x)})").ToArray(),
                win.inputManager.GetConnectedJoysticks().Count);
            ImGui.Separator();
            if(win.inputManager.GetConnectedJoysticks().Count > 0 && _currentSelectedJoystick < win.inputManager.GetConnectedJoysticks().Count && win.inputManager.IsJoystickConnected(_currentSelectedJoystick))
            {
                ImGui.Text($"Number of Axis : {win.inputManager.GetJoystickAxisNumber(_currentSelectedJoystick)}");
                ImGui.Text($"Number of Buttons : {win.inputManager.GetJoystickButtonsCount(_currentSelectedJoystick)}");
                ImGui.Text($"Number of Hats : {win.inputManager.GetJoystickHatsCount(_currentSelectedJoystick)}");

                if(ImGui.CollapsingHeader("Axis"))
                {
                    for (int i = 0; i < win.inputManager.GetJoystickAxisNumber(_currentSelectedJoystick); i++)
                        ImGui.Text($"Axis {i} : {win.inputManager.GetJoystickAxis(_currentSelectedJoystick, i)} (Previous : {win.inputManager.GetJoystickPreviousAxis(_currentSelectedJoystick, i)})");
                }
                if(ImGui.CollapsingHeader("Buttons"))
                {
                    for (int i = 0; i < win.inputManager.GetJoystickButtonsCount(_currentSelectedJoystick); i++)
                        ImGui.Text($"Button {i} : {win.inputManager.IsJoystickButtonDown(_currentSelectedJoystick, i)}");
                }
                if(ImGui.CollapsingHeader("Hats"))
                {
                    for (int i = 0; i < win.inputManager.GetJoystickHatsCount(_currentSelectedJoystick); i++)
                        ImGui.Text($"Hat {i} : {win.inputManager.GetJoystickHat(_currentSelectedJoystick, i)}");
                }
            }
            ImGui.End();
        }

        internal static void RenderEntityInfo(Window win)
        {
            ImGui.Begin("Entity", ref _showEntity);
            ImGui.ListBox("Entities",
                ref _currentSelectedEntity,
                win.scenes[win.currentScene].entities.Select(x => $"Entity (ID : {x.id})").ToArray(),
                win.scenes[win.currentScene].entities.Count);
            ImGui.Separator();

            if (ImGui.Button("Add Entity"))
            {
                Entities.Entity e = new Entities.Entity();
                win.scenes[win.currentScene].AddEntity(e);
            }

            ImGui.BeginDisabled(win.scenes[win.currentScene].entities.Count == 0);
            if (ImGui.Button("Remove Entity"))
                win.scenes[win.currentScene].RemoveEntity(win.scenes[win.currentScene].entities[_currentSelectedEntity]);
            ImGui.EndDisabled();

            ImGui.Separator();

            if (win.scenes[win.currentScene].entities.Count > 0 && _currentSelectedEntity < win.scenes[win.currentScene].entities.Count)
            {
                ImGui.ListBox("Tags",
                    ref _currentSelectedTag,
                    win.scenes[win.currentScene].entities[_currentSelectedEntity].tags.ToArray(),
                    win.scenes[win.currentScene].entities[_currentSelectedEntity].tags.Count);
                ImGui.Separator();
                ImGui.InputText("Tag :", ref _newTag, 40);
                if (ImGui.Button("Add Tag"))
                    win.scenes[win.currentScene].entities[_currentSelectedEntity].AddTag(_newTag);
                ImGui.BeginDisabled(win.scenes[win.currentScene].entities[_currentSelectedEntity].tags.Count == 0);
                if (ImGui.Button("Remove Tag"))
                    win.scenes[win.currentScene].entities[_currentSelectedEntity].RemoveTag(win.scenes[win.currentScene].entities[_currentSelectedEntity].tags[_currentSelectedTag]);
                ImGui.EndDisabled();
                ImGui.Separator();

                foreach (Component c in win.scenes[win.currentScene].entities[_currentSelectedEntity].components)
                {
                    if(c.GetType() == typeof(AutoMovementComponent))
                    {
                        if(ImGui.CollapsingHeader("AutoMovementComponent"))
                        {
                            ImGui.DragFloat("Movement X", ref ((AutoMovementComponent)c).movement.x);
                            ImGui.DragFloat("Movement Y", ref ((AutoMovementComponent)c).movement.y);
                            ImGui.DragFloat("Movement Z", ref ((AutoMovementComponent)c).movement.z);
                            ImGui.Separator();
                            ImGui.DragInt("Rotation", ref ((AutoMovementComponent)c).rotation);
                            ImGui.Separator();
                        }
                    }
                    else if (c.GetType() == typeof(CircleComponent))
                    {
                        if (ImGui.CollapsingHeader("CircleComponent"))
                        {
                            ImGui.DragInt("Color Red", ref ((CircleComponent)c).color.internalR, 1f, 0, 255);
                            ImGui.DragInt("Color Green", ref ((CircleComponent)c).color.internalG, 1f, 0, 255);
                            ImGui.DragInt("Color Blue", ref ((CircleComponent)c).color.internalB, 1f, 0, 255);
                            ImGui.DragInt("Color Alpha", ref ((CircleComponent)c).color.internalA, 1f, 0, 255);
                            ImGui.Separator();
                            ImGui.DragFloat("Radius", ref ((CircleComponent)c).radius);
                            ImGui.DragInt("Number Segment", ref ((CircleComponent)c).numberSegment);
                            ImGui.InputText("Shader Name", ref ((CircleComponent)c).shaderName, 40);
                            ImGui.Checkbox("Displayed", ref ((CircleComponent)c).displayed);
                            ImGui.Separator();
                        }
                    }
                    else if (c.GetType() == typeof(ControlComponent))
                    {
                        if (ImGui.CollapsingHeader("ControlComponent"))
                        {
                            ImGui.DragInt("Speed", ref ((ControlComponent)c).speed);
                            ImGui.DragInt("Jump Force", ref ((ControlComponent)c).jumpForce);
                            ImGui.InputInt("Active Joystick", ref ((ControlComponent)c).activeJoystick);
                            ImGui.Separator();
                        }
                    }
                    else if(c.GetType() == typeof(PhysicsComponent))
                    {
                        if(ImGui.CollapsingHeader("PhysicsComponent"))
                        {
                            ImGui.Text($"Position : {((PhysicsComponent)c).GetPosition()}");
                            ImGui.Text($"Rotation : {((PhysicsComponent)c).GetRotation()}");
                            ImGui.Text($"Fixed Rotation : {((PhysicsComponent)c).body.FixedRotation}");
                            ImGui.Text($"Body Type : {((PhysicsComponent)c).body.BodyType}");
                            ImGui.Text($"Mass : {((PhysicsComponent)c).body.Mass}");
                            ImGui.Text($"Is Awake : {((PhysicsComponent)c).body.Awake}");
                            ImGui.Text($"Ignore Gravity : {((PhysicsComponent)c).body.IgnoreGravity}");
                            ImGui.Text($"Angular Velocity : {((PhysicsComponent)c).body.AngularVelocity}");
                            ImGui.Text($"Angular Damping : {((PhysicsComponent)c).body.AngularDamping}");
                            ImGui.Text($"Linear Velocity : {((PhysicsComponent)c).body.LinearVelocity}");
                            ImGui.Text($"Linear Damping : {((PhysicsComponent)c).body.LinearDamping}");
                            ImGui.Separator();
                            ImGui.InputFloat2("Position", ref _tempPosition);
                            if (ImGui.Button("Change Position"))
                                ((PhysicsComponent)c).SetPosition(new Vec2(_tempPosition.X, _tempPosition.Y));
                            ImGui.InputInt("Rotation", ref _tempRotation);
                            if (ImGui.Button("Change Rotation"))
                                ((PhysicsComponent)c).SetRotation(_tempRotation);
                            ImGui.Separator();
                        }
                    }
                    else if(c.GetType() == typeof(PolygonComponent))
                    {
                        if(ImGui.CollapsingHeader("PolygonComponent"))
                        {
                            ImGui.DragInt("Color Red", ref ((PolygonComponent)c).color.internalR, 1f, 0, 255);
                            ImGui.DragInt("Color Green", ref ((PolygonComponent)c).color.internalG, 1f, 0, 255);
                            ImGui.DragInt("Color Blue", ref ((PolygonComponent)c).color.internalB, 1f, 0, 255);
                            ImGui.DragInt("Color Alpha", ref ((PolygonComponent)c).color.internalA, 1f, 0, 255);
                            ImGui.Separator();
                            ImGui.InputText("Shader Name", ref ((PolygonComponent)c).shaderName, 40);
                            ImGui.Checkbox("Displayed", ref ((PolygonComponent)c).displayed);
                            ImGui.Separator();
                        }
                    }
                    else if(c.GetType() == typeof(RectComponent))
                    {
                        if(ImGui.CollapsingHeader("RectComponent"))
                        {
                            ImGui.DragFloat("Size X", ref ((RectComponent)c).size.x);
                            ImGui.DragFloat("Size Y", ref ((RectComponent)c).size.y);
                            ImGui.DragFloat("Size Z", ref ((RectComponent)c).size.z);
                            ImGui.Separator();
                            ImGui.DragInt("Color Red", ref ((RectComponent)c).color.internalR, 1f, 0, 255);
                            ImGui.DragInt("Color Green", ref ((RectComponent)c).color.internalG, 1f, 0, 255);
                            ImGui.DragInt("Color Blue", ref ((RectComponent)c).color.internalB, 1f, 0, 255);
                            ImGui.DragInt("Color Alpha", ref ((RectComponent)c).color.internalA, 1f, 0, 255);
                            ImGui.Separator();
                            ImGui.InputText("Shader Name", ref ((RectComponent)c).shaderName, 40);
                            ImGui.Checkbox("Displayed", ref ((RectComponent)c).displayed);
                            ImGui.Separator();
                        }
                    }
                    else if(c.GetType() == typeof(SpriteAnimComponent))
                    {
                        if(ImGui.CollapsingHeader("SpriteAnimComponent"))
                        {
                            ImGui.DragInt("Time Frame MS", ref ((SpriteAnimComponent)c).timerFrameMS);
                            ImGui.DragInt("Current Image", ref ((SpriteAnimComponent)c).currentImage);
                            ImGui.InputText("Shader Name", ref ((SpriteAnimComponent)c).shaderName, 40);
                            ImGui.Separator();
                            ImGui.Checkbox("Flip X", ref ((SpriteAnimComponent)c).flipX);
                            ImGui.Checkbox("Flip Y", ref ((SpriteAnimComponent)c).flipY);
                            ImGui.Separator();
                            ImGui.Checkbox("Displayed", ref ((SpriteAnimComponent)c).displayed);
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
                        if (ImGui.CollapsingHeader("TransformComponent"))
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

                ImGui.InputText("New Component", ref _newComponent, 40);
                if (ImGui.Button("Add"))
                {
                    if(_newComponent == "PhysicsComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new PhysicsComponent(new Vec2(1)));
                    else if (_newComponent == "ControlComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new ControlComponent());
                    else if (_newComponent == "SpriteComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new SpriteComponent(""));
                    else if (_newComponent == "SpriteSheetComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new SpriteSheetComponent("", new Vec2(0), new Dictionary<string, List<int>>()));
                    else if (_newComponent == "TextComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new TextComponent("", ""));
                    else if (_newComponent == "TileMapComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new TileMapComponent(""));
                    else if (_newComponent == "TransformComponent")
                        win.scenes[win.currentScene].entities[_currentSelectedEntity].AddComponent(new TransformComponent());
                    else
                        Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] ImGui : Unknown Component {_newComponent}");
                }
            }

            ImGui.End();
        }

        internal static void RenderWidgetsInfo(Window win)
        {
            ImGui.Begin("Widget", ref _showWidget);
            ImGui.ListBox("",
                ref _currentSelectedWidget,
                win.scenes[win.currentScene].widgets.Select(x => $"{x.GetType()} (ID : {x.id})").ToArray(),
                win.scenes[win.currentScene].widgets.Count);
            ImGui.Separator();

            ImGui.InputText("New Widget", ref _newWidget, 40);
            if (ImGui.Button("Add"))
            {
                if (_newWidget == "Image")
                    win.scenes[win.currentScene].AddWidget(new Widgets.Image(new Vec3(0), new Vec3(1), ""));
                else if (_newWidget == "Label")
                    win.scenes[win.currentScene].AddWidget(new Widgets.Label(new Vec3(0), new Vec3(1), "", ""));
                else
                    Trace.WriteLineIf(Window.DEBUG, $"[DEBUG] ImGui : Unknown Widget {_newWidget}");
            }

            ImGui.BeginDisabled(win.scenes[win.currentScene].widgets.Count == 0);
            if (ImGui.Button("Remove Widget"))
                win.scenes[win.currentScene].RemoveWidget(win.scenes[win.currentScene].widgets[_currentSelectedEntity]);
            ImGui.EndDisabled();

            ImGui.Separator();

            if (win.scenes[win.currentScene].widgets.Count > 0 && _currentSelectedWidget < win.scenes[win.currentScene].widgets.Count)
            {
                Widgets.Widget w = win.scenes[win.currentScene].widgets[_currentSelectedWidget];

                ImGui.DragFloat("Position X", ref w.position.x);
                ImGui.DragFloat("Position Y", ref w.position.y);
                ImGui.DragFloat("Position Z", ref w.position.z);
                ImGui.Separator();
                ImGui.DragFloat("Scale X", ref w.scale.x, 0.05f);
                ImGui.DragFloat("Scale Y", ref w.scale.y, 0.05f);
                ImGui.DragFloat("Scale Z", ref w.scale.z, 0.05f);
                ImGui.Separator();
                ImGui.DragInt("Rotation", ref w.rotation);
                ImGui.Checkbox("Displayed", ref w.displayed);
                ImGui.Checkbox("Active", ref w.active);
                ImGui.Separator();

                if (w.GetType() == typeof(Widgets.Image))
                {
                    ImGui.InputText("Texture", ref ((Widgets.Image)w).texture, 40);
                    ImGui.InputText("Shader Name", ref ((Widgets.Image)w).shaderName, 40);
                    ImGui.Checkbox("Flip X", ref ((Widgets.Image)w).flipX);
                    ImGui.Checkbox("Flip Y", ref ((Widgets.Image)w).flipY);

                }
                else if(w.GetType() == typeof(Widgets.Label))
                {
                    ImGui.InputText("Text", ref ((Widgets.Label)w).text, 40);
                    ImGui.InputText("Shader Name", ref ((Widgets.Label)w).shaderName, 40);
                    ImGui.InputText("Font", ref ((Widgets.Label)w).font, 40);
                    ImGui.Separator();
                    ImGui.DragInt("Color Red", ref ((Widgets.Label)w).color.internalR, 1f, 0, 255);
                    ImGui.DragInt("Color Green", ref ((Widgets.Label)w).color.internalG, 1f, 0, 255);
                    ImGui.DragInt("Color Blue", ref ((Widgets.Label)w).color.internalB, 1f, 0, 255);
                    ImGui.DragInt("Color Alpha", ref ((Widgets.Label)w).color.internalA, 1f, 0, 255);
                }
                else if(w.GetType() == typeof(Widgets.Button))
                {
                    ImGui.InputText("Text", ref ((Widgets.Button)w).text, 40);
                    ImGui.InputText("Text Shader Name", ref ((Widgets.Button)w).textShaderName, 40);
                    ImGui.InputText("Font", ref ((Widgets.Button)w).font, 40);
                    ImGui.DragInt("Font Color Red", ref ((Widgets.Button)w).fontColor.internalR, 1f, 0, 255);
                    ImGui.DragInt("Font Color Green", ref ((Widgets.Button)w).fontColor.internalG, 1f, 0, 255);
                    ImGui.DragInt("Font Color Blue", ref ((Widgets.Button)w).fontColor.internalB, 1f, 0, 255);
                    ImGui.DragInt("Font Color Alpha", ref ((Widgets.Button)w).fontColor.internalA, 1f, 0, 255);
                    ImGui.Separator();
                    ImGui.InputText("Background Shader Name", ref ((Widgets.Button)w).buttonShaderName, 40);
                    ImGui.DragInt("Background Color Red", ref ((Widgets.Button)w).bgColor.internalR, 1f, 0, 255);
                    ImGui.DragInt("Background Color Green", ref ((Widgets.Button)w).bgColor.internalG, 1f, 0, 255);
                    ImGui.DragInt("Background Color Blue", ref ((Widgets.Button)w).bgColor.internalB, 1f, 0, 255);
                    ImGui.DragInt("Background Color Alpha", ref ((Widgets.Button)w).bgColor.internalA, 1f, 0, 255);
                }
                else if(w.GetType() == typeof(Widgets.Checkbox))
                {
                    ImGui.Checkbox("Is Checked", ref ((Widgets.Checkbox)w).isChecked);
                    ImGui.InputText("Shader Name", ref ((Widgets.Checkbox)w).shaderName, 40);
                }
                else if(w.GetType() == typeof(Widgets.TexturedButton))
                {
                    ImGui.InputText("Text", ref ((Widgets.TexturedButton)w).text, 40);
                    ImGui.InputText("Text Shader Name", ref ((Widgets.TexturedButton)w).textShaderName, 40);
                    ImGui.InputText("Font", ref ((Widgets.TexturedButton)w).font, 40);
                    ImGui.DragInt("Font Color Red", ref ((Widgets.TexturedButton)w).fontColor.internalR, 1f, 0, 255);
                    ImGui.DragInt("Font Color Green", ref ((Widgets.TexturedButton)w).fontColor.internalG, 1f, 0, 255);
                    ImGui.DragInt("Font Color Blue", ref ((Widgets.TexturedButton)w).fontColor.internalB, 1f, 0, 255);
                    ImGui.DragInt("Font Color Alpha", ref ((Widgets.TexturedButton)w).fontColor.internalA, 1f, 0, 255);
                    ImGui.Separator();
                    ImGui.InputText("Texture Shader Name", ref ((Widgets.TexturedButton)w).buttonShaderName, 40);
                    ImGui.InputText("Normal Texture", ref ((Widgets.TexturedButton)w).textures[0], 40);
                    ImGui.InputText("Hovered Texture", ref ((Widgets.TexturedButton)w).textures[1], 40);
                    ImGui.InputText("Clicked / Inactive Texture", ref ((Widgets.TexturedButton)w).textures[2], 40);
                }
                else if(w.GetType() == typeof(Widgets.ProgressBar))
                {
                    ImGui.DragFloat("Value", ref ((Widgets.ProgressBar)w).value, 0.1f, 0, 100);
                    ImGui.InputText("Shader Name", ref ((Widgets.ProgressBar)w).shaderName, 40);
                    ImGui.DragInt("Color Red", ref ((Widgets.ProgressBar)w).color.internalR, 1f, 0, 255);
                    ImGui.DragInt("Color Green", ref ((Widgets.ProgressBar)w).color.internalG, 1f, 0, 255);
                    ImGui.DragInt("Color Blue", ref ((Widgets.ProgressBar)w).color.internalB, 1f, 0, 255);
                    ImGui.DragInt("Color Alpha", ref ((Widgets.ProgressBar)w).color.internalA, 1f, 0, 255);

                }
                else if(w.GetType() == typeof(Widgets.TexturedProgressBar))
                {
                    ImGui.DragFloat("Value", ref ((Widgets.TexturedProgressBar)w).value, 0.1f, 0, 100);
                    ImGui.InputText("Shader Name", ref ((Widgets.TexturedProgressBar)w).shaderName, 40);
                    ImGui.InputText("Normal Texture", ref ((Widgets.TexturedProgressBar)w).textures[0], 40);
                    ImGui.InputText("Background Texture", ref ((Widgets.TexturedProgressBar)w).textures[1], 40);
                }
                else if(w.GetType() == typeof(Widgets.LineEdit))
                {
                    ImGui.InputText("Text", ref ((Widgets.LineEdit)w).text, 40);
                    ImGui.InputText("Text Shader Name", ref ((Widgets.LineEdit)w).fontShader, 40);
                    ImGui.InputText("Font", ref ((Widgets.LineEdit)w).font, 40);
                    ImGui.DragInt("Font Color Red", ref ((Widgets.LineEdit)w).fontColor.internalR, 1f, 0, 255);
                    ImGui.DragInt("Font Color Green", ref ((Widgets.LineEdit)w).fontColor.internalG, 1f, 0, 255);
                    ImGui.DragInt("Font Color Blue", ref ((Widgets.LineEdit)w).fontColor.internalB, 1f, 0, 255);
                    ImGui.DragInt("Font Color Alpha", ref ((Widgets.LineEdit)w).fontColor.internalA, 1f, 0, 255);
                    ImGui.Separator();
                    ImGui.InputText("Shader Name", ref ((Widgets.LineEdit)w).editShader, 40);
                    ImGui.Text($"Focused : {((Widgets.LineEdit)w).focused}");
                }

                ImGui.Separator();
            }

            ImGui.End();
        }
    }
}
