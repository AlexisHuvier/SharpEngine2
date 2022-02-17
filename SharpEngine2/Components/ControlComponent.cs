using System.Collections.Generic;

namespace SE2.Components
{
    public class ControlComponent: Component
    {
        public Utils.ControlType controlType;
        public int speed;
        public int jumpForce;
        private readonly Dictionary<Utils.ControlKey, Utils.Inputs.Key> keys;
        private readonly Dictionary<Utils.ControlAxis, int> axis;
        private readonly Dictionary<Utils.ControlKey, int> buttons;
        private readonly Dictionary<Utils.ControlKey, KeyValuePair<int, Utils.Inputs.HatState>> hats;
        public int activeJoystick;

        public ControlComponent(Utils.ControlType type = Utils.ControlType.FOURDIRECTION, int spd = 100, int jmpForce = 100)
        {
            controlType = type;
            speed = spd;
            jumpForce = jmpForce;
            activeJoystick = -1;

            keys = new Dictionary<Utils.ControlKey, Utils.Inputs.Key>()
            {
                { Utils.ControlKey.UP, Utils.Inputs.Key.UP },
                { Utils.ControlKey.DOWN, Utils.Inputs.Key.DOWN },
                { Utils.ControlKey.LEFT, Utils.Inputs.Key.LEFT },
                { Utils.ControlKey.RIGHT, Utils.Inputs.Key.RIGHT }
            };
            axis = new Dictionary<Utils.ControlAxis, int>()
            {
                { Utils.ControlAxis.UPDOWN, 1 },
                { Utils.ControlAxis.LEFTRIGHT, 0 }
            };
            buttons = new Dictionary<Utils.ControlKey, int>()
            {
                { Utils.ControlKey.UP, -1 },
                { Utils.ControlKey.DOWN, -1 },
                { Utils.ControlKey.LEFT, -1 },
                { Utils.ControlKey.RIGHT, -1 }
            };
            hats = new Dictionary<Utils.ControlKey, KeyValuePair<int, Utils.Inputs.HatState>>()
            {
                { Utils.ControlKey.UP, new KeyValuePair<int, Utils.Inputs.HatState>(0, Utils.Inputs.HatState.UP) },
                { Utils.ControlKey.DOWN, new KeyValuePair<int, Utils.Inputs.HatState>(0, Utils.Inputs.HatState.DOWN) },
                { Utils.ControlKey.LEFT, new KeyValuePair<int, Utils.Inputs.HatState>(0, Utils.Inputs.HatState.LEFT) },
                { Utils.ControlKey.RIGHT, new KeyValuePair<int, Utils.Inputs.HatState>(0, Utils.Inputs.HatState.RIGHT) }
            };
        }

        public Utils.Inputs.Key GetKey(Utils.ControlKey controlKey) => keys[controlKey];
        public void SetKey(Utils.ControlKey controlKey, Utils.Inputs.Key key) => keys[controlKey] = key;

        public int GetJoystickAxis(Utils.ControlAxis controlAxis) => axis[controlAxis];
        public void SetJoystickAxis(Utils.ControlAxis controlAxis, int axisID) => axis[controlAxis] = axisID;

        public int GetJoystickButton(Utils.ControlKey controlKey) => buttons[controlKey];
        public void SetJoystickButton(Utils.ControlKey controlKey, int buttonID) => buttons[controlKey] = buttonID;

        public KeyValuePair<int, Utils.Inputs.HatState> GetJoystickHat(Utils.ControlKey controlKey) => hats[controlKey];
        public void SetJoystickHat(Utils.ControlKey controlKey, int hatID, Utils.Inputs.HatState hatState) => hats[controlKey] = new KeyValuePair<int, Utils.Inputs.HatState>(hatID, hatState);

        private bool IsControlKeyDown(Utils.ControlKey key)
        {
            return GetWindow().inputManager.IsKeyDown(keys[key]) || (
                activeJoystick != -1 && GetWindow().inputManager.IsJoystickConnected(activeJoystick) && (
                    (buttons[key] != - 1 && GetWindow().inputManager.IsJoystickButtonDown(activeJoystick, buttons[key])) ||
                    (hats[key].Key != -1 && GetWindow().inputManager.GetJoystickHat(activeJoystick, hats[key].Key) == hats[key].Value)
                    )
                );
        }

        private bool IsAxisTriggered(Utils.ControlAxis axe, bool left)
        {
            return activeJoystick != -1 && GetWindow().inputManager.IsJoystickConnected(activeJoystick) && 
                ((left && GetWindow().inputManager.GetJoystickAxis(activeJoystick, axis[axe]) < -0.5) ||
                (!left && GetWindow().inputManager.GetJoystickAxis(activeJoystick, axis[axe]) > 0.5));
        }
        
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Utils.Vec3 pos = new Utils.Vec3(tc.position.x, tc.position.y, tc.position.z);

                    switch (controlType)
                    {
                        case Utils.ControlType.MOUSEFOLLOW:
                            var mp = GetWindow().inputManager.GetMousePosition();
                            if (pos.x < mp.x - speed / 2)
                                pos.x += speed * (float)deltaTime;
                            else if (pos.x > mp.x + speed / 2)
                                pos.x -= speed * (float)deltaTime;

                            if (pos.y < (GetWindow().ClientSize.Y - mp.y) - speed / 2)
                                pos.y += speed * (float)deltaTime;
                            else if (pos.y > (GetWindow().ClientSize.Y - mp.y) + speed / 2)
                                pos.y -= speed * (float)deltaTime;
                            break;
                        case Utils.ControlType.LEFTRIGHT:
                            if (IsControlKeyDown(Utils.ControlKey.LEFT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, true))
                                pos.x -= speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.RIGHT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, false))
                                pos.x += speed * (float)deltaTime;
                            break;
                        case Utils.ControlType.UPDOWN:
                            if (IsControlKeyDown(Utils.ControlKey.UP) || IsAxisTriggered(Utils.ControlAxis.UPDOWN, true))
                                pos.y += speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.DOWN) || IsAxisTriggered(Utils.ControlAxis.UPDOWN, false))
                                pos.y -= speed * (float)deltaTime;
                            break;
                        case Utils.ControlType.FOURDIRECTION:
                            if (IsControlKeyDown(Utils.ControlKey.LEFT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, true))
                                pos.x -= speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.RIGHT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, false))
                                pos.x += speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.UP) || IsAxisTriggered(Utils.ControlAxis.UPDOWN, true))
                                pos.y += speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.DOWN) || IsAxisTriggered(Utils.ControlAxis.UPDOWN, false))
                                pos.y -= speed * (float)deltaTime;
                            break;
                        case Utils.ControlType.CLASSICJUMP:
                            if (IsControlKeyDown(Utils.ControlKey.LEFT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, true))
                                pos.x -= speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.RIGHT) || IsAxisTriggered(Utils.ControlAxis.LEFTRIGHT, false))
                                pos.x += speed * (float)deltaTime;
                            if (IsControlKeyDown(Utils.ControlKey.UP) || IsAxisTriggered(Utils.ControlAxis.UPDOWN, true))
                            {
                                throw new System.NotImplementedException();
                            }
                            break;
                    }

                    if (e.GetComponent<PhysicsComponent>() is PhysicsComponent pc)
                        pc.SetPosition(new Utils.Vec2(pos.x, pos.y));
                    else
                        tc.position = pos;
                }
            }
        }
    }
}
