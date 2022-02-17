using OpenTK.Mathematics;

namespace SE2.Components
{
    public class TextComponent: Component
    {
        public string text;
        public string font;
        public Utils.Color color;
        public bool displayed;
        public float opacity;
        public string shaderName;

        public TextComponent(string text, string font, Utils.Color color, float opacity = 1f, string shaderName = "text", bool displayed = true): base()
        {
            this.text = text;
            this.font = font;
            this.color = color;
            this.displayed = displayed;
            this.shaderName = shaderName;
            this.opacity = opacity;
        }

        public TextComponent(string text, string font, float opacity = 1f, string shaderName = "text", bool displayed = true): this(text, font, Utils.Color.BLACK, opacity, shaderName, displayed) { }


        public override void Render()
        {
            base.Render();

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null || GetWindow().fontManager.GetFont(font) == null)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.TextRenderer.Render(GetWindow(), text, shaderName, font, tc.scale, tc.position, color, opacity, model);
                }
            }
        }
    }
}
