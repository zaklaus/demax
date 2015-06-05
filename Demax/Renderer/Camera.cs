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
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter.Collision;
using Jitter;
using Jitter.Collision;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Joints;
using Jitter.Collision.Shapes;

namespace Demax
{
	public class Camera
	{
		/// <summary>
		/// The position.
		/// </summary>
		public Vector3 Position = Vector3.Zero;
        public Vector3 LookAt = Vector3.Zero;
		public Camera()
		{
		}
		/// <summary>
		/// The field of view.
		/// </summary>
		public float FieldOfView = 75.0f;
		/// <summary>
		/// The near plane.
		/// </summary>
		public float nearPlane = 0.01f;
		/// <summary>
		/// The far plane.
		/// </summary>
		public float farPlane = 500.0f;



		/// <summary>
		/// The orientation.
		/// </summary>
		public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);

		/// <summary>
		/// The move speed.
		/// </summary>
		public float MoveSpeed = 0.2f;

		/// <summary>
		/// The mouse sensitivity.
		/// </summary>
		public float MouseSensitivity = 0.01f;

		/// <summary>
		/// Gets the view matrix.
		/// </summary>
		/// <returns>The view matrix.</returns>
		public Matrix4 GetViewMatrix()
		{
            if(LookAt!=Vector3.Zero)
                return Matrix4.LookAt(Position, Position + LookAt, Vector3.UnitY);

			Vector3 lookat = new Vector3();

			lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
			lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = ((float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y)));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
		}

		/// <summary>
		/// Updates the camera.
		/// </summary>
		public void UpdateCamera()
		{
			//Position = new Vector3 (body.Position.X, body.Position.Y, body.Position.Z);
		}

		/// <summary>
		/// Move the specified x, y and z.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		public void Move(float x, float y, float z)
		{
			Vector3 offset = new Vector3();

			Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
			Vector3 right = new Vector3(-forward.Z, 0, forward.X);

			offset += x * right;
			offset += y * forward;
			offset.Y += z;

			offset.NormalizeFast();
			offset = Vector3.Multiply(offset, MoveSpeed);

			Position += offset;


			//5dy.Position = new JVector(Position.X+tPosition.X,Position.Y+tPosition.Y,Position.Z+tPosition.Z);
			//body.AddForce(new JVector(offset.X,offset.Y,offset.Z));
		}

		/// <summary>
		/// Adds the rotation.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void AddRotation(float x, float y, float z = 0.0f)
		{
			x = x * MouseSensitivity;
			y = y * MouseSensitivity;

			Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
			Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
            Orientation.Z = (Orientation.Z + z) % ((float)Math.PI * 2.0f);
		}

        public void SetRotation(float x, float y, float z)
        {
            Orientation.X = x;
            Orientation.Y = y;
            Orientation.Z = z;
        }

        public void LookAtf(Vector3 v)
        {
            LookAt = v;
        }
	}
}

