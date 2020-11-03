using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace RayTracing
{
    public sealed class Engine : GameWindow
    {
        private readonly string outputFilePath;
        private readonly List<Light> SceneLights = new List<Light>();
        private readonly List<Pigment> Pigments = new List<Pigment>();
        private readonly List<Finishing> Finishings = new List<Finishing>();
        private readonly List<EngineObject> EngineObjects = new List<EngineObject>();
        private Camera Camera { get; set; }

        public Engine(int width, int height, string title, string inputFilePath, string outputFilePath) : base(width, height, new GraphicsMode(new ColorFormat(32), 16, 0, 4, new ColorFormat(0), 2, false), title)
        {
            this.outputFilePath = outputFilePath;

            Init(inputFilePath);
        }

        private void Init(string inputFilePath)
        {
            ReadFile(inputFilePath);
            SaveFile();
        }

        private void SaveFile()
        {
            using StreamWriter sw = new StreamWriter(outputFilePath);
            sw.WriteLine("P3");
            sw.WriteLine($"{Width} {Height}");
            sw.WriteLine("255");

            for (int j = Height - 1; j >= 0; j--)
            {
                Console.WriteLine($"Scanlines remaining: {j}");
                for (int i = 0; i < Width; i++)
                {
                    var u = (float)i / (Width - 1);
                    var v = (float)j / (Height - 1);

                    Ray r = new Ray(Camera.Eye, Camera.LowerLeftCorner + u * Camera.Horizontal + v * Camera.Vertical - Camera.Eye);
                    sw.WriteColor(Utils.RayColor(r));
                }
            }
            sw.Flush();
            Console.WriteLine("Done!");
        }

        private void ReadFile(string inputFilePath)
        {
            using StreamReader sr = new StreamReader(Path.GetFullPath(inputFilePath));

            string[] cameraPosStr = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] cameraTargetStr = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] cameraUpStr = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string cameraFov = sr.ReadLine().Trim();

            Camera = new Camera(
                cameraPosStr.ToVector3(),
                cameraTargetStr.ToVector3(),
                cameraUpStr.ToVector3(),
                float.Parse(cameraFov),
                Width / Height,
                .01f);

            int lightCount = int.Parse(sr.ReadLine().Trim());
            for (int i = 0; i < lightCount; i++)
            {
                string[] lightInfo = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string[] lightPosStr = new string[] { lightInfo[0], lightInfo[1], lightInfo[2] };
                string[] lightColorStr = new string[] { lightInfo[3], lightInfo[4], lightInfo[5] };
                string[] lightCoeffStr = new string[] { lightInfo[6], lightInfo[7], lightInfo[8] };

                SceneLights.Add(new Light(
                    lightPosStr.ToVector3(),
                    lightColorStr.ToVector3(),
                    lightCoeffStr.ToVector3(),
                    i == 0));
            }

            int pigmentCount = int.Parse(sr.ReadLine().Trim());
            for (int i = 0; i < pigmentCount; i++)
            {
                string[] pigmentInfo = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                switch (pigmentInfo[0])
                {
                    case "solid":
                        Pigments.Add(new Pigment(new string[] { pigmentInfo[1], pigmentInfo[2], pigmentInfo[3] }.ToVector3()));
                        break;
                    case "checker":
                        Pigments.Add(new Pigment(new string[] { pigmentInfo[1], pigmentInfo[2], pigmentInfo[3] }.ToVector3(),
                            new string[] { pigmentInfo[4], pigmentInfo[5], pigmentInfo[6] }.ToVector3(),
                            float.Parse(pigmentInfo[7])));
                        break;
                    case "textmap":
                        string fileName = pigmentInfo[1];
                        Vector4 p0 = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToVector4();
                        Vector4 p1 = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToVector4();
                        Pigments.Add(new Pigment(fileName, p0, p1));
                        break;
                }
            }

            int finishingCount = int.Parse(sr.ReadLine().Trim());
            for (int i = 0; i < finishingCount; i++)
            {
                string[] finishingInfo = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                float ka = float.Parse(finishingInfo[0]);
                float kd = float.Parse(finishingInfo[1]);
                float ks = float.Parse(finishingInfo[2]);
                float alpha = float.Parse(finishingInfo[3]);
                float kr = float.Parse(finishingInfo[4]);
                float kt = float.Parse(finishingInfo[5]);
                float ior = float.Parse(finishingInfo[6]);
                Finishings.Add(new Finishing(ka, kd, ks, alpha, kr, kt, ior));
            }

            int objCount = int.Parse(sr.ReadLine().Trim());
            for (int i = 0; i < objCount; i++)
            {
                string[] objInfo = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int pIndex = int.Parse(objInfo[0]);
                int fIndex = int.Parse(objInfo[1]);

                switch (objInfo[2])
                {
                    case "sphere":
                        EngineObjects.Add(new Sphere(Pigments[pIndex], Finishings[fIndex],
                            new string[] { objInfo[3], objInfo[4], objInfo[5] }.ToVector3(), float.Parse(objInfo[6])));
                        break;
                    case "polyhedron":
                        int faceCount = int.Parse(objInfo[3]);
                        var poly = new Polyhedron(Pigments[pIndex], Finishings[fIndex], faceCount);
                        for (int j = 0; j < faceCount; j++)
                        {
                            string[] faceInfo = sr.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            float a = float.Parse(faceInfo[0]);
                            float b = float.Parse(faceInfo[1]);
                            float c = float.Parse(faceInfo[2]);
                            float d = float.Parse(faceInfo[3]);
                            poly.Faces.Add(new Face(a, b, c, d));
                        }
                        EngineObjects.Add(poly);
                        break;
                }
            }
        }
    }
}