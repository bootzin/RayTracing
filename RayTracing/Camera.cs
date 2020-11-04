using OpenTK;
using System;

namespace RayTracing
{
    public sealed class Camera
    {
        public Vector3 Eye { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public float LensRadius { get; }
        public Vector3 U { get; }
        public Vector3 V { get; }
        public Vector3 W { get; }
        public Vector3 LowerLeftCorner { get; }
        public Vector3 Horizontal { get; }
        public Vector3 Vertical { get; }

        public Camera(Vector3 eye, Vector3 target, Vector3 up, float vFov, float aspectRatio, float aperture)
        {
            Eye = eye;
            Target = target;
            Up = up;
            LensRadius = aperture / 2f;

            float theta = Utils.Deg2Rad(vFov);
            float halfHeight = MathF.Tan(theta / 2f);
            float halfWidth = aspectRatio * halfHeight;

            W = (eye - target).Normalized();
            U = Vector3.Cross(up, W).Normalized();
            V = Vector3.Cross(W, U);

            var focusDist = (eye - target).Length;
            LowerLeftCorner = Eye - (halfWidth * focusDist * U) - (halfHeight * focusDist * V) - (focusDist * W);
            Horizontal = 2 * halfWidth * focusDist * U;
            Vertical = 2 * halfHeight * focusDist * V;
        }

		public Ray GetRay(float u, float v)
		{
			return new Ray(Eye, LowerLeftCorner + (u * Horizontal) + (v * Vertical) - Eye);
        }
	}
}
