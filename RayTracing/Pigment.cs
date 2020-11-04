using OpenTK;

namespace RayTracing
{
	public sealed class Pigment
    {
		public Vector3 Color { get; }
		public Vector3? Color2 { get; }

		public Pigment(Vector3 color)
        {
            Color = color;
        }

        public Pigment(Vector3 color1, Vector3 color2, float cubeSide)
        {
            Color = color1;
            Color2 = color2;
        }

        public Pigment(string fileName, Vector4 p0, Vector4 p1)
        {
        }
    }
}
