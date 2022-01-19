using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SE2.Graphics.Renderers
{
    class TextRenderer
    {
        private static int _vertexBufferObject;
        private static int _vertexArrayObject;

        public static void Load()
        {
            Unload();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 30, System.IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public static void Render(Window win, string text, string shader, string font, Utils.Vec3 scale, Utils.Vec3 position, Utils.Color color, Matrix4 model)
        {

            GL.BindVertexArray(_vertexArrayObject);
            GL.ActiveTexture(TextureUnit.Texture0);

            win.shaderManager.GetShader(shader).Use();

            win.shaderManager.GetShader(shader).SetMatrix4("model", model);
            win.shaderManager.GetShader(shader).SetMatrix4("view", win.camera.GetViewMatrix());
            win.shaderManager.GetShader(shader).SetMatrix4("projection", win.camera.GetProjectionMatrix());

            float[] fcolor = color.Normalized();
            win.shaderManager.GetShader(shader).SetVector3("textColor", new Vector3(fcolor[0], fcolor[1], fcolor[2]));

            List<Font.Character> characters = win.fontManager.GetFont(font).GetCharacters(text);

            Utils.Vec3 size = new Utils.Vec3(0);

            foreach (Font.Character ch in characters)
            {
                size.x += ch.Size.x * scale.x;
                size.y = ch.Size.y * scale.y;
            }


            float x = 0;

            foreach (Font.Character ch in characters)
            {
                float xp = ch.Bearing.x * scale.x + x - size.x / 2;
                float yp = -(ch.Size.y - ch.Bearing.y) * scale.y - size.y / 2;
                float w = ch.Size.x * scale.x;
                float h = ch.Size.y * scale.y;

                float[] vertices = new float[]
                {
                    xp, yp + h, position.z, 0f, 0f,
                    xp, yp, position.z, 0f, 1f,
                    xp + w, yp, position.z, 1f, 1f,
                    xp, yp + h, position.z, 0f, 0f,
                    xp + w, yp, position.z, 1f, 1f,
                    xp + w, yp + h, position.z, 1f, 0f
                };

                GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferSubData(BufferTarget.ArrayBuffer, System.IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                x += ch.Advance * scale.x;
            }
        }

        public static void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
