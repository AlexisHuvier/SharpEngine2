using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SE2.Graphics.Renderers
{
    class PolygonRenderer
    {
        private static float[] _vertices =
        {
            // Position   Color
             1f,  1f, 0f, 0f, 0f, 0f, 0f, // top right
             1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom right
            -1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom left
            -1f,  1f, 0f, 0f, 0f, 0f, 0f  // top left
        };

        private static uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private static int _vertexBufferObject;
        private static int _vertexArrayObject;
        private static int _elementBufferObject;

        private static void UpdateVertices(float[] vertices, Utils.Color color)
        {
            uint lenPoints = (uint)vertices.Length / 3;

            float[] normalizedColor = color.Normalized();
            List<float> temp = new List<float>();
            for(int i = 0; i < vertices.Length; i++)
            {
                if (i != 0 && i % 3 == 0)
                    temp.AddRange(normalizedColor);
                temp.Add(vertices[i]);
            }
            temp.AddRange(normalizedColor);
            _vertices = temp.ToArray();

            List<uint> temp2 = new List<uint>();
            uint i2 = 0;
            temp2.Add(0);
            temp2.Add(1);
            temp2.Add(lenPoints - 1);
            while (lenPoints - 2 - i2 >= 2)
            {
                temp2.Add(1);
                temp2.Add(lenPoints - 2 - i2);
                temp2.Add(lenPoints - 1 - i2);
                i2++;
            }
            _indices = temp2.ToArray();

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
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

        public static void Render(Window win, string shader, float[] vertices, Utils.Color color, Matrix4 model)
        {
            UpdateVertices(vertices, color);

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
