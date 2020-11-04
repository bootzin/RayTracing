using OpenTK;
using System;
using System.Collections.Generic;

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

        public Vector3 RayColor(List<EngineObject> objList, List<Light> lightList, int depth)
        {
            if (depth < 0)
                return Vector3.Zero;

            if (HitAnything(objList, out RayHit rayHit))
			{
                if (Scatter(rayHit, lightList, out Vector3 attenuation, out Ray scattered))
                    return attenuation * scattered.RayColor(objList, lightList, depth - 1);
                return Vector3.Zero;
			}

            var t = .5f * (Dir.Normalized().Y + 1f);
            return ((1 - t) * Vector3.One) + (t * new Vector3(.5f, .7f, 1f));
        }

		private bool Scatter(RayHit hit, List<Light> lightList, out Vector3 attenuation, out Ray scattered)
		{
            if (hit.ObjHit.Finishing.Kr > 0)
			{
                Vector3 reflected = Reflect(hit.Normal);
                scattered = new Ray(hit.Position, reflected);
                attenuation = hit.ObjHit.Pigment.Color;
                return Vector3.Dot(scattered.Dir, hit.Normal) > 0;
            }
            else
			{
                Vector3 target = hit.Position + Utils.RandomInHemisphere(hit.Normal);
                scattered = new Ray(hit.Position, target - hit.Position);
                Vector3 ambient = Vector3.Zero;
				for (int i = 0; i < lightList.Count; i++)
				{
					ambient += GetLight(hit, lightList[i]);
				}
				attenuation = ambient / lightList.Count;
                return true;
            }
		}

        private Vector3 GetLight(RayHit hit, Light light)
		{
            var obj = hit.ObjHit;

			Vector3 ambient = obj.Finishing.Ka * light.Color;

            Vector3 diffuse, specular;
            diffuse = specular = Vector3.Zero;
            float attentuation = 1;
            if (light.Position != Vector3.Zero)
			{
                var L = light.Position - hit.Position;
                float distance = L.Length;
                attentuation = 1 / (light.ConstantCoeff + (light.LinearCoeff * distance) + (light.SquareCoeff * (distance * distance)));

                Vector3 lightDir = L.Normalized();
                float diff = MathF.Max(Vector3.Dot(lightDir, hit.Normal), 0f);
                diffuse = diff * light.Color * obj.Finishing.Kd;

                Vector3 reflected = Reflect(lightDir, hit.Normal);
                float spec = MathF.Pow(MathF.Max(Vector3.Dot(reflected, (Engine.Camera.Eye - hit.Position).Normalized()), 0f), obj.Finishing.Alpha);
                specular = obj.Finishing.Ks * spec * light.Color;
			}

            return (ambient + (attentuation * (diffuse + specular))) * hit.ObjHit.Pigment.Color;
		}

		private Vector3 Reflect(Vector3 l, Vector3 n)
		{
            return (2 * Vector3.Dot(l, n) * n) - l;
        }

		private Vector3 Reflect(Vector3 n)
		{
            var v = Dir.Normalized();
            return v - (2 * Vector3.Dot(v, n) * n);
		}

		private bool HitAnything(List<EngineObject> objList, out RayHit hit)
		{
            hit = new RayHit();
            bool hitAnything = false;
            float closest = Utils.Infinity;

            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].Hit(this, 0.001f, closest, out RayHit tempHit))
                {
                    hitAnything = true;
                    hit = tempHit;
                    closest = hit.T;
                }
            }

            return hitAnything;
        }
	}
}
