namespace RayTracing
{
    public abstract class EngineObject
    {
        public Pigment Pigment { get; protected set; }
        public Finishing Finishing { get; protected set; }
        public abstract bool Hit(Ray ray, float tMin, float tMax, out RayHit hit);
    }
}
