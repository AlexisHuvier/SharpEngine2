using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class Image: Widget
    {
        public string texture;
        public string shaderName;
        public bool flipX;
        public bool flipY;

        public Image(Utils.Vec3 position, Utils.Vec3 scale, string texture, int rotation = 0, string shaderName = "sprite", bool displayed = true) : base(position, scale, rotation, displayed, true)
        {
            this.texture = texture;
            this.shaderName = shaderName;
            this.displayed = displayed;
            flipX = false;
            flipY = false;
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

            Matrix4 model = Matrix4.Identity
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                * Matrix4.CreateScale(GetWindow().textureManager.GetTexture(texture).size.X / 2, GetWindow().textureManager.GetTexture(texture).size.Y / 2, 1)
                * Matrix4.CreateScale(scale.x, scale.y, 1)
                * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, texture, flipX, flipY, model);
        }
    }
}
