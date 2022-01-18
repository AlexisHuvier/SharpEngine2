using System;

namespace SE2.Utils
{
    class Rect
    {
        public Vec3 position;
        public Vec3 size;

        public Rect(Vec3 pos, Vec3 siz)
        {
            position = pos;
            size = siz;
        }

        public Rect(float x, float y, float z, Vec3 size) : this(new Vec3(x, y, z), size) { }
        public Rect(Vec3 pos, float width, float height, float prof) : this(pos, new Vec3(width, height, prof)) { }
        public Rect(float x, float y, float z, float width, float height, float prof) : this(new Vec3(x, y, z), new Vec3(width, height, prof)) { }

        public override bool Equals(object obj)
        {
            if (obj is Rect rect)
                return this == rect;
            return base.Equals(obj);
        }

        public bool Intersect(Rect other)
        {
            return position.x - size.x / 2 < other.position.x + other.size.x / 2 &&
                position.x + size.x / 2 > other.position.x - other.size.x / 2 &&
                position.y - size.y / 2 < other.position.y + other.size.y / 2 &&
                position.y + size.y / 2 > other.position.y - other.size.y / 2 &&
                position.z - size.z / 2 < other.position.z + other.size.z / 2 &&
                position.z + size.z / 2 > other.position.z - other.size.z / 2;
        }

        public bool Intersect(Sphere other)
        {
            float testX = other.position.x;
            float testY = other.position.y;
            float testZ = other.position.z;

            if (other.position.x < position.x - size.x / 2) testX = position.x - size.x / 2;
            else if (other.position.x > position.x + size.x / 2) testX = position.x + size.x / 2;
            if (other.position.y < position.y - size.y / 2) testY = position.y - size.y / 2;
            else if (other.position.y > position.y + size.y / 2) testY = position.y + size.y / 2;
            if (other.position.z < position.z - size.z / 2) testZ = position.z - size.z / 2;
            else if (other.position.z > position.z + size.z / 2) testZ = position.z + size.z / 2;

            float distX = other.position.x - testX;
            float distY = other.position.y - testY;
            float distZ = other.position.z - testZ;
            double distance = System.Math.Sqrt((distX * distX) + (distY * distY) + (distZ * distZ));

            if (distance <= other.radius)
                return true;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(position, size);
        public override string ToString() => $"Rect(position={position}, size={size})";
        public static bool operator !=(Rect r1, Rect r2) => !(r1 == r2);

        public static bool operator ==(Rect r1, Rect r2)
        {
            if (r1 is null)
                return r2 is null;
            else if (r2 is null)
                return false;
            else
                return r1.position == r2.position && r1.size == r2.size;
        }
    }
}
