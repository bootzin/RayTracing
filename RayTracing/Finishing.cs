namespace RayTracing
{
	public sealed class Finishing
	{
		public float Ka { get; set; }
		public float Kd { get; set; }
		public float Ks { get; set; }
		public float Alpha { get; set; }
		public float Kr { get; set; }
		public float Kt { get; set; }
		public float Ior { get; set; }

		public Finishing(float ka, float kd, float ks, float alpha, float kr, float kt, float ior)
		{
			Ka = ka;
			Kd = kd;
			Ks = ks;
			Alpha = alpha;
			Kr = kr;
			Kt = kt;
			Ior = ior;
		}
	}
}
