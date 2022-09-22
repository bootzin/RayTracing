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
			if (depth <= 0)
				return Vector3.Zero;

			if (HitAnything(objList, out RayHit rayHit))
			{
				if (Scatter(rayHit, lightList, objList, out Vector3 attenuation, depth))
					return attenuation;
				return Vector3.Zero;
			}

			return lightList[0].Color;
		}

		private bool Scatter(RayHit hit, List<Light> lightList, List<EngineObject> objList, out Vector3 attenuation, int depth)
		{
			Vector3 color = hit.ObjHit.Finishing.Ka * lightList[0].Color * hit.ObjHit.Pigment.ColorAt(hit.Position);
			for (int i = 1; i < lightList.Count; i++)
			{
				color += GetLightContribution(hit, lightList[i], objList);
			}
			color += Reflection(hit, lightList, objList, depth);
			color += Refraction(hit, lightList, objList, depth);
			attenuation = Vector3.Clamp(color, Vector3.Zero, Vector3.One);
			return true;
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

			return (diffuse * hit.ObjHit.Pigment.ColorAt(hit.Position)) + specular;
		}

		private Vector3 Reflection(RayHit hit, List<Light> lightList, List<EngineObject> objList, int depth)
		{
			if (depth <= 0 || hit.ObjHit.Finishing.Kr == 0)
				return Vector3.Zero;

			Vector3 reflected = Reflect(hit.Normal);
			Ray scattered = new Ray(hit.Position, reflected);
			if (Vector3.Dot(scattered.Dir, hit.Normal) > Utils.Epsilon)
				return scattered.RayColor(objList, lightList, depth - 1) * hit.ObjHit.Finishing.Kr;
			return Vector3.Zero;
		}

		private Vector3 Refraction(RayHit hit, List<Light> lightList, List<EngineObject> objList, int depth)
		{
			if (depth < 0 || hit.ObjHit.Finishing.Kt == 0)
				return Vector3.Zero;
			float refr = hit.FrontFace ? 1f / hit.ObjHit.Finishing.Ior : hit.ObjHit.Finishing.Ior;

			var scattered = Refract(hit, refr);
			return scattered.RayColor(objList, lightList, depth - 1) * hit.ObjHit.Finishing.Kt;
		}

		private Ray Refract(RayHit hit, float refractivity)
		{
			var uv = Dir.Normalized();
			float cosTheta = MathF.Min(Vector3.Dot(-uv, hit.Normal), 1);
			Vector3 rOutPerp = refractivity * (uv + (cosTheta * hit.Normal));
			Vector3 rOutParallel = -MathF.Sqrt(MathF.Abs(1 - rOutPerp.LengthSquared)) * hit.Normal;
			return new Ray(hit.Position, rOutPerp + rOutParallel);
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

			for (int i = 0; i < objList.Count ; i++)
			{
				if (objList[i].Hit(this, Utils.Epsilon, closest, out RayHit tempHit))
				{
					hitAnything = true;
					hit = tempHit;
					closest = tempHit.T;
				}
			}

			return hitAnything;
		}
	}
}
