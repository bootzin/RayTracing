using OpenTK;
using System;
using System.Collections.Generic;

namespace RayTracing
{
    public sealed class Polyhedron : EngineObject
    {
        public List<Face> Faces { get; } = new List<Face>();
        public Polyhedron(Pigment pigment, Finishing finishing)
        {
            Pigment = pigment;
            Finishing = finishing;
        }

		public override bool Hit(Ray ray, float tMin, float tMax, out RayHit hit)
		{
			float tt, t0 = Utils.Epsilon, t1 = Utils.Infinity;
			Vector3 nt0, nt1;
			nt0 = nt1 = Vector3.Zero;
			foreach (var face in Faces)
			{
				Vector3 p0 = ray.Origin;
				Vector3 n = face.OriginalNormal;
				float dn = Vector3.Dot(ray.Dir, n);
				float val = Vector3.Dot(p0, n) + face.D;

				if (dn <= Utils.Epsilon && dn >= -Utils.Epsilon && val > Utils.Epsilon)
				{
					t1 = -1.0f;
				}
				if (dn > Utils.Epsilon)
				{
					tt = -val / dn;
					if (tt < t1)
					{
						// Replace the furthest point.
						t1 = tt;
						nt1 = n;
					}
				}
				if (dn < -Utils.Epsilon)
				{
					tt = -val / dn;
					if (tt > t0)
					{
						t0 = tt;
						nt0 = n;
					}
				}
			}

			if (MathF.Abs(t0) <= Utils.Epsilon && (t1 >= t0) && t1 < Utils.Infinity)
			{
				hit = new RayHit(ray.PointAt(t1), t1, this, -(nt1).Normalized());
				return t1 < tMax;
			}
			if (t0 > Utils.Epsilon && t1 >= t0)
			{
				hit = new RayHit(ray.PointAt(t0), t0, this, (nt0).Normalized());
				return t0 < tMax;
			}
			hit = new RayHit();
			return false;
		}
	}
}
