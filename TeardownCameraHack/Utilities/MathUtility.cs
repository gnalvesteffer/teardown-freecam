using System;
using System.Numerics;

namespace TeardownCameraHack.Utilities
{
    public static class MathUtility
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            var result = value;
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            return result;
        }

        public static Vector3 Normalized(this Vector3 vector)
        {
            var length = vector.Length();
            if (length > 0.0f)
            {
                return vector / length;
            }
            return Vector3.Zero;
        }

        public static float WrapAngle(float radians)
        {
            return (float)Math.Atan2(Math.Sin(radians), Math.Cos(radians));
        }
    }
}
