using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Diagnostics;


namespace SE2.Components
{
    public class SpriteSheetComponent: Component
    {
        public string texture;
        public bool displayed;
        public string shaderName;
        public Utils.Vec2 spriteSize;
        public Dictionary<string, List<int>> animations;
        public int timerFrameMS;
        public bool flipX;
        public bool flipY;

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

        public override void Load()
        {
            base.Load();

            Graphics.Renderers.SpriteSheetRenderer.Load(GetWindow(), shaderName, texture);
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

            if (!displayed || texture.Length == 0 || currentAnim.Length == 0 || spriteSize == new Utils.Vec2(0) || !animations.ContainsKey(currentAnim) || GetWindow().shaderManager.GetShader(shaderName) == null || GetWindow().textureManager.GetTexture(texture) == null)
                return;


            foreach (Entities.Entity e in entities)
            {
                if (e.GetComponent<TransformComponent>() is TransformComponent tc)
                {
                    Matrix4 model = Matrix4.Identity
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(tc.rotation))
                        * Matrix4.CreateScale(spriteSize.x / 2, spriteSize.y / 2, 1)
                        * Matrix4.CreateScale(tc.scale.x, tc.scale.y, 1)
                        * Matrix4.CreateTranslation(new Vector3(tc.position.x, tc.position.y, tc.position.z));

                    Graphics.Renderers.SpriteSheetRenderer.Render(GetWindow(), shaderName, texture, animations[currentAnim][currentImage], spriteSize, flipX, flipY, model);
                }
            }
        }
    }
}
