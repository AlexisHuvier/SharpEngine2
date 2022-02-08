namespace SE2.Components
{
    public class AutoMovementComponent: Component
    {
        public Utils.Vec3 movement;
        public int rotation;

        public AutoMovementComponent(Utils.Vec3 movement, int rotation)
        {
            this.movement = movement;
            this.rotation = rotation;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Utils.Vec3 pos = new Utils.Vec3(tc.position.x, tc.position.y, tc.position.z);
                    int rot= tc.rotation;

                    pos += movement;
                    rot += rotation;

                    if (e.GetComponent<PhysicsComponent>() is PhysicsComponent pc)
                    {
                        pc.SetPosition(new Utils.Vec2(pos.x, pos.y));
                        pc.SetRotation(rot);
                    }
                    else
                    {
                        tc.position = pos;
                        tc.rotation = rot;
                    }
                }
            }
        }
    }
}
