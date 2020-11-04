using OpenTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace RayTracing
{
    public sealed class Light
    {
        public Vector3 Position { get; }
        public Vector3 Color { get; }
        public float ConstantCoeff { get; }
        public float LinearCoeff { get; }
        public float SquareCoeff { get; }


        public Light(Vector3 pos, Vector3 color, Vector3 coeffs, bool ambientLight)
        {
            if (!ambientLight)
                Position = pos;
            else
                Position = Vector3.Zero;

            Color = color;
            ConstantCoeff = coeffs.X;
            LinearCoeff = coeffs.Y;
            SquareCoeff = coeffs.Z;
        }
    }
}
