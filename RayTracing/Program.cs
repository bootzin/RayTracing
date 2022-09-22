using System;

namespace RayTracing
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            int width = 800;
            int height = 600;
            int samples = 16;
            int depth = 5;
            if (args.Length > 2)
            {
                width = int.Parse(args[2]);
                height = int.Parse(args[3]);
                if (args.Length > 4)
				{
                    samples = int.Parse(args[4]);
                    depth = int.Parse(args[5]);
				}
            }

            Engine prog = new Engine(width, height, "Raio Tracejante", args[0], args[1], samples, depth);
            prog.Close();
        }
    }
}
