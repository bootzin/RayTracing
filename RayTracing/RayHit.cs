using OpenTK;

namespace RayTracing
{
    public class RayHit
    {
        public Vector3 Position { get; }
        public Vector3 Normal { get; private set; }
        public EngineObject ObjHit { get; }
		public float T { get; }
		public bool FrontFace { get; set; }

		public RayHit()
        {
            Position = new Vector3();
            Normal = new Vector3();
            T = -1;
        }

        public RayHit(Vector3 pos, Vector3 normal, float t)
        {
            Position = pos;
            Normal = normal;
            T = t;
        }

        public RayHit(Vector3 pos, float t, EngineObject objHit)
        {
            Position = pos;
            ObjHit = objHit;
            T = t;
        }

        public RayHit(Vector3 pos, float t, EngineObject objHit, Vector3 normal)
        {
            Position = pos;
            ObjHit = objHit;
            T = t;
            Normal = normal;
        }

        public void SetNormal(Ray r, Vector3 outNormal)
		{
            FrontFace = Vector3.Dot(r.Dir, outNormal) < 0;
            Normal = FrontFace ? outNormal : -outNormal;
		}
    }
}
