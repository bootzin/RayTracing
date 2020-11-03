using OpenTK;

namespace RayTracing
{
    public class Ray
    {
        public Vector3 Dir { get; }
        public Vector3 Origin { get; }

        public Ray(Vector3 point, Vector3 dir)
        {
            Dir = dir;
            Origin = point;
        }

        public Vector3 PointAt(float t) => Origin + (t * Dir);
    }
}
