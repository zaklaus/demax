//
//  Author:
//    Dominik Madarász combatwz.sk@gmail.com
//
//  Copyright (c) 2015, ZaKlaus
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	public class TexturedCube : Cube
	{
		public TexturedCube(CEntity e, string file="")
			: base(e)
		{
			VertCount = 24;
			IndiceCount = 36;
			TextureCoordsCount = 24;

			if (file != "")
				this.LoadTexture (file);

			meshes.Add (this);
		}

		public override Vector3[] GetVerts()
		{
			return new Vector3[] {
				//left
				new Vector3(-0.5f, -0.5f,  -0.5f),
				new Vector3(0.5f, 0.5f,  -0.5f),
				new Vector3(0.5f, -0.5f,  -0.5f),
				new Vector3(-0.5f, 0.5f,  -0.5f),

				//back
				new Vector3(0.5f, -0.5f,  -0.5f),
				new Vector3(0.5f, 0.5f,  -0.5f),
				new Vector3(0.5f, 0.5f,  0.5f),
				new Vector3(0.5f, -0.5f,  0.5f),

				//right
				new Vector3(-0.5f, -0.5f,  0.5f),
				new Vector3(0.5f, -0.5f,  0.5f),
				new Vector3(0.5f, 0.5f,  0.5f),
				new Vector3(-0.5f, 0.5f,  0.5f),

				//top
				new Vector3(0.5f, 0.5f,  -0.5f),
				new Vector3(-0.5f, 0.5f,  -0.5f),
				new Vector3(0.5f, 0.5f,  0.5f),
				new Vector3(-0.5f, 0.5f,  0.5f),

				//front
				new Vector3(-0.5f, -0.5f,  -0.5f), 
				new Vector3(-0.5f, 0.5f,  0.5f), 
				new Vector3(-0.5f, 0.5f,  -0.5f),
				new Vector3(-0.5f, -0.5f,  0.5f),

				//bottom
				new Vector3(-0.5f, -0.5f,  -0.5f), 
				new Vector3(0.5f, -0.5f,  -0.5f),
				new Vector3(0.5f, -0.5f,  0.5f),
				new Vector3(-0.5f, -0.5f,  0.5f)

			};
		}

		public override int[] GetIndices(int offset = 0)
		{
			int[] inds = new int[] {
				//left
				0,1,2,0,3,1,

				//back
				4,5,6,4,6,7,

				//right
				8,9,10,8,10,11,

				//top
				13,14,12,13,15,14,

				//front
				16,17,18,16,19,17,

				//bottom 
				20,21,22,20,22,23
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

		public override Vector2[] GetTextureCoords()
		{
			return new Vector2[] {
				// left
				new Vector2(0.0f, 0.0f),
				new Vector2(-1.0f, 1.0f),
				new Vector2(-1.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				// back
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(-1.0f, 1.0f),
				new Vector2(-1.0f, 0.0f),

				// right
				new Vector2(-1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(-1.0f, 1.0f),

				// top
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(-1.0f, 0.0f),
				new Vector2(-1.0f, 1.0f),

				// front
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 0.0f),

				// bottom
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(-1.0f, 1.0f),
				new Vector2(-1.0f, 0.0f)
			};
		}
	}
}

