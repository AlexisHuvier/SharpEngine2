using System;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace SE2.Widgets
{
    public class TexturedButton : Widget
    {
        private enum ButtonState
        {
            IDLE,
            CLICK,
            HOVERED
        }

        public string text;
        public string font;
        public string[] textures;
        public Utils.Color fontColor;
        public string textShaderName;
        public string buttonShaderName;
        public Action<TexturedButton> command;

        private ButtonState state;

        public TexturedButton(Utils.Vec3 position, Utils.Vec3 size, string text, string font, string[] textures, Action<TexturedButton> command, Utils.Color fontColor, string textShader = "text", string buttonShader = "sprite", int rotation = 0, bool displayed = true, bool active = true) : base(position, size, rotation, displayed, active)
        {
            this.text = text;
            this.font = font;
            this.textures = textures;
            this.fontColor = fontColor;
            this.textShaderName = textShader;
            this.buttonShaderName = buttonShader;
            this.command = command;

            if (textures.Length != 3)
                Trace.WriteLine("[ERROR] TextureButton must have 3 textures : normal, hovered, clicked/inactive.");

            state = ButtonState.IDLE;
        }

        public TexturedButton(Utils.Vec3 position, Utils.Vec3 size, string text, string font, string[] textures, Action<TexturedButton> command, string textShader = "text", string buttonShader = "sprite", int rotation = 0, bool displayed = true, bool active = true) : this(position, size, text, font, textures, command, Utils.Color.BLACK, textShader, buttonShader, rotation, displayed, active) { }

        public override void Load()
        {
            base.Load();

            Graphics.Renderers.SpriteRenderer.Load(GetWindow(), buttonShaderName, textures[0]);
        }

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

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(textShaderName) == null || GetWindow().shaderManager.GetShader(buttonShaderName) == null || GetWindow().fontManager.GetFont(font) == null || 
                GetWindow().textureManager.GetTexture(textures[0]) == null || GetWindow().textureManager.GetTexture(textures[1]) == null || GetWindow().textureManager.GetTexture(textures[2]) == null)
                return;

            Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Matrix4 fontModel = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.TextRenderer.Render(GetWindow(), text, textShaderName, font, new Utils.Vec3(1), position, fontColor, 1f, fontModel);

            if (state == ButtonState.CLICK || !active)
                Graphics.Renderers.SpriteRenderer.Render(GetWindow(), buttonShaderName, textures[2], false, false, 1f, model);

            Graphics.Renderers.SpriteRenderer.Render(GetWindow(), buttonShaderName, textures[0], false, false, 1f, model);

            if (state == ButtonState.HOVERED && active)
                Graphics.Renderers.SpriteRenderer.Render(GetWindow(), buttonShaderName, textures[1], false, false, 1f, model);
        }
    }
}
