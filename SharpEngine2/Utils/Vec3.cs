namespace SE2.Utils
{
    public class Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3(float same)
        {
            x = same;
            y = same;
            z = same;
        }

        public Vec3(Vec2 vec, float z_)
        {
            x = vec.x;
            y = vec.y;
            z = z_;
        }

        public Vec3(float x_, float y_)
        {
            x = x_;
            y = y_;
            z = 0;
        }

        public Vec3(float x_, float y_, float z_)
        {
            x = x_;
            y = y_;
            z = z_;
        }

        public Vec3 Normalized()
        {
            var length = Length();
            if (length == 0)
                return null;
            else
                return new Vec3(x / length, y / length, z / length);
        }

        public float Length()
        {
            return (float)System.Math.Sqrt(x * x + y * y + z * z);
        }

        public float LengthSquared()
        {
            return x * x + y * y + z * z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec3 vec)
                return this == vec;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Vec3(x={x}, y={y}, z={z})";
        }

        public static bool operator !=(Vec3 vec1, Vec3 Vec3)
            => !(vec1 == Vec3);

        public static bool operator ==(Vec3 vec1, Vec3 Vec3)
        {
            if (vec1 is null)
                return Vec3 is null;
            else if (Vec3 is null)
                return false;
            else
                return vec1.x == Vec3.x && vec1.y == Vec3.y && vec1.z == Vec3.z;
        }

        public static Vec3 operator -(Vec3 vec)
            => vec == null ? null : new Vec3(-vec.x, -vec.y, -vec.z);

        public static Vec3 operator -(Vec3 vec, Vec3 Vec3)
            => vec == null || Vec3 == null ? null : new Vec3(vec.x - Vec3.x, vec.y - Vec3.y, vec.z - Vec3.z);

        public static Vec3 operator -(Vec3 vec, float factor)
            => vec == null ? null : new Vec3(vec.x - factor, vec.y - factor, vec.z - factor);
        public static Vec3 operator +(Vec3 vec, Vec3 Vec3)
            => vec == null || Vec3 == null ? null : new Vec3(vec.x + Vec3.x, vec.y + Vec3.y, vec.z + Vec3.z);

        public static Vec3 operator +(Vec3 vec, float factor)
            => vec == null ? null : new Vec3(vec.x + factor, vec.y + factor, vec.z + factor);

        public static Vec3 operator *(Vec3 vec, Vec3 Vec3)
            => vec == null || Vec3 == null ? null : new Vec3(vec.x * Vec3.x, vec.y * Vec3.y, vec.z * Vec3.z);

        public static Vec3 operator *(Vec3 vec, float factor)
            => vec == null ? null : new Vec3(vec.x * factor, vec.y * factor, vec.z * factor);

        public static Vec3 operator /(Vec3 vec, Vec3 Vec3)
            => vec == null || Vec3 == null ? null : new Vec3(vec.x / Vec3.x, vec.y / Vec3.y, vec.z / Vec3.z);

        public static Vec3 operator /(Vec3 vec, float factor)
            => vec == null ? null : new Vec3(vec.x / factor, vec.y / factor, vec.z / factor);
    }
}
