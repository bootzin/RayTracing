using OpenTK;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace RayTracing
{
    public static class Utils
    {
        public const float Infinity = float.MaxValue;
        public const float Epsilon = 1e-4f;

        public static Random Rand => ThreadLocalRandom.Instance;

		public static bool GammaCorrection { get; set; }

		public static void WriteColor(this StreamWriter sw, Vector3 color, int samplesPerPixel)
        {
            float gamma = GammaCorrection ? 2.2f : 1f;
            color = Vector3.Clamp(color / samplesPerPixel, Vector3.Zero, Vector3.One * .999f);
            sw.WriteLine($"{(int)(MathF.Pow(color.X, 1 / gamma) * 256)} {(int)(MathF.Pow(color.Y, 1 / gamma) * 256)} {(int)(MathF.Pow(color.Z, 1 / gamma) * 256)}");
        }

        public static float Deg2Rad(float angle)
        {
            return MathF.PI * angle / 180f;
        }

        public static Vector3 ToVector3(this string[] array)
        {
            return new Vector3(
                float.Parse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture),
                float.Parse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture),
                float.Parse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        public static Vector4 ToVector4(this string[] array)
        {
            return new Vector4(
                float.Parse(array[0], NumberStyles.Float, CultureInfo.InvariantCulture),
                float.Parse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture),
                float.Parse(array[2], NumberStyles.Float, CultureInfo.InvariantCulture),
                float.Parse(array[3], NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        public static Vector3 RandomInUnitSphere()
        {
            Vector3 ret;
            do
            {
                ret = (2f * new Vector3((float)Rand.NextDouble(), (float)Rand.NextDouble(), (float)Rand.NextDouble())) - Vector3.One;
            } while (ret.LengthSquared >= 1f);
            return ret;
        }

        public static Vector3 RandomInHemisphere(Vector3 normal)
        {
            Vector3 inUnitSphere = RandomInUnitSphere();
            return Vector3.Dot(inUnitSphere, normal) > 0 ? inUnitSphere : -inUnitSphere;
        }

        public static class ThreadLocalRandom
        {
            /// <summary>
            /// Random number generator used to generate seeds,
            /// which are then used to create new random number
            /// generators on a per-thread basis.
            /// </summary>
            private static readonly Random globalRandom = new Random();
            private static readonly object globalLock = new object();

            /// <summary>
            /// Random number generator
            /// </summary>
            private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(NewRandom);

            /// <summary>
            /// Creates a new instance of Random. The seed is derived
            /// from a global (static) instance of Random, rather
            /// than time.
            /// </summary>
            public static Random NewRandom()
            {
                lock (globalLock)
                {
                    return new Random(globalRandom.Next());
                }
            }

            /// <summary>
            /// Returns an instance of Random which can be used freely
            /// within the current thread.
            /// </summary>
            public static Random Instance { get { return threadRandom.Value; } }

            /// <summary>See <see cref="Random.Next()" /></summary>
            public static int Next()
            {
                return Instance.Next();
            }

            /// <summary>See <see cref="Random.Next(int)" /></summary>
            public static int Next(int maxValue)
            {
                return Instance.Next(maxValue);
            }

            /// <summary>See <see cref="Random.Next(int, int)" /></summary>
            public static int Next(int minValue, int maxValue)
            {
                return Instance.Next(minValue, maxValue);
            }

            /// <summary>See <see cref="Random.NextDouble()" /></summary>
            public static double NextDouble()
            {
                return Instance.NextDouble();
            }

            /// <summary>See <see cref="Random.NextBytes(byte[])" /></summary>
            public static void NextBytes(byte[] buffer)
            {
                Instance.NextBytes(buffer);
            }
        }
    }
}
