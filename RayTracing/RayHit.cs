using OpenTK;

namespace RayTracing
{
    public class RayHit
    {
        public Vector3 Position { get; }
        public Vector3 Normal { get; }
        public float T { get; }

        public RayHit()
        {
            Position = new Vector3();
            Normal = new Vector3();
            T = 0;
        }

        public RayHit(Vector3 pos, Vector3 normal, float t)
        {
            Position = pos;
            Normal = normal;
            T = t;
        }
    }
}
