using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class Label: Widget
    {
        public string text;
        public string font;
        public Utils.Color color;
        public string shaderName;

        public Label(Utils.Vec3 position, Utils.Vec3 scale, string text, string font, Utils.Color color, int rotation = 0, string shaderName = "text", bool displayed = true) : base(position, scale, rotation, displayed, true)
        {
            this.text = text;
            this.font = font;
            this.color = color;
            this.displayed = displayed;
            this.shaderName = shaderName;
        }

        public Label(Utils.Vec3 position, Utils.Vec3 scale, string text, string font, int rotation = 0, string shaderName = "text", bool displayed = true) : this(position, scale, text, font, Utils.Color.BLACK, rotation, shaderName, displayed) { }

        public override void Render()
        {
            base.Render();

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null || GetWindow().fontManager.GetFont(font) == null)
                return;

            Matrix4 model = Matrix4.Identity
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.TextRenderer.Render(GetWindow(), text, shaderName, font, scale, position, color, 1f, model);
        }
    }
}
