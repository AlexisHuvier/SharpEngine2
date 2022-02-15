using OpenTK.Mathematics;
using System.Collections.Generic;

namespace SE2.Components
{
    public class SpriteAnimComponent : Component
    {
        public Dictionary<string, List<string>> textures;
        public bool displayed;
        public string shaderName;
        public int timerFrameMS;
        public int currentImage;

        public bool flipX;
        public bool flipY;

        internal string currentAnim;
        private float internalTimer;

        public SpriteAnimComponent(Dictionary<string, List<string>> textures, string currentAnim, int timerFrameMS = 250, string shaderName = "sprite", bool displayed = true) : base()
        {
            this.textures = textures;
            this.shaderName = shaderName;
            this.displayed = displayed;
            this.timerFrameMS = timerFrameMS;
            this.currentImage = 0;
            this.currentAnim = currentAnim;
        }

        public void SetAnim(string anim)
        {
            currentAnim = anim;
            currentImage = 0;
            internalTimer = timerFrameMS;
        }

        public string GetAnim() => currentAnim;

        public override void Load()
        {
            base.Load();

            foreach(string texture in textures[currentAnim])
                Graphics.Renderers.SpriteRenderer.Load(GetWindow(), shaderName, texture);
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (textures[currentAnim].Count > currentImage)
            {
                if (internalTimer <= 0)
                {
                    if (currentImage >= textures[currentAnim].Count - 1)
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

            if (!displayed || GetWindow().shaderManager.GetShader(shaderName) == null || textures[currentAnim].Count == 0 || GetWindow().textureManager.GetTexture(textures[currentAnim][currentImage]) == null)
                return;


            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateScale(GetWindow().textureManager.GetTexture(textures[currentAnim][currentImage]).size.X / 2, GetWindow().textureManager.GetTexture(textures[currentAnim][currentImage]).size.Y / 2, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, tc.scale.z)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.SpriteRenderer.Render(GetWindow(), shaderName, textures[currentAnim][currentImage], flipX, flipY, model);
                }
            }
        }
    }
}
