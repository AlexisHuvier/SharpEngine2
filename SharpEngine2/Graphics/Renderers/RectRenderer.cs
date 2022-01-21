using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SE2.Graphics.Renderers
{
    internal class RectRenderer
    {
        private static float[] _vertices =
        {
            // Position   Color
             1f,  1f, 0f, 0f, 0f, 0f, 0f, // top right
             1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom right
            -1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom left
            -1f,  1f, 0f, 0f, 0f, 0f, 0f  // top left
        };

        private static readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private static int _vertexBufferObject;
        private static int _vertexArrayObject;
        private static int _elementBufferObject;

        private static void UpdateVertices(Utils.Color color)
        {
            float[] normalizedColor = color.Normalized();
            _vertices = new float[]
            {
                // Position   Color
                 1f,  1f, 0f, normalizedColor[0], normalizedColor[1], normalizedColor[2], normalizedColor[3], // top right
                 1f, -1f, 0f, normalizedColor[0], normalizedColor[1], normalizedColor[2], normalizedColor[3], // bottom right
                -1f, -1f, 0f, normalizedColor[0], normalizedColor[1], normalizedColor[2], normalizedColor[3], // bottom left
                -1f,  1f, 0f, normalizedColor[0], normalizedColor[1], normalizedColor[2], normalizedColor[3]  // top left
            };

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        }

        public static void Load()
        {
            Unload();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
        }

        public static void Render(Window win, string shader, Utils.Color color, Matrix4 model)
        {
            UpdateVertices(color);

            GL.BindVertexArray(_vertexArrayObject);

            win.shaderManager.GetShader(shader).Use();

            win.shaderManager.GetShader(shader).SetMatrix4("model", model);
            win.shaderManager.GetShader(shader).SetMatrix4("view", win.camera.GetViewMatrix());
            win.shaderManager.GetShader(shader).SetMatrix4("projection", win.camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public static void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
