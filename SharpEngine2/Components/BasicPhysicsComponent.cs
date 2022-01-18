namespace SE2.Components
{
    public class BasicPhysicsComponent: Component
    {
        public int maxGravity;
        public int groundedGravity;
        public bool grounded;
        public int timeGravity;
        private int time;
        public int gravity;

        public BasicPhysicsComponent(int gravity = 5, int groundedGravity = 2, int timeGravity = 100) : base()
        {
            maxGravity = gravity;
            this.groundedGravity = groundedGravity;
            this.timeGravity = timeGravity;
            grounded = false;
            this.gravity = maxGravity;
            time = timeGravity;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Utils.Vec3 pos = new Utils.Vec3(tc.position.x, tc.position.y, tc.position.z);
                    pos.y -= gravity;

                    if(e.HasComponent<SphereCollisionComponent>() || e.HasComponent<RectCollisionComponent>())
                    {
                        bool canGo = false;
                        if (e.GetComponent<RectCollisionComponent>() is RectCollisionComponent rcc)
                        {
                            if (rcc.CanGo(pos, "GRAVITY"))
                            {
                                grounded = false;
                                tc.position = pos;
                                canGo = true;
                            }
                        }
                        else if (e.GetComponent<SphereCollisionComponent>() is SphereCollisionComponent scc)
                        {
                            if (scc.CanGo(pos, "GRAVITY"))
                            {
                                grounded = false;
                                tc.position = pos;
                                canGo = true;
                            }
                        }

                        if (!canGo)
                        {
                            grounded = true;
                            gravity = groundedGravity;
                        }

                        if (time <= 0 && gravity < maxGravity && !grounded)
                        {
                            gravity += 1;
                            time = timeGravity;
                        }
                        time -= (int)(deltaTime * 1000);
                    }
                    else
                        tc.position = pos;
                }
            }
        }

        public override string ToString()
        {
            return $"BasicPhysicsComponent(timeGravity={timeGravity}, maxGravity={maxGravity})";
        }
    }
}
