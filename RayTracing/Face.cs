using OpenTK;

namespace RayTracing
{
    public sealed class Face
    {
		private readonly float a;
		private readonly float b;
		private readonly float c;
		public float D { get; }

		public Face(float a, float b, float c, float d)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.D = d;
		}

		public Vector3 Position { get; }
		public Vector3 Normal => new Vector3(a, b, c);

		public bool ContainsPoint(Vector3 p) => (a * p.X) + (b * p.Y) + (c * p.Z) + D <= Utils.Epsilon;

		public Vector3 PointInPlane => new Vector3(-a * D, -b * D, -c * D);
	}
}
