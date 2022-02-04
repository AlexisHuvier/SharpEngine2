namespace SE2.Widgets
{
    public class Widget
    {
        internal Scene scene;
        public int id;
        public Utils.Vec3 position;
        public Utils.Vec3 scale;
        public int rotation;
        public bool displayed;
        public bool active;

        public Widget(Utils.Vec3 position, Utils.Vec3 scale, int rotation, bool displayed, bool active)
        {
            this.position = position;
            this.displayed = displayed;
            this.active = active;
            this.scale = scale;
            this.rotation = rotation;
            id = -1;
        }

        public virtual void SetScene(Scene w) => scene = w;
        public virtual Scene GetScene() => scene;

        public Window GetWindow()
        {
            if (scene != null)
                return scene.window;
            return null;
        }

        public virtual void Init() { }
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void OnResize(Utils.Vec2 size) { }
        public virtual void OnTextInput(char key) { }
        public virtual void Update(double deltaTime) { }
        public virtual void Render() { }
    }
}
