using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace Demax
{
	public abstract class Volume
	{
		public Vector3 Position = Vector3.Zero;
		public Matrix4 Rotation = Matrix4.Identity;
        public Vector3 Scale = Vector3.One;
		public CEntity me;
		public bool isStatic = false;
        public int VertCount;
        public int IndiceCount;
		public int NormalCount;
        public int ColorDataCount;
		public bool IsTextured = false;
		public int TextureID;
		public int TextureCoordsCount;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;
		public RigidBody body;

		public Volume(CEntity x)
		{
			me = x;
			//AddRigidbody ();
		}

		public void SetPosition(Vector3 x)
		{
			Position = x;

			if(body!=null)
				body.Position = new JVector(Position.X+me.RecursiveTransform().Position.X,Position.Y+me.RecursiveTransform().Position.Y,Position.Z+me.RecursiveTransform().Position.Z);
		}

		public void SetRotation(Matrix4 x)
		{
			Rotation = x;
		}

		public void SetScale(Vector3 x)
		{
			Scale = x;

			if(body!=null)
				body.Shape = new Jitter.Collision.Shapes.BoxShape(Scale.X+me.RecursiveTransform().Scale.X,Scale.Y+me.RecursiveTransform().Scale.Y,Scale.Z+me.RecursiveTransform().Scale.Z);
		}

		public Matrix4 JMatrixToMatrix(Jitter.LinearMath.JMatrix matrix)
		{
			return new Matrix4(matrix.M11,
				matrix.M12,
				matrix.M13,
				0.0f,
				matrix.M21,
				matrix.M22,
				matrix.M23,
				0.0f,
				matrix.M31,
				matrix.M32,
				matrix.M33,
				0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
		}

		/// <summary>
		/// Adds the rigidbody.
		/// </summary>
		public void AddRigidbody()
		{
			body = new RigidBody (new Jitter.Collision.Shapes.BoxShape (Scale.X, Scale.Y, Scale.Z));
			body.Position = new JVector(Position.X,Position.Y,Position.Z);
			//body.Orientation = (JVector)Rotation;
			CCore.GetCore().world.AddBody(body);
		}

		/// <summary>
		/// Removes the rigidbody.
		/// </summary>
		public void RemoveRigidbody()
		{
			CCore.GetCore ().world.RemoveBody (body);
			body = null;
		}
 
        public abstract Vector3[] GetVerts();
		public abstract Vector3[] GetNormals();
        public abstract int[] GetIndices(int offset = 0);
        public abstract Vector3[] GetColorData();
        public abstract void CalculateModelMatrix();
		public abstract Vector2[] GetTextureCoords();
	}
}

