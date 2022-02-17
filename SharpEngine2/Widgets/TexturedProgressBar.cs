using OpenTK.Mathematics;
using System.Diagnostics;

namespace SE2.Widgets
{
    public class TexturedProgressBar: Widget
    {
        public float value;
        public string shaderName;
        public string[] textures;

        public TexturedProgressBar(Utils.Vec3 position, Utils.Vec3 scale, string[] textures, float value = 100, string shaderName = "sprite", int rotation = 0, bool displayed = true, bool active = true) : base(position, scale, rotation, displayed, active)
        {
            this.value = value;
            this.shaderName = shaderName;
            this.textures = textures;

            if (textures.Length != 2)
                Trace.WriteLine("[ERROR] TexturedProgressBar must have 2 textures : normal, background.");
        }

        public override void Load()
        {
            base.Load();

            Graphics.Renderers.SpriteRenderer.Load(GetWindow(), shaderName, textures[0]);
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(shaderName) == null || GetWindow().textureManager.GetTexture(textures[0]) == null || 
                GetWindow().textureManager.GetTexture(textures[1]) == null)
                return;

            Matrix4 blackBGModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 bgModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x * (value / 100) / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x - scale.x / 2 + (scale.x * (value / 100) / 2), position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, textures[0], false, false, 1f, bgModel);
            Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, textures[1], false, false, 1f, blackBGModel);
        }
    }
}
