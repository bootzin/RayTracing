using System.Collections.Generic;

namespace RayTracing
{
    public sealed class Polyhedron : EngineObject
    {
        public List<Face> Faces { get; } = new List<Face>();
        public Polyhedron(Pigment pigment, Finishing finishing, int faceCount)
        {
        }

		public override bool Hit(Ray ray, float tMin, float tMax, out RayHit hit)
		{
			throw new System.NotImplementedException();
		}
	}
}
