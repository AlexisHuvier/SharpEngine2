using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics;


namespace SE2.Components
{
    public class SpriteSheetComponent: Component
    {
        private float[] _vertices =
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
        public bool displayed;
        public string shaderName;
        public Utils.Vec2 spriteSize;
        public Dictionary<string, List<int>> animations;
        public int timerFrameMS;

        private int currentImage;
        private string currentAnim;
        private float internalTimer;

        public SpriteSheetComponent(string texture, Utils.Vec2 size, Dictionary<string, List<int>> animations, string currentAnim = "", int timerFrameMS = 250, string shaderName = "sprite", bool displayed = true) : base()
        {
            this.texture = texture;
            this.shaderName = shaderName;
            this.displayed = displayed;
            spriteSize = size;
            this.animations = animations;
            this.currentAnim = currentAnim;
            this.timerFrameMS = timerFrameMS;

            currentImage = 0;
            internalTimer = timerFrameMS;
        }

        public void SetAnim(string anim)
        {
            currentAnim = anim;
            currentImage = 0;
            internalTimer = timerFrameMS;
        }

        public string GetAnim() => currentAnim;

        private void CalculateVertices()
        {
            int frameIndex = animations[currentAnim][currentImage];
            Vector2 textureSize = GetWindow().textureManager.GetTexture(texture).size;
            float tw = spriteSize.x / textureSize.X;
            float th = spriteSize.y / textureSize.Y;
            int numPerRow = (int)(textureSize.X / spriteSize.x);
            float tx = (frameIndex % numPerRow) * tw;
            float ty = 1 - (frameIndex / numPerRow + 1) * th;

            _vertices = new float[]
            {
                // Position   Texture coordinates
                 1f,  1f, 0f, tx + tw, ty + th, // top right
                 1f, -1f, 0f, tx + tw, ty     , // bottom right
                -1f, -1f, 0f, tx     , ty     , // bottom left
                -1f,  1f, 0f, tx     , ty + th  // top left
            };
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

            int vertexLocation = entities[0].scene.window.shaderManager.GetShader(shaderName).GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = entities[0].scene.window.shaderManager.GetShader(shaderName).GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GetWindow().textureManager.GetTexture(texture).Use(TextureUnit.Texture0);

            GetWindow().shaderManager.GetShader(shaderName).SetInt("texture0", 0);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if(currentAnim.Length > 0 && animations.ContainsKey(currentAnim))
            {
                if(internalTimer <= 0)
                {
                    if (currentImage >= animations[currentAnim].Count - 1)
                        currentImage = 0;
                    else
                        currentImage++;
                    internalTimer = timerFrameMS;
                }
                internalTimer -= (float)deltaTime * 1000;
            }
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || texture.Length == 0 || currentAnim.Length == 0 || spriteSize == new Utils.Vec2(0) || !animations.ContainsKey(currentAnim))
                return;

            CalculateVertices();
            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);


            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    GL.BindVertexArray(_vertexArrayObject);

                    GetWindow().textureManager.GetTexture(texture).Use(TextureUnit.Texture0);
                    GetWindow().shaderManager.GetShader(shaderName).Use();

                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateScale(spriteSize.x / 2, spriteSize.y / 2, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, 1)
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("model", model);
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("view", GetWindow().camera.GetViewMatrix());
                    GetWindow().shaderManager.GetShader(shaderName).SetMatrix4("projection", GetWindow().camera.GetProjectionMatrix());

                    GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
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
