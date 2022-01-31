using OpenTK.Mathematics;

namespace SE2.Components
{
    public class PolygonComponent: Component
    {
        public float[] vertices;
        public bool displayed;
        public string shaderName;
        public Utils.Color color;

        public PolygonComponent(float[] vertices, Utils.Color color, string shaderName = "shape", bool displayed = true) : base()
        {
            this.vertices = vertices;
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
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.PolygonRenderer.Render(GetWindow(), shaderName, vertices, color, model);
                }
            }
        }
    }
}
