using OpenTK.Mathematics;

namespace SE2.Components
{
    public class RectComponent: Component
    {
        public bool displayed;
        public string shaderName;
        public Utils.Color color;
        public Utils.Vec3 size;

        public RectComponent(Utils.Vec3 size, Utils.Color color, string shaderName = "shape", bool displayed = true) : base()
        {
            this.size = size;
            this.color = color;
            this.shaderName = shaderName;
            this.displayed = displayed;
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateScale(size.x / 2, size.y / 2, size.z / 2)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, color, model);
                }
            }
        }
    }
}
