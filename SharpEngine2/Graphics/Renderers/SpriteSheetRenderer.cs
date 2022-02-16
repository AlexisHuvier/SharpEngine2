using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace SE2.Graphics.Renderers
{
    class SpriteSheetRenderer
    {
        private static float[] _vertices =
        {
            // Position   Texture coordinates
             1f,  1f, 0f, 1f, 1f, // top right
             1f, -1f, 0f, 1f, 0f, // bottom right
            -1f, -1f, 0f, 0f, 0f, // bottom left
            -1f,  1f, 0f, 0f, 1f  // top left
        };

        private static readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private static int _vertexBufferObject;
        private static int _vertexArrayObject;
        private static int _elementBufferObject;
        private static bool oldFlipX;
        private static bool oldFlipY;
        private static int oldframeIndex;

        private static void UpdateVertices(Window win, string texture, int frameIndex, Utils.Vec2 spriteSize, bool flipX, bool flipY)
        {
            oldFlipX = flipX;
            oldFlipY = flipY;
            oldframeIndex = frameIndex;

            Vector2 textureSize = win.textureManager.GetTexture(texture).size;
            float tw = spriteSize.x / textureSize.X;
            float th = spriteSize.y / textureSize.Y;
            int numPerRow = (int)(textureSize.X / spriteSize.x);
            float tx = (frameIndex % numPerRow) * tw;
            float ty = 1 - (frameIndex / numPerRow + 1) * th;

            if (flipX && flipY)
            {
                _vertices = new float[]
                {
                    // Position   Texture coordinates
                     1f,  1f, 0f, tx     , ty     , // top right
                     1f, -1f, 0f, tx     , ty + th, // bottom right
                    -1f, -1f, 0f, tx + tw, ty + th, // bottom left
                    -1f,  1f, 0f, tx + tw, ty       // top left
                };
            }
            else if (flipX && !flipY)
            {
                _vertices = new float[]
                {
                    // Position   Texture coordinates
                     1f,  1f, 0f, tx     , ty + th, // top right
                     1f, -1f, 0f, tx     , ty     , // bottom right
                    -1f, -1f, 0f, tx + tw, ty     , // bottom left
                    -1f,  1f, 0f, tx + tw, ty + th, // top left
                };
            }
            else if (!flipX && flipY)
            {
                _vertices = new float[]
                {
                    // Position   Texture coordinates
                     1f,  1f, 0f, tx + tw, ty     , // top right
                     1f, -1f, 0f, tx + tw, ty + th, // bottom right
                    -1f, -1f, 0f, tx     , ty + th, // bottom left
                    -1f,  1f, 0f, tx     , ty       // top left
                };
            }
            else
            {
                _vertices = new float[]
                {
                    // Position   Texture coordinates
                     1f,  1f, 0f, tx + tw, ty + th, // top right
                     1f, -1f, 0f, tx + tw, ty     , // bottom right
                    -1f, -1f, 0f, tx     , ty     , // bottom left
                    -1f,  1f, 0f, tx     , ty + th  // top left
                };
            }

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        }

        public static void Load(Window win, string shader, string texture)
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

            win.shaderManager.GetShader(shader).Use();

            int vertexLocation = win.shaderManager.GetShader(shader).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = win.shaderManager.GetShader(shader).GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            win.textureManager.GetTexture(texture).Use(TextureUnit.Texture0);

            win.shaderManager.GetShader(shader).SetInt("texture0", 0);
        }

        public static void Render(Window win, string shader, string texture, int frameIndex, Utils.Vec2 spriteSize, bool flipX, bool flipY, Matrix4 model)
        {
            if(oldFlipX != flipX || oldFlipY != flipY || oldframeIndex != frameIndex)
                UpdateVertices(win, texture, frameIndex, spriteSize, flipX, flipY);

            GL.BindVertexArray(_vertexArrayObject);

            win.textureManager.GetTexture(texture).Use(TextureUnit.Texture0);
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
