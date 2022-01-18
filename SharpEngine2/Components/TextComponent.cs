using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SE2.Components
{
    public class TextComponent: Component
    {
        public string text;
        public string font;
        public Utils.Color color;
        public bool displayed;
        public string shaderName;

        public TextComponent(string text, string font, Utils.Color color, string shaderName = "text", bool displayed = true): base()
        {
            this.text = text;
            this.font = font;
            this.color = color;
            this.displayed = displayed;
            this.shaderName = shaderName;
        }

        public TextComponent(string text, string font, string shaderName = "text", bool displayed = true): this(text, font, Utils.Color.BLACK, shaderName, displayed) { }


        public override void Render()
        {
            base.Render();

            if (!displayed)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("model", model);
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("view", GetWindow().camera.GetViewMatrix());
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("projection", GetWindow().camera.GetProjectionMatrix());

                    Graphics.Renderers.TextRenderer.Render(GetWindow(), text, shaderName, font, tc.scale, tc.position, color, model);
                }
            }
        }
    }
}
