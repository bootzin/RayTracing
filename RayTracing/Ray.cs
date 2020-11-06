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
                if (Scatter(rayHit, lightList, objList, out Vector3 attenuation, out Ray scattered))
                    return attenuation;// * scattered.RayColor(objList, lightList, depth - 1);
                return Vector3.Zero;
			}

            var t = .5f * (Dir.Normalized().Y + 1f);
            return lightList[0].Color;// ((1 - t) * Vector3.One) + (t * new Vector3(.5f, .7f, 1f));
        }

		private bool Scatter(RayHit hit, List<Light> lightList, List<EngineObject> objList, out Vector3 attenuation, out Ray scattered)
		{
            if (hit.ObjHit.Finishing.Kr < -10)
			{
                Vector3 reflected = Reflect(hit.Normal);
                scattered = new Ray(hit.Position, reflected);
                bool scatter = Vector3.Dot(scattered.Dir, hit.Normal) > 0;
                attenuation = Vector3.Zero;
                if (scatter)
                {
                    Vector3 ambient = hit.ObjHit.Finishing.Ka * lightList[0].Color * hit.ObjHit.Pigment.ColorAt(hit.Position);
                    for (int i = 1; i < lightList.Count; i++)
                    {
                        ambient += GetLightContribution(hit, lightList[i], objList);
                    }
                    attenuation = ambient * hit.ObjHit.Finishing.Kr;
                }
                return scatter;
            }
            else
			{
                Vector3 target = hit.Position + Utils.RandomInHemisphere(hit.Normal);
                scattered = new Ray(hit.Position, target - hit.Position);
                Vector3 ambient = hit.ObjHit.Finishing.Ka * lightList[0].Color * hit.ObjHit.Pigment.ColorAt(hit.Position);
                for (int i = 1; i < lightList.Count; i++)
				{
                    ambient += GetLightContribution(hit, lightList[i], objList);
				}
				attenuation = ambient;
                return true;
            }
		}

        private Vector3 GetLightContribution(RayHit hit, Light light, List<EngineObject> objList)
		{
            var obj = hit.ObjHit;

            Vector3 diffuse, specular;
            diffuse = specular = Vector3.Zero;

			Vector3 L = light.Position - hit.Position;
            Ray lightRay = new Ray(hit.Position, L);
            if (light.Position != Vector3.Zero
                && (!lightRay.HitAnything(objList, out RayHit tmpHit)
                || L.LengthFast < (tmpHit.Position - hit.Position).LengthFast))
            {
                float distance = L.Length;
				float attentuation = 1 / (light.ConstantCoeff + (light.LinearCoeff * distance) + (light.SquareCoeff * (distance * distance)));

				Vector3 lightDir = L.Normalized();
                float diff = MathF.Max(Vector3.Dot(lightDir, hit.Normal), 0f);
                diffuse = diff * light.Color * obj.Finishing.Kd * attentuation;

                Vector3 reflected = Reflect(lightDir, hit.Normal);
                float spec = MathF.Pow(MathF.Max(Vector3.Dot(reflected, (Engine.Camera.Eye - hit.Position).Normalized()), 0f), obj.Finishing.Alpha);
                specular = obj.Finishing.Ks * spec * light.Color * attentuation;
			}

            return Vector3.Clamp((diffuse * hit.ObjHit.Pigment.ColorAt(hit.Position)) + specular, Vector3.Zero, Vector3.One);
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
                if (objList[i].Hit(this, Utils.Epsilon, closest, out RayHit tempHit))
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
