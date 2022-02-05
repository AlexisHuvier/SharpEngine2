using OpenTK.Mathematics;

namespace SE2.Components
{
    public class SpriteAnimComponent : Component
    {
        public string[] textures;
        public bool displayed;
        public string shaderName;
        public int timerFrameMS;
        public int currentImage;

        public bool flipX;
        public bool flipY;

        private float internalTimer;

        public SpriteAnimComponent(string[] textures, int currentImage = 0, int timerFrameMS = 250, string shaderName = "sprite", bool displayed = true) : base()
        {
            this.textures = textures;
            this.shaderName = shaderName;
            this.displayed = displayed;
            this.timerFrameMS = timerFrameMS;
            this.currentImage = currentImage;
        }

        public override void Load()
        {
            base.Load();

            foreach(string texture in textures)
                Graphics.Renderers.SpriteRenderer.Load(GetWindow(), shaderName, texture);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (textures.Length > currentImage)
            {
                if (internalTimer <= 0)
                {
                    if (currentImage >= textures.Length - 1)
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

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null || textures.Length == 0 || GetWindow().textureManager.GetTexture(textures[currentImage]) == null)
                return;


            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateScale(GetWindow().textureManager.GetTexture(textures[currentImage]).size.X / 2, GetWindow().textureManager.GetTexture(textures[currentImage]).size.Y / 2, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, textures[currentImage], flipX, flipY, model);
                }
            }
        }
    }
}
