using OpenTK.Mathematics;

namespace SE2.Components
{
    public class CircleComponent: Component
    {
        public bool displayed;
        public string shaderName;
        public Utils.Color color;
        public float radius;
        public int numberSegment;

        public CircleComponent(float radius, Utils.Color color, int numberSegment = 10, string shaderName = "shape", bool displayed = true) : base()
        {
            this.radius = radius;
            this.color = color;
            this.shaderName = shaderName;
            this.displayed = displayed;
            this.numberSegment = numberSegment;
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
                        * Matrix4.CreateScale(radius, radius, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.CircleRenderer.Render(GetWindow(), shaderName, numberSegment, color, model);
                }
            }
        }
    }
}
