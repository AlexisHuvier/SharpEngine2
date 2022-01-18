using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace SE2.Managers
{
    public class InputManager
    {
        private static Window window;

        internal static void Setup(Window win)
        {
            window = win;
            Trace.WriteLineIf(Window.DEBUG, "[DEBUG] InputManager Initialized");
        }

        public static bool IsKeyDown(Utils.Inputs.Key key) => window.KeyboardState.IsKeyDown(GetKeys(key));
        public static bool IsKeyUp(Utils.Inputs.Key key) => !window.KeyboardState.IsKeyDown(GetKeys(key));
        public static bool IsKeyPressed(Utils.Inputs.Key key) => window.KeyboardState.IsKeyPressed(GetKeys(key));
        public static bool IsKeyReleased(Utils.Inputs.Key key) => window.KeyboardState.IsKeyReleased(GetKeys(key));

        public static bool IsMouseButtonDown(Utils.Inputs.MouseButton input) => window.MouseState.IsButtonDown(GetMouseButton(input));
        public static bool IsMouseButtonUp(Utils.Inputs.MouseButton input) => !window.MouseState.IsButtonDown(GetMouseButton(input));
        public static bool IsMouseButtonPressed(Utils.Inputs.MouseButton input) => !window.MouseState.WasButtonDown(GetMouseButton(input)) && window.MouseState.IsButtonDown(GetMouseButton(input));
        public static bool IsMouseButtonReleased(Utils.Inputs.MouseButton input) => window.MouseState.WasButtonDown(GetMouseButton(input)) && !window.MouseState.IsButtonDown(GetMouseButton(input));

        public static bool MouseInRectangle(Utils.Vec2 position, Utils.Vec2 size) => window.MouseState.X >= position.x && window.MouseState.X <= position.x + size.x &&
            window.MouseState.Y >= position.y && window.MouseState.Y <= position.y + size.y;
        public static float GetMouseWheelValue() => window.MouseState.ScrollDelta.Y;
        public static Utils.Vec2 GetMousePosition() => new Utils.Vec2(window.MouseState.X, window.MouseState.Y);

        internal static Keys GetKeys(Utils.Inputs.Key key)
        {
            return (Keys)(int)key;
        }

        internal static MouseButton GetMouseButton(Utils.Inputs.MouseButton mouseButton)
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
