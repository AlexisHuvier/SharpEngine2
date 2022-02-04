using OpenTK.Mathematics;

namespace SE2.Widgets
{
    public class Checkbox: Widget
    {
        public bool isChecked;
        public string shaderName;

        public Checkbox(Utils.Vec3 position, Utils.Vec3 scale, string shaderName = "shape", bool isChecked = true, int rotation = 0, bool displayed = true, bool active = true): base(position, scale, rotation, displayed, active)
        {
            this.isChecked = isChecked;
            this.shaderName = shaderName;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            if (!active)
                return;

            if (GetWindow().inputManager.IsMouseButtonPressed(Utils.Inputs.MouseButton.LEFT) && GetWindow().inputManager.MouseInRectangle(new Utils.Vec2(position.x, position.y) * GetWindow().camera.zoom, new Utils.Vec2(scale.x, scale.y) * GetWindow().camera.zoom))
                isChecked = !isChecked;
        }

        public override void Render()
        {
            base.Render();

            if (!displayed || scene == null || GetWindow().shaderManager.GetShader(shaderName) == null)
                return;

            if (isChecked)
            {

                Matrix4 whiteModel = Matrix4.Identity
                            * Matrix4.CreateScale((scale.x - 12) / 2, (scale.y - 12) / 2, scale.z / 2)
                            * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                            * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

                Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, Utils.Color.BLACK, whiteModel);
            }

            Matrix4 blackBGModel = Matrix4.Identity
                        * Matrix4.CreateScale(scale.x / 2, scale.y / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);
            Matrix4 bgModel = Matrix4.Identity
                        * Matrix4.CreateScale((scale.x - 6) / 2, (scale.y - 6) / 2, scale.z / 2)
                        * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation))
                        * Matrix4.CreateTranslation(new Vector3(position.x, position.y, position.z) + GetWindow().camera.Position);

            Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, Utils.Color.WHITE, bgModel);
            Graphics.Renderers.RectRenderer.Render(GetWindow(), shaderName, Utils.Color.BLACK, blackBGModel);
        }
    }
}
