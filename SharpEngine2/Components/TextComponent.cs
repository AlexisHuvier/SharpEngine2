using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SE2.Components
{
    public class TextComponent: Component
    {
        private int _vertexBufferObject;
        private int _vertexArrayObject;

        public string text;
        public string font;
        public Utils.Color color;
        public bool displayed;
        public string shaderName;

        public TextComponent(string text, string font, Utils.Color color, string shaderName = "text", bool displayed = true): base()
        {
            this.text = text;
            this.font = font;
            this.color = color;
            this.displayed = displayed;
            this.shaderName = shaderName;
        }

        public TextComponent(string text, string font, string shaderName = "text", bool displayed = true): this(text, font, Utils.Color.BLACK, shaderName, displayed) { }

        public override void Load()
        {
            base.Load();

            _vertexArrayObject = GL.GenVertexArray();
            _vertexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 30, System.IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public override void Render()
        {
            base.Render();

            if (!displayed)
                return;

            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    GetWindow().shaderManager.GetShader(shaderName).Use();

                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("model", model);
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("view", GetWindow().camera.GetViewMatrix());
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("projection", GetWindow().camera.GetProjectionMatrix());

                    float[] fcolor = color.Normalized();
                    GetWindow().shaderManager.GetShader(shaderName).SetVector3("textColor", new Vector3(fcolor[0], fcolor[1], fcolor[2]));

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindVertexArray(_vertexArrayObject);

                    List<Graphics.Font.Character> characters = GetWindow().fontManager.GetFont(font).GetCharacters(text);

                    Utils.Vec3 size = new Utils.Vec3(0);

                    foreach (Graphics.Font.Character ch in characters)
                    {
                        size.x += ch.Size.x * tc.scale.x;
                        size.y = ch.Size.y * tc.scale.y;
                    }

                    float x = 0;

                    foreach (Graphics.Font.Character ch in characters)
                    {
                        float xp = ch.Bearing.x * tc.scale.x + x - size.x / 2;
                        float yp = - (ch.Size.y - ch.Bearing.y) * tc.scale.y - size.y / 2;
                        float w = ch.Size.x * tc.scale.x;
                        float h = ch.Size.y * tc.scale.y;

                        float[] vertices = new float[]
                        {
                            xp, yp + h, tc.position.z, 0f, 0f,
                            xp, yp, tc.position.z, 0f, 1f,
                            xp + w, yp, tc.position.z, 1f, 1f,
                            xp, yp + h, tc.position.z, 0f, 0f,
                            xp + w, yp, tc.position.z, 1f, 1f,
                            xp + w, yp + h, tc.position.z, 1f, 0f
                        };

                        GL.BindTexture(TextureTarget.Texture2D, ch.TextureID);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                        GL.BufferSubData(BufferTarget.ArrayBuffer, System.IntPtr.Zero, vertices.Length * sizeof(float), vertices);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

                        x += ch.Advance * tc.scale.x;
                    }

                    GL.BindVertexArray(0);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
            }
        }

        public override void Unload()
        {
            base.Unload();

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
