using OpenTK;
using System;

namespace RayTracing
{
    public sealed class Sphere : EngineObject
    {
        public Vector3 Center { get; }
        public float Radius { get; }

        public Sphere(Pigment pigment, Finishing finishing, Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            Pigment = pigment;
            Finishing = finishing;
        }

        public override bool Hit(Ray ray, float tMin, float tMax, out RayHit hit)
        {
            var oc = ray.Origin - Center;
            var a = ray.Dir.LengthSquared;
            var halfB = 2*Vector3.Dot(oc, ray.Dir);
            var c = oc.LengthSquared - (Radius * Radius);

            float discriminant = (halfB * halfB) - (4 *(a * c));
            if (discriminant < Utils.Epsilon)
            {
                hit = new RayHit();
                return false;
            }

            float tmp = MathF.Sqrt(discriminant);
            float t = (-halfB - tmp) / (2 * a);
            if (t > tMin && t < tMax)
            {
                Vector3 position = ray.PointAt(t);
                Vector3 normal = (position - Center) / Radius;
                hit = new RayHit(position, t, this);
                hit.SetNormal(ray, normal);
                return true;
            }
            t = (-halfB + tmp) / (2 * a);
            if (t > tMin && t < tMax)
            {
                Vector3 position = ray.PointAt(t);
                Vector3 normal = (position - Center) / Radius;
                hit = new RayHit(position, t, this);
                hit.SetNormal(ray, normal);
                return true;
            }

            hit = new RayHit();
            return false;
        }
    }
}
