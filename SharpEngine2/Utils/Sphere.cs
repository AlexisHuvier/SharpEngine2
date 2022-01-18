using System;
using System.Collections.Generic;
using System.Text;

namespace SE2.Utils
{
    class Sphere
    {
        public Vec3 position;
        public float radius;

        public Sphere(Vec3 pos, float rad)
        {
            position = pos;
            radius = rad;
        }

        public Sphere(float x, float y, float z, float radius) : this(new Vec3(x, y, z), radius) { }

        public override bool Equals(object obj)
        {
            if (obj is Sphere sphere)
                return this == sphere;
            return base.Equals(obj);
        }

        public bool Intersect(Sphere other)
        {
            float dx = position.x - other.position.x;
            float dy = position.y - other.position.y;
            float dz = position.z - other.position.z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz) <= radius + other.radius;
        }

        public bool Intersect(Rect other)
        {
            float testX = position.x;
            float testY = position.y;
            float testZ = position.z;

            if (position.x < other.position.x - other.size.x / 2) testX = other.position.x - other.size.x / 2;
            else if (position.x > other.position.x + other.size.x / 2) testX = other.position.x + other.size.x / 2;
            if (position.y < other.position.y - other.size.y / 2) testY = other.position.y - other.size.y / 2;
            else if (position.y > other.position.y + other.size.y / 2) testY = other.position.y + other.size.y / 2;
            if (position.z < other.position.z - other.size.z / 2) testZ = other.position.z - other.size.z / 2;
            else if (position.z > other.position.z + other.size.z / 2) testZ = other.position.z + other.size.z / 2;

            float distX = position.x - testX;
            float distY = position.y - testY;
            float distZ = position.z - testZ;
            double distance = Math.Sqrt((distX * distX) + (distY * distY) + (distZ * distZ));

            if (distance <= radius)
                return true;
            return false;
        }

        public override string ToString() => $"Sphere(position={position}, radius={radius})";

        public override int GetHashCode() => HashCode.Combine(position, radius);

        public static bool operator !=(Sphere r1, Sphere r2) => !(r1 == r2);

        public static bool operator ==(Sphere r1, Sphere r2)
        {
            if (r1 is null)
                return r2 is null;
            else if (r2 is null)
                return false;
            else
                return r1.position == r2.position && r1.radius == r2.radius;
        }
    }
}
