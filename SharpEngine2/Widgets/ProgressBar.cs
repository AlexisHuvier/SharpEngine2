using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class ProgressBar : Widget
    {
        public float value;
        public string shaderName;
        public Utils.Color color;

        public ProgressBar(Utils.Vec3 position, Utils.Vec3 scale, Utils.Color color, float value = 100, string shaderName = "shape", int rotation = 0, bool displayed = true, bool active = true) : base(position, scale, rotation, displayed, active)
        {
            this.value = value;
            this.shaderName = shaderName;
            this.color = color;
        }

        public ProgressBar(Utils.Vec3 position, Utils.Vec3 scale, float value = 100, string shaderName = "shape", int rotation = 0, bool displayed = true, bool active = true) : this(position, scale, Utils.Color.GREEN, value, shaderName, rotation, displayed, active) { }

        public override void Render()
        {
            base.Render();

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(shaderName) == null)
                return;

            Matrix4 blackBGModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 bgModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x * (value / 100) / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x - scale.x / 2 + (scale.x * (value / 100) / 2), position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, color, bgModel);
            Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, Utils.Color.BLACK, blackBGModel);
        }
    }
}
