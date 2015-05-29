using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System.Drawing.Imaging;

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
		public Matrix4 ViewModelMatrix = Matrix4.Identity;
		public RigidBody body;

		public Volume(CEntity x)
		{
			me = x;

			x.game.GetCell().Flush ();
		}

		public void LoadTexture(string file)
		{
			try {
				this.TextureID = this.loadImage (file);	
			} catch (Exception ex) {
				Console.WriteLine ("IO Error: "+ ex.ToString ());
			}

		}

		int loadImage(Bitmap image)
		{
			int texID = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, texID);
			BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			image.UnlockBits(data);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			return texID;
		}

		/// <summary>
		/// Loads the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="filename">Filename.</param>
		public int loadImage(string filename)
		{
			try
			{
				Bitmap file = new Bitmap(filename);
				int id = loadImage(file);
				return id;
			}
			catch
			{
				return -1;
			}
		}



		public void SetPosition(Vector3 x)
		{
			Position = x;

			if(body!=null)
				body.Position = new JVector(Position.X+me.RecursiveTransform().Position.X,Position.Y+me.RecursiveTransform().Position.Y,Position.Z+me.RecursiveTransform().Position.Z);
		}

		public void SetStatic(bool state)
		{
			body.AffectedByGravity = !state;
			body.IsStatic = state;
		}

		public void SetRotation(Matrix4 x)
		{
			Rotation = x;

			if (body != null)
				body.Orientation = MatrixToJMatrix (Rotation);
		}

		public void SetScale(Vector3 x)
		{
			Scale = x;

			if(body!=null)
				body.Shape = new Jitter.Collision.Shapes.BoxShape(Scale.X+me.RecursiveTransform().Scale.X,Scale.Y+me.RecursiveTransform().Scale.Y,Scale.Z+me.RecursiveTransform().Scale.Z);
		}

		public Vector3[] GetNormals()
		{
			return GetVerts ();
		}

		public Vector3 CalculateNormalSurface(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 V1 = (p2 - p1);
			Vector3 V2 = (p3 - p1);

			Vector3 surfaceNormal = Vector3.Zero;
			surfaceNormal.X = (V1.Y*V2.Y) - (V1.Z-V2.Y);
			surfaceNormal.Y = - ( (V2.Z * V1.X) - (V2.X * V1.Z) );
			surfaceNormal.Z = (V1.X*V2.Y) - (V1.Y*V2.X);

			return surfaceNormal;
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

		public JMatrix MatrixToJMatrix(Matrix4 matrix)
		{
			return JMatrix.Identity;
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

		public void CalculateModelMatrix()
		{ 
			ModelMatrix = Matrix4.Scale(Scale) * Rotation * Matrix4.CreateTranslation(Position + me.RecursiveTransform().Position);
		}
 
        public abstract Vector3[] GetVerts();
        public abstract int[] GetIndices(int offset = 0);
		public abstract Vector3[] GetColorData();

		public abstract Vector2[] GetTextureCoords();
	}
}

