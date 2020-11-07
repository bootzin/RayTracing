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
			hit = new RayHit();
			for (int i = 0; i < Faces.Count; i++)
			{
				float denom = Vector3.Dot(ray.Dir, Faces[i].Normal);
				if (denom < -Utils.Epsilon) // denom can't be 0
				{
					float t = (Vector3.Dot(Faces[i].PointInPlane - ray.Origin, Faces[i].OriginalNormal)) / denom;
					if (t > tMin && t < tMax)
					{
						hit = new RayHit(ray.PointAt(t), t, this);
						hit.SetNormal(ray, Faces[i].Normal.Normalized());
						return true;
					}
				}
			}
            return false;
		}
	}
}
