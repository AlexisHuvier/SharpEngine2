using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class LineEdit: Widget
    {
        public string text;
        public string font;
        public string fontShader;
        public string editShader;
        public Utils.Color fontColor;

        internal bool focused;

        public LineEdit(Utils.Vec3 position, Utils.Vec3 scale, string text, string font, Utils.Color fontColor, string fontShaderName = "text", string editShaderName = "shape", int rotation = 0, bool displayed = true, bool active = true) : base(position, scale, rotation, displayed, active)
        {
            this.text = text;
            this.font = font;
            this.fontShader = fontShaderName;
            this.editShader = editShaderName;
            this.fontColor = fontColor;

            focused = false;
        }

        public LineEdit(Utils.Vec3 position, Utils.Vec3 scale, string text, string font, string fontShaderName = "text", string editShaderName = "shape", int rotation = 0, bool displayed = true, bool active = true): this(position, scale, text, font, Utils.Color.BLACK, fontShaderName, editShaderName, rotation, displayed, active) { }

        public override void OnTextInput(char key)
        {
            base.OnTextInput(key);

            if(active && focused)
                text += key;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if(!active)
                return;

            if(GetWindow().inputManager.IsMouseButtonPressed(Utils.Inputs.MouseButton.LEFT))
                focused = GetWindow().inputManager.MouseInRectangle(new Utils.Vec2(position.x, position.y) * GetWindow().camera.zoom, new Utils.Vec2(scale.x, scale.y) * GetWindow().camera.zoom);

            if(focused && GetWindow().inputManager.IsKeyPressed(Utils.Inputs.Key.BACKSPACE))
            {
                if (text.Length >= 1)
                    text = text[..^1];
            }
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(fontShader) == null || GetWindow().shaderManager.GetShader(editShader) == null ||
                GetWindow().fontManager.GetFont(font) == null)
                return;

            Matrix4 fontModel = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.TextRenderer.Render(GetWindow(), text, fontShader, font, new Utils.Vec3(1), position, fontColor, fontModel);

            Matrix4 blackBGModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 bgModel = Matrix4.Identity
                        * Matrix4.CreateScale((scale.x - 6) / 2, (scale.y - 6) / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 whiteBgModel = Matrix4.Identity
                        * Matrix4.CreateScale((scale.x + 6) / 2, (scale.y + 6) / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.RectRenderer.Render(GetWindow(), editShader, Utils.Color.WHITE, bgModel);
            Graphics.Renderers.RectRenderer.Render(GetWindow(), editShader, Utils.Color.BLACK, blackBGModel);
            if(focused)
                Graphics.Renderers.RectRenderer.Render(GetWindow(), editShader, Utils.Color.WHITE, whiteBgModel);
        }
    }
}
