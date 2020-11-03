using OpenTK;
using System;

namespace RayTracing
{
    public sealed class Sphere : EngineObject
    {
        public Vector3 Center { get; }
        public float Radius { get; }
        public Pigment Pigment { get; }
        public Finishing Finishing { get; }

        public Sphere(Pigment pigment, Finishing finishing, Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            Pigment = pigment;
            Finishing = finishing;
        }

        public bool Hit(Ray ray, float tMin, float tMax, out RayHit hit)
        {
            var oc = ray.Origin - Center;
            var a = ray.Dir.LengthSquared;
            var halfB = Vector3.Dot(oc, ray.Dir);
            var c = oc.LengthSquared - (Radius * Radius);

            float discriminant = (halfB * halfB) - (a * c);
            if (discriminant < 0)
            {
                hit = new RayHit();
                return false;
            }

            float tmp = MathF.Sqrt(discriminant);
            float root = (-halfB - tmp) / a;
            if (root < tMax && root > tMin)
            {
                Vector3 position = ray.PointAt(root);
                Vector3 normal = (position - Center) / Radius;
                hit = new RayHit(ray.PointAt(root), normal, root);
                return true;
            }
            root = (-halfB + tmp) / a;
            if (root < tMax && root > tMin)
            {
                Vector3 position = ray.PointAt(root);
                Vector3 normal = (position - Center) / Radius;
                hit = new RayHit(position, normal, root);
                return true;
            }

            hit = new RayHit();
            return false;
        }
    }
}
