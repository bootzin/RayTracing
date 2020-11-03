using System.Collections.Generic;

namespace RayTracing
{
    public sealed class Polyhedron : EngineObject
    {
        public List<Face> Faces { get; } = new List<Face>();
        public Polyhedron(Pigment pigment, Finishing finishing, int faceCount)
        {
        }
    }
}
