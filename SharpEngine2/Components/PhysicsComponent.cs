using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Common;
using System.Collections.Generic;
using System.Diagnostics;

namespace SE2.Components
{
    public class PhysicsComponent : Component
    {
        public Body body;

        private string type;
        private List<object> parameters;

        public PhysicsComponent(Utils.Vec2 size, float density = 1f, float restitution = 0.5f, float friction = 0.5f, BodyType bodyType = BodyType.Dynamic)
        {
            type = "Rectangle";
            parameters = new List<object>() { size, density, restitution, friction, bodyType };
        }

        public PhysicsComponent(float radius, float density = 1f, float restitution = 0.5f, float friction = 0.5f, BodyType bodyType = BodyType.Dynamic)
        {
            type = "Circle";
            parameters = new List<object>() { radius, density, restitution, friction, bodyType };
        }

        public PhysicsComponent(float height, float endRadius, float density = 1f, float restitution = 0.5f, float friction = 0.5f, BodyType bodyType = BodyType.Dynamic)
        {
            type = "Capsule";
            parameters = new List<object>() { new Utils.Vec2(height, endRadius), density, restitution, friction, bodyType };
        }

        public override void Load()
        {
            base.Load();

            float density = System.Convert.ToSingle(parameters[1]);
            float restitution = System.Convert.ToSingle(parameters[2]);
            float friction = System.Convert.ToSingle(parameters[3]);
            BodyType bodyType = (BodyType)parameters[4];

            if (type == "Rectangle")
            {
                Utils.Vec2 size = parameters[0] as Utils.Vec2;
                body = GetWindow().GetCurrentScene().world.CreateRectangle(size.x, size.y, density, bodyType: bodyType);
            }
            else if (type == "Circle")
            {
                float radius = System.Convert.ToSingle(parameters[0]);
                body = GetWindow().GetCurrentScene().world.CreateCircle(radius, density, bodyType: bodyType);
            }
            else if (type == "Capsule")
            {
                Utils.Vec2 heightEndRadius = parameters[0] as Utils.Vec2;
                body = GetWindow().GetCurrentScene().world.CreateCapsule(heightEndRadius.x, heightEndRadius.y, density, bodyType: bodyType);
            }
            else
                Trace.WriteLine($"[ERROR] Unknown body type : {type}");

#pragma warning disable CS0612 // 'Body.SetRestitution(float)' est obsolète
            body.SetRestitution(restitution);
#pragma warning restore CS0612 // 'Body.SetRestitution(float)' est obsolète
#pragma warning disable CS0612 // 'Body.SetFriction(float)' est obsolète
            body.SetFriction(friction);
#pragma warning restore CS0612 // 'Body.SetFriction(float)' est obsolète

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
