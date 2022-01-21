using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace SE2.Managers
{
    public class InputManager
    {
        private Window window;

        public InputManager(Window win)
        {
            window = win;
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] InputManager Initialized");
        }

        public List<int> GetConnectedJoysticks() => new List<int>(window.JoystickStates.Where(x => x != null).Select(x => x.Id));
        public bool IsJoystickConnected(int joystickId) => window.JoystickStates.Where(x => x != null && x.Id == joystickId).Count() > 0;
        public JoystickState GetJoystickState(int joystickId) => window.JoystickStates.Where(x => x != null && x.Id == joystickId).Single();
        public string GetJoystickName(int joystickId) => GetJoystickState(joystickId).Name;
        public int GetJoystickAxisNumber(int joystickId) => GetJoystickState(joystickId).AxisCount;
        public int GetJoystickButtonsCount(int joystickId) => GetJoystickState(joystickId).ButtonCount;
        public int GetJoystickHatsCount(int joystickId) => GetJoystickState(joystickId).HatCount;

        public Utils.Inputs.HatState GetJoystickHat(int joystickID, int hatID) => (Utils.Inputs.HatState)(int)GetJoystickState(joystickID).GetHat(hatID);
        public float GetJoystickAxis(int joystickId, int axisID) => GetJoystickState(joystickId).GetAxis(axisID);
        public float GetJoystickPreviousAxis(int joystickId, int axisID) => GetJoystickState(joystickId).GetAxisPrevious(axisID);
        public bool IsJoystickButtonDown(int joystickId, int buttonId) => GetJoystickState(joystickId).IsButtonDown(buttonId);
        public bool IsJoystickButtonUp(int joystickId, int buttonId) => !GetJoystickState(joystickId).IsButtonDown(buttonId);
        public bool IsJoystickButtonPressed(int joystickId, int buttonId) => !GetJoystickState(joystickId).WasButtonDown(buttonId) && GetJoystickState(joystickId).IsButtonDown(buttonId);
        public bool IsJoystickButtonReleased(int joystickId, int buttonId) => GetJoystickState(joystickId).WasButtonDown(buttonId) && !GetJoystickState(joystickId).IsButtonDown(buttonId);

        public bool IsKeyDown(Utils.Inputs.Key key) => window.KeyboardState.IsKeyDown(GetKeys(key));
        public bool IsKeyUp(Utils.Inputs.Key key) => !window.KeyboardState.IsKeyDown(GetKeys(key));
        public bool IsKeyPressed(Utils.Inputs.Key key) => window.KeyboardState.IsKeyPressed(GetKeys(key));
        public bool IsKeyReleased(Utils.Inputs.Key key) => window.KeyboardState.IsKeyReleased(GetKeys(key));

        public bool IsMouseButtonDown(Utils.Inputs.MouseButton input) => window.MouseState.IsButtonDown(GetMouseButton(input));
        public bool IsMouseButtonUp(Utils.Inputs.MouseButton input) => !window.MouseState.IsButtonDown(GetMouseButton(input));
        public bool IsMouseButtonPressed(Utils.Inputs.MouseButton input) => !window.MouseState.WasButtonDown(GetMouseButton(input)) && window.MouseState.IsButtonDown(GetMouseButton(input));
        public bool IsMouseButtonReleased(Utils.Inputs.MouseButton input) => window.MouseState.WasButtonDown(GetMouseButton(input)) && !window.MouseState.IsButtonDown(GetMouseButton(input));

        public bool MouseInRectangle(Utils.Vec2 position, Utils.Vec2 size) => window.MouseState.X >= position.x && window.MouseState.X <= position.x + size.x &&
            window.MouseState.Y >= position.y && window.MouseState.Y <= position.y + size.y;
        public float GetMouseWheelValue() => window.MouseState.ScrollDelta.Y;
        public Utils.Vec2 GetMousePosition() => new Utils.Vec2(window.MouseState.X, window.ClientSize.Y - window.MouseState.Y);

        internal Keys GetKeys(Utils.Inputs.Key key)
        {
            return (Keys)(int)key;
        }

        internal MouseButton GetMouseButton(Utils.Inputs.MouseButton mouseButton)
        {
            switch(mouseButton)
            {
                case Utils.Inputs.MouseButton.LEFT:
                    return MouseButton.Left;
                case Utils.Inputs.MouseButton.MIDDLE:
                    return MouseButton.Middle;
                case Utils.Inputs.MouseButton.RIGHT:
                    return MouseButton.Right;
            }
            return MouseButton.Left;
        }
    }
}
