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
            if (args.Length == 4)
            {
                width = int.Parse(args[2]);
                height = int.Parse(args[3]);
            }

            Engine prog = new Engine(width, height, "Raio Tracejante", args[0], args[1]);
            prog.Close();
            //prog.Run(30);
        }
    }
}
