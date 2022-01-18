using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class Image: Widget
    {
        private readonly float[] _vertices =
        {
            // Position   Texture coordinates
             1f,  1f, 0f, 1f, 1f, // top right
             1f, -1f, 0f, 1f, 0f, // bottom right
            -1f, -1f, 0f, 0f, 0f, // bottom left
            -1f,  1f, 0f, 0f, 1f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        public string texture;
        public string shaderName;

        public Image(Utils.Vec3 position, Utils.Vec3 scale, string texture, int rotation = 0, string shaderName = "sprite", bool displayed = true) : base(position, scale, rotation, displayed, true)
        {
            this.texture = texture;
            this.shaderName = shaderName;
            this.displayed = displayed;
        }

        public override void Load()
        {
            base.Load();

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GetWindow().shaderManager.GetShader(shaderName).Use();

            int vertexLocation = GetWindow().shaderManager.GetShader(shaderName).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = GetWindow().shaderManager.GetShader(shaderName).GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GetWindow().textureManager.GetTexture(texture).Use(TextureUnit.Texture0);

            GetWindow().shaderManager.GetShader(shaderName).SetInt("texture0", 0);
        }

        public override void Render()
        {
            base.Render();

            if (!displayed)
                return;

                GL.BindVertexArray(_vertexArrayObject);

                GetWindow().textureManager.GetTexture(texture).Use(TextureUnit.Texture0);
                GetWindow().shaderManager.GetShader(shaderName).Use();

                Matrix4 model = Matrix4.Identity
                    * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                    * Matrix4.CreateScale(GetWindow().textureManager.GetTexture(texture).size.X / 2, GetWindow().textureManager.GetTexture(texture).size.Y / 2, 1)
                    * Matrix4.CreateScale(scale.x, scale.y, 1)
                    * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

                GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("model", model);
                GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("view", GetWindow().camera.GetViewMatrix());
                GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("projection", GetWindow().camera.GetProjectionMatrix());

                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void Unload()
        {
            base.Unload();

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
