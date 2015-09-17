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
using System.Collections.Generic;
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System.Drawing.Imaging;

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

			meshes.Add (this);
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

		public override Vector3[] GetNormals()
		{
			return GetVerts ();
		}

		public override Vector2[] GetTextureCoords()
		{
			return new Vector2[]{};
		}

		public override void AddRigidbody(string type = "")
		{
            var z = new RigidBody(new Jitter.Collision.Shapes.BoxShape(Scale.X, Scale.Y, Scale.Z));
            body.Add(z);
			z.Position = new JVector(Position.X,Position.Y,Position.Z);
			//body.Orientation = (JVector)Rotation;
			CCore.GetCore().world.AddBody(z);
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

