using OpenTK.Mathematics;

namespace SE2.Graphics
{
    public class Camera2D
    {
        internal Window window;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        public Entities.Entity follow;
        public float zoom = 1;

        public Camera2D(Vector3 position)
        {
            Position = position;
            follow = null;
        }

        public void Update(Vector2i WindowSize)
        {
            if(follow != null && follow.GetComponent<Components.TransformComponent>() is Components.TransformComponent tc)
            {
                Position.X = tc.position.x - WindowSize.X / 2;
                Position.Y = tc.position.y - WindowSize.Y / 2;
            }
        }

        public Vector3 Position;

        public Vector3 Up => _up;
        public Vector3 Right => _right;


        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, Position - Vector3.UnitZ, _up);
        public Matrix4 GetProjectionMatrix() => Matrix4.CreateOrthographicOffCenter(0, window.ClientSize.X * 1/zoom, 0, window.ClientSize.Y * 1/zoom, -1000f, 1000f);
    }
}
