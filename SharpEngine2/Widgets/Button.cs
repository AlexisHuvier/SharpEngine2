using System;
using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class Button: Widget
    {
        private enum ButtonState
        {
            IDLE,
            CLICK,
            HOVERED
        }

        public string text;
        public string font;
        public Utils.Color bgColor;
        public Utils.Color fontColor;
        public string textShaderName;
        public string buttonShaderName;
        public Action<Button> command;

        private ButtonState state;

        public Button(Utils.Vec3 position, Utils.Vec3 size, string text, string font, Action<Button> command, Utils.Color fontColor, Utils.Color bgColor, string textShader = "text", string buttonShader="shape", int rotation = 0, bool displayed = true, bool active = true) : base(position, size, rotation, displayed, active)
        {
            this.text = text;
            this.font = font;
            this.bgColor = bgColor;
            this.fontColor = fontColor;
            this.textShaderName = textShader;
            this.buttonShaderName = buttonShader;
            this.command = command;

            state = ButtonState.IDLE;
        }

        public Button(Utils.Vec3 position, Utils.Vec3 size, string text, string font, Action<Button> command, Utils.Color fontColor, string textShader = "text", string buttonShader = "shape", int rotation = 0, bool displayed = true, bool active = true) : this(position, size, text, font, command, fontColor, Utils.Color.GRAY, textShader, buttonShader, rotation, displayed, active) { }
        public Button(Utils.Vec3 position, Utils.Vec3 size, string text, string font, Action<Button> command, string textShader = "text", string buttonShader = "shape", int rotation = 0, bool displayed = true, bool active = true) : this(position, size, text, font, command, Utils.Color.BLACK, Utils.Color.GRAY, textShader, buttonShader, rotation, displayed, active) { }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (!active)
                return;

            if (GetWindow().inputManager.MouseInRectangle(new Utils.Vec2(position.x, position.y) * GetWindow().camera.zoom, new Utils.Vec2(scale.x, scale.y) * GetWindow().camera.zoom))
            {
                if (GetWindow().inputManager.IsMouseButtonPressed(Utils.Inputs.MouseButton.LEFT) && command != null)
                    command(this);

                if (GetWindow().inputManager.IsMouseButtonDown(Utils.Inputs.MouseButton.LEFT))
                    state = ButtonState.CLICK;
                else
                    state = ButtonState.HOVERED;
            }
            else
                state = ButtonState.IDLE;
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(textShaderName) == null || GetWindow().shaderManager.GetShader(buttonShaderName) == null || GetWindow().fontManager.GetFont(font) == null)
                return;

            Matrix4 fontModel = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.TextRenderer.Render(GetWindow(), text, textShaderName, font, new Utils.Vec3(1), position, fontColor, 1f, fontModel);

            if (state == ButtonState.CLICK || !active)
            {
                Matrix4 hoverBGModel = Matrix4.Identity
                            * Matrix4.CreateScale(scale.x  / 2, scale.y / 2, scale.z / 2)
                            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                            * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z));

                Graphics.Renderers.RectRenderer.Render(GetWindow(), buttonShaderName, new Utils.Color(0, 0, 0, 128), 1f, hoverBGModel);
            }

            Matrix4 blackBGModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 bgModel = Matrix4.Identity
                        * Matrix4.CreateScale((scale.x - 6) / 2, (scale.y - 6) / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.RectRenderer.Render(GetWindow(), buttonShaderName, bgColor, 1f, bgModel);
            Graphics.Renderers.RectRenderer.Render(GetWindow(), buttonShaderName, Utils.Color.BLACK, 1f, blackBGModel);

            if (state == ButtonState.HOVERED && active)
            {
                Matrix4 hoverBGModel = Matrix4.Identity
                            * Matrix4.CreateScale((scale.x + 6) / 2, (scale.y + 6) / 2, scale.z / 2)
                            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                            * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z));

                Graphics.Renderers.RectRenderer.Render(GetWindow(), buttonShaderName, Utils.Color.WHITE, 1f, hoverBGModel);
            }
        }
    }
}
