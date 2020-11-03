using OpenTK;
using System;
using System.IO;

namespace RayTracing
{
    public static class Utils
    {
        public static void WriteColor(this StreamWriter sw, Vector3 color)
        {
            sw.WriteLine($"{(int)(color.X * 255)} {(int)(color.Y * 255)} {(int)(color.Z * 255)}");
        }

        public static float Deg2Rad(float angle)
        {
            return MathF.PI * angle / 180f;
        }

        public static Vector3 ToVector3(this string[] array)
        {
            return new Vector3(
                float.Parse(array[0]),
                float.Parse(array[1]),
                float.Parse(array[2]));
        }

        public static Vector4 ToVector4(this string[] array)
        {
            return new Vector4(
                float.Parse(array[0]),
                float.Parse(array[1]),
                float.Parse(array[2]),
                float.Parse(array[3]));
        }

        internal static Vector3 RayColor(Ray r)
        {
            var t = .5f * (r.Dir.Normalized().Y + 1f);
            var sphere = new Sphere(null, null, new Vector3(0, 0, -1), .5f);
            bool hit = sphere.Hit(r, 0, 100, out RayHit rayHit);
            if (hit && rayHit.T > 0)
                return .5f * (rayHit.Normal + Vector3.One);
            return ((1 - t) * Vector3.One) + (t * new Vector3(.5f, .7f, 1f));
        }
    }
}
