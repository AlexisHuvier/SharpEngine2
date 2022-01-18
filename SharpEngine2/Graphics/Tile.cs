using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace SE2.Graphics
{
    public class Tile
    {
        public int id;
        public string source;
        internal int _vertexBufferObject;
        internal int _vertexArrayObject;
        internal int _elementBufferObject;
        public string shaderName;

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

        public Tile(int id, string source, string shaderName)
        {
            this.id = id;
            this.source = source;
            this.shaderName = shaderName;
        }

        public void Load(Window win)
        {
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            win.shaderManager.GetShader(shaderName).Use();

            int vertexLocation = win.shaderManager.GetShader(shaderName).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = win.shaderManager.GetShader(shaderName).GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            win.textureManager.GetTexture(source).Use(TextureUnit.Texture0);

            win.shaderManager.GetShader(shaderName).SetInt("texture0", 0);
        }

        public void Render(Window win, Components.TransformComponent tc, Utils.Vec2 offset)
        {
            GL.BindVertexArray(_vertexArrayObject);

            win.textureManager.GetTexture(source).Use(TextureUnit.Texture0);
            win.shaderManager.GetShader(shaderName).Use();

            Matrix4 model = Matrix4.Identity
                * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                * Matrix4.CreateScale(win.textureManager.GetTexture(source).size.X / 2, win.textureManager.GetTexture(source).size.Y / 2, 1)
                * Matrix4.CreateScale(tc.scale.x, tc.scale.y, 1)
                * Matrix4.CreateTranslation(new Vector3(tc.position.x + offset.x, tc.position.y + offset.y, tc.position.z));

            win.shaderManager.GetShader(shaderName).SetMatrix4("model", model);
            win.shaderManager.GetShader(shaderName).SetMatrix4("view", win.camera.GetViewMatrix());
            win.shaderManager.GetShader(shaderName).SetMatrix4("projection", win.camera.GetProjectionMatrix());

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
