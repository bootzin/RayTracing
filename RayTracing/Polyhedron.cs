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
            foreach(var face in Faces)
            {
				float denom = Vector3.Dot(ray.Dir, face.Normal);
				if (MathF.Abs(denom) > Utils.Epsilon)
				{
					float t = (Vector3.Dot(face.PointInPlane - ray.Origin, face.Normal)) / denom;
					if (t > tMin && t < tMax)
					{
						hit = new RayHit(ray.PointAt(t), t, this);
						hit.SetNormal(ray, face.Normal.Normalized());
						return true;
					}
				}
			}
            hit = new RayHit();
            return false;
		}
	}
}
