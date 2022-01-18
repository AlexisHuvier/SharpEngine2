using System.Collections.Generic;

namespace SE2.Components
{
    public class ControlComponent: Component
    {
        public Utils.ControlType controlType;
        public int speed;
        public int jumpForce;
        private Dictionary<Utils.ControlKey, Utils.Inputs.Key> keys;
        public bool isMoving;

        public ControlComponent(Utils.ControlType type = Utils.ControlType.FOURDIRECTION, int spd = 5, int jmpForce = 5)
        {
            controlType = type;
            speed = spd;
            jumpForce = jmpForce;
            isMoving = false;
            keys = new Dictionary<Utils.ControlKey, Utils.Inputs.Key>()
            {
                { Utils.ControlKey.UP, Utils.Inputs.Key.UP },
                { Utils.ControlKey.DOWN, Utils.Inputs.Key.DOWN },
                { Utils.ControlKey.LEFT, Utils.Inputs.Key.LEFT },
                { Utils.ControlKey.RIGHT, Utils.Inputs.Key.RIGHT }
            };
        }

        public Utils.Inputs.Key GetKey(Utils.ControlKey controlKey) => keys[controlKey];
        public void SetKey(Utils.ControlKey controlKey, Utils.Inputs.Key key) => keys[controlKey] = key;



        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            isMoving = false;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Utils.Vec3 pos = new Utils.Vec3(tc.position.x, tc.position.y, tc.position.z);

                    switch (controlType)
                    {
                        case Utils.ControlType.MOUSEFOLLOW:
                            var mp = Managers.InputManager.GetMousePosition();
                            if (pos.x < mp.x - speed / 2)
                                pos.x += speed;
                            else if (pos.x > mp.x + speed / 2)
                                pos.x -= speed;

                            if (pos.y < (GetWindow().ClientSize.Y - mp.y) - speed / 2)
                                pos.y += speed;
                            else if (pos.y > (GetWindow().ClientSize.Y - mp.y) + speed / 2)
                                pos.y -= speed;
                            break;
                        case Utils.ControlType.LEFTRIGHT:
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.LEFT]))
                                pos.x -= speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.RIGHT]))
                                pos.x += speed;
                            break;
                        case Utils.ControlType.UPDOWN:
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.UP]))
                                pos.y += speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.DOWN]))
                                pos.y -= speed;
                            break;
                        case Utils.ControlType.FOURDIRECTION:
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.LEFT]))
                                pos.x -= speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.RIGHT]))
                                pos.x += speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.UP]))
                                pos.y += speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.DOWN]))
                                pos.y -= speed;
                            break;
                        case Utils.ControlType.CLASSICJUMP:
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.LEFT]))
                                pos.x -= speed;
                            if (Managers.InputManager.IsKeyDown(keys[Utils.ControlKey.RIGHT]))
                                pos.x += speed;
                            if (Managers.InputManager.IsKeyPressed(keys[Utils.ControlKey.UP]))
                            {
                                if (e.GetComponent<BasicPhysicsComponent>() is BasicPhysicsComponent pc && pc.grounded)
                                {
                                    pc.grounded = false;
                                    pc.gravity = -jumpForce;
                                }
                            }
                            break;
                    }

                    if (e.GetComponent<RectCollisionComponent>() is RectCollisionComponent rcc)
                    {
                        if (rcc.CanGo(pos, "ControlComponent"))
                        {
                            isMoving = true;
                            tc.position = pos;
                        }
                    }
                    else if (e.GetComponent<SphereCollisionComponent>() is SphereCollisionComponent scc)
                    {
                        if (scc.CanGo(pos, "ControlComponent"))
                        {
                            isMoving = true;
                            tc.position = pos;
                        }
                    }
                    else
                    {
                        isMoving = true;
                        tc.position = pos;
                    }
                }
            }
        }
    }
}
