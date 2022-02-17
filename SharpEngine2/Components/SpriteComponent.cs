using OpenTK.Mathematics;

namespace SE2.Components
{
    public class SpriteComponent: Component
    {
        public string texture;
        public bool displayed;
        public string shaderName;
        public float opacity;

        public bool flipX;
        public bool flipY;

        public SpriteComponent(string texture, float opacity = 1f, string shaderName = "sprite", bool displayed = true): base()
        {
            this.texture = texture;
            this.shaderName = shaderName;
            this.displayed = displayed;
            this.opacity = opacity;
        }

        public override void Load()
        {
            base.Load();

            Graphics.Renderers.SpriteRenderer.Load(GetWindow(), shaderName, texture);
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null || GetWindow().textureManager.GetTexture(texture) == null)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateScale(GetWindow().textureManager.GetTexture(texture).size.X / 2, GetWindow().textureManager.GetTexture(texture).size.Y / 2, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, texture, flipX, flipY, opacity, model);
                }
            }
        }
    }
}
