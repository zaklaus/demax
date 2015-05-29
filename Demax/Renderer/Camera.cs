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

		public Camera()
		{
		}

		public RigidBody body;

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
			Vector3 lookat = new Vector3();

			lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
			lookat.Y = (float)Math.Sin((float)Orientation.Y);
			lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

			return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
		}

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

			//body.Position = new JVector(Position.X+tPosition.X,Position.Y+tPosition.Y,Position.Z+tPosition.Z);
			//body.AddForce(new JVector(offset.X,offset.Y,offset.Z));
		}

		/// <summary>
		/// Adds the rotation.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void AddRotation(float x, float y)
		{
			x = x * MouseSensitivity;
			y = y * MouseSensitivity;

			Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
			Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
		}
	}
}

