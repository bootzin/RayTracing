using OpenTK;
using System;

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
			OriginalNormal = Normal = new Vector3(a, b, c);
			if (Vector3.Dot(Normal, PointInPlane) > 0)
				Normal *= -1;
		}

		public Vector3 Position { get; }
		public Vector3 Normal { get; }
		public Vector3 OriginalNormal { get; }

		public bool ContainsPoint(Vector3 p) => MathF.Abs((a * p.X) + (b * p.Y) + (c * p.Z) + D) <= Utils.Epsilon;

		public Vector3 PointInPlane
		{
			get
			{
				if (c != 0)
					return new Vector3(0, 0, -D / c);
				if (a != 0)
					return new Vector3(-D / a, 0, 0);
				if (b != 0)
					return new Vector3(0, -D / b, 0);
				return new Vector3(-a * D, -b * D, -c * D);
			}
		}
	}
}
