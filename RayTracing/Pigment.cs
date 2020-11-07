using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RayTracing
{
	public sealed class Pigment
	{
		private Vector3 color;
		private Vector3 color2;
		private readonly float cubeSide;
		private readonly List<Vector3> TextureData = new List<Vector3>();
		private int TexWidth;
		private int TexHeight;

		private readonly PigmentType Type;

		public Pigment(Vector3 color)
		{
			this.color = color;
			Type = PigmentType.Solid;
		}

		public Pigment(Vector3 color1, Vector3 color2, float cubeSide)
		{
			this.color = color1;
			this.color2 = color2;
			this.cubeSide = cubeSide;
			Type = PigmentType.Checker;
		}

		public Pigment(string fileName, Vector4 p0, Vector4 p1)
		{
			Type = PigmentType.Texture;
			ReadFile(fileName);
			P0 = p0;
			P1 = p1;
		}

		public Vector4 P0 { get; }
		public Vector4 P1 { get; }

		public Vector3 ColorAt(Vector3 pC)
		{
			switch (Type)
			{
				case PigmentType.Solid:
					return color;

				case PigmentType.Checker:
					float val = (MathF.Floor(pC.X / cubeSide)
							+ MathF.Floor(pC.Y / cubeSide)
							+ MathF.Floor(pC.Z / cubeSide)) % 2;
					return val == 0 ? color : color2;

				case PigmentType.Texture:
					var s = Vector4.Dot(P0, new Vector4(pC));
					var r = Vector4.Dot(P1, new Vector4(pC));
					int i = (int)(r * TexHeight) % TexHeight;
					int j = (int)(s * TexWidth) % TexWidth;
					if (i < 0) i += TexHeight;
					if (j < 0) j += TexWidth;
					return TextureData[(i * TexWidth) + j];
				default:
					return color;
			}
		}
		private void ReadFile(string fileName)
		{
			var reader = new BinaryReader(new FileStream(fileName, FileMode.Open));

			if (reader.ReadChar() != 'P' || reader.ReadChar() != '6')
				throw new NotSupportedException("Ppm file type not supported!");
			reader.ReadChar(); //Eat newline
			if (reader.PeekChar() == 10)
				reader.ReadChar(); //Eat windows newline

			while (reader.PeekChar() == '#')
			{
				while (reader.ReadChar() != '\n') ;
			}

			StringBuilder widthSb = new StringBuilder();
			StringBuilder heightSb = new StringBuilder();
			char temp;
			while ((temp = reader.ReadChar()) != ' ')
				widthSb.Append(temp);
			while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
				heightSb.Append(temp);

			if (reader.PeekChar() == 10)
				reader.ReadChar(); //Eat windows newline

			if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5')
				throw new NotSupportedException("Max color must be 255!");

			reader.ReadChar(); //Eat the last newline
			if (reader.PeekChar() == 10)
				reader.ReadChar(); //Eat the last windows newline

			TexWidth = int.Parse(widthSb.ToString());
			TexHeight = int.Parse(heightSb.ToString());

			//Read in the pixels
			for (int y = 0; y < TexHeight; y++)
			{
				for (int x = 0; x < TexWidth; x++)
				{
					TextureData.Insert((y * TexWidth) + x, new Vector3()
					{
						X = reader.ReadByte() / 255f,
						Y = reader.ReadByte() / 255f,
						Z = reader.ReadByte() / 255f
					});
				}
			}
		}
	}
}
