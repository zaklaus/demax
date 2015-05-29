using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace Demax
{
	public class Cube : Volume
	{
		

		public Cube (CEntity e) : base (e)
		{
			VertCount = 8;
			IndiceCount = 36;
			ColorDataCount = 8;
			NormalCount = VertCount;
		}

		public override Vector3[] GetVerts()
		{
			return new Vector3[] {
				new Vector3(-0.5f, -0.5f,  -0.5f),
				new Vector3(0.5f, -0.5f,  -0.5f),
				new Vector3(0.5f, 0.5f,  -0.5f),
				new Vector3(-0.5f, 0.5f,  -0.5f),
				new Vector3(-0.5f, -0.5f,  0.5f),
				new Vector3(0.5f, -0.5f,  0.5f),
				new Vector3(0.5f, 0.5f,  0.5f),
				new Vector3(-0.5f, 0.5f,  0.5f),
			};
		}



		public override Vector2[] GetTextureCoords()
		{
			return new Vector2[]{};
		}

		public override int[] GetIndices(int offset = 0)
		{
			int[] inds = new int[] {
				//left
				0, 2, 1,
				0, 3, 2,
				//back
				1, 2, 6,
				6, 5, 1,
				//right
				4, 5, 6,
				6, 7, 4,
				//top
				2, 3, 6,
				6, 3, 7,
				//front
				0, 7, 3,
				0, 4, 7,
				//bottom
				0, 1, 5,
				0, 5, 4
			};



			if (offset != 0)
			{
				for (int i = 0; i < inds.Length; i++)
				{
					inds[i] += offset;
				}
			}

			return inds;
		}

		public override Vector3[] GetColorData()
		{
			return new Vector3[] {
				new Vector3( 1f, 0f, 0f),
				new Vector3( 0f, 0f, 1f),
				new Vector3( 0f, 1f, 0f),
				new Vector3( 1f, 0f, 0f),
				new Vector3( 0f, 0f, 1f),
				new Vector3( 0f, 1f, 0f),
				new Vector3( 1f, 0f, 0f),
				new Vector3( 0f, 0f, 1f)
			};
		}




	}
}

