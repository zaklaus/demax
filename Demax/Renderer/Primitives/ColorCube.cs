using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	public class ColorCube : Cube
	{
		Vector3 Color = new Vector3(1, 1, 1);

		public ColorCube(Vector3 color, CEntity e) : base(e) {
			Color = color;
		}

		public override Vector3[] GetColorData()
		{
			return new Vector3[] { 
				Color,
				Color, 
				Color,
				Color,
				Color, 
				Color, 
				Color, 
				Color
			};
		}
	}
}

