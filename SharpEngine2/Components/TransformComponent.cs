namespace SE2.Components
{
    public class TransformComponent : Component
    {
        public Utils.Vec3 position;
        public Utils.Vec3 scale;
        public int rotation;

        public TransformComponent(Utils.Vec3 position, Utils.Vec3 scale, int rotation) : base()
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }

        public TransformComponent(Utils.Vec3 position, Utils.Vec3 scale) : this(position, scale, 0) { }
        public TransformComponent(Utils.Vec3 position, int rotation) : this(position, new Utils.Vec3(1), rotation) { }
        public TransformComponent(int rotation) : this(new Utils.Vec3(0), new Utils.Vec3(1), rotation) { }
        public TransformComponent(Utils.Vec3 position) : this(position, new Utils.Vec3(1), 0) { }
        public TransformComponent() : this(new Utils.Vec3(0), new Utils.Vec3(1), 0) { }
    }
}
