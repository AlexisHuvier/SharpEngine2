using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;

namespace SE2.Components
{
    public class Physics2DComponent: Component
    {
        public Body body;

        private Utils.Vec2 size;
        private float density;
        private BodyType bodyType;

        public Physics2DComponent(Utils.Vec2 size, float density, BodyType bodyType = BodyType.Dynamic)
        {
            this.size = size;
            this.density = density;
            this.bodyType = bodyType;
        }

        public override void Load()
        {
            base.Load();

            body = GetWindow().GetCurrentScene().world.CreateRectangle(size.x, size.y, density, bodyType: bodyType);
            body.SetRestitution(0.5f);
            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    body.Rotation = (float)(tc.rotation * System.Math.PI / 180f);
                    body.Position = new Vector2(tc.position.x, tc.position.y);
                }
            }
        }

        public Utils.Vec2 GetPosition() => new Utils.Vec2(body.Position.X, body.Position.Y);
        public void SetPosition(Utils.Vec2 position) => body.Position = new Vector2(position.x, position.y);

        public int GetRotation() => (int)(body.Rotation * 180 / System.Math.PI);
        public void SetRotation(int rotation) => body.Rotation = (float)(rotation * System.Math.PI / 180f);

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    tc.position.x = body.Position.X;
                    tc.position.y = body.Position.Y;
                    tc.rotation = (int)(body.Rotation * 180 / System.Math.PI);
                }
            }
        }
    }
}
