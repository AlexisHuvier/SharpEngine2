using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace SE2.Graphics.Renderers
{
    class CircleRenderer
    {
        private readonly static List<float> _vertices = new List<float>()
        {
            // Position   Color
             1f,  1f, 0f, 0f, 0f, 0f, 0f, // top right
             1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom right
            -1f, -1f, 0f, 0f, 0f, 0f, 0f, // bottom left
            -1f,  1f, 0f, 0f, 0f, 0f, 0f  // top left
        };

        private static int _vertexBufferObject;
        private static int _vertexArrayObject;
        private static int oldNbSegment;
        private static Utils.Color oldColor;

        private static void UpdateVertices(int nbSegment, Utils.Color color)
        {
            oldColor = color;
            oldNbSegment = nbSegment;
            float[] normalizedColor = color.Normalized();
            _vertices.Clear();
            for(int i= 0; i < nbSegment; i++)
            {
                float theta = 2.0f * System.MathF.PI * (float)i / (float)nbSegment;
                float x = System.MathF.Cos(theta);
                float y = System.MathF.Sin(theta);
                _vertices.AddRange(new float[] { x, y, 0f, normalizedColor[0], normalizedColor[1], normalizedColor[2], normalizedColor[3] });
            }

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * sizeof(float), _vertices.ToArray(), BufferUsageHint.StaticDraw);
        }

        public static void Load()
        {
            Unload();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * sizeof(float), _vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
        }

        public static void Render(Window win, string shader, int numberSegment, Utils.Color color, float opacity, Matrix4 model)
        {
            if(oldNbSegment != numberSegment || oldColor != color)
                UpdateVertices(numberSegment, color);

            GL.BindVertexArray(_vertexArrayObject);

            win.shaderManager.GetShader(shader).Use();

            win.shaderManager.GetShader(shader).SetMatrix4("model", model);
            win.shaderManager.GetShader(shader).SetMatrix4("view", win.camera.GetViewMatrix());
            win.shaderManager.GetShader(shader).SetMatrix4("projection", win.camera.GetProjectionMatrix());
            win.shaderManager.GetShader(shader).SetFloat("alpha", opacity);

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
        }

        public static void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
