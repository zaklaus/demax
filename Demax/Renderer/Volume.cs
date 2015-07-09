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
	public class Material
	{
		public int TextureID;
		public string name;

		public void LoadTexture(string file)
		{
			try {
				if(!CCore.GetCore().Renderer.textures.ContainsKey(file)){
					this.TextureID = this.loadImage (file);	
					CCore.GetCore().Renderer.textures.Add(file,this.TextureID);
				}
				else
					this.TextureID = CCore.GetCore().Renderer.textures[file];
			} catch (Exception ex) {
				CLog.WriteLine ("IO Error: "+ ex.ToString ());
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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);

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

		public Material(string name, string texture)
		{
			this.name = name;
			this.LoadTexture (texture);
		}
	}

	public abstract class Volume
	{
		public Vector3 Position = Vector3.Zero;
		public Matrix4 Rotation = Matrix4.Identity;
        public Vector3 Scale = Vector3.One;
		public CEntity me;
        public Vector3[] vertices;
        public Vector3[] colors;
        public Vector3[] normals;
        public Vector2[] texturecoords;
		public bool isStatic = false;
		public bool isVisible = true;
		public virtual int VertCount { get; set; }
		public virtual int NormalCount { get; set; }
		public virtual int IndiceCount { get; set; }
		public virtual int ColorDataCount { get; set; }
		public bool IsTextured = false;
		public bool killOnZero = true;
		public int TextureID;
		public int TextureCoordsCount;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;
		public Matrix4 ViewModelMatrix = Matrix4.Identity;
		public List<RigidBody> body = new List<RigidBody>();
        public List<int> indices = new List<int>();
		public List<Volume> meshes = new List<Volume>();

        public Dictionary<string, List<Volume>> anims = new Dictionary<string, List<Volume>>();
        public int cframe = 0;
        public string cname = "";
        public float frameStep = 0.0f;
        public float frameTime = 0.0f;
        public Volume cvolume;

        public Dictionary<int, string> matuse = new Dictionary<int, string> ();
		public List<Material> materials = new List<Material> ();
		public bool markedForDestroy = false;
		public Tuple<int,int,int> maxIndex = new Tuple<int, int, int> (0,0,0);
		public string Shader = "light";
		public Volume(CEntity x)
		{
			me = x;
		}

		public void LoadTexture(string file)
		{
			try {
				if(!CCore.GetCore().Renderer.textures.ContainsKey(file)){
					this.TextureID = this.loadImage (file);	
					CCore.GetCore().Renderer.textures.Add(file,this.TextureID);
				}
				else
					this.TextureID = CCore.GetCore().Renderer.textures[file];
			} catch (Exception ex) {
				CLog.WriteLine ("IO Error: "+ ex.ToString ());
			}

		}

        public void TickAnim()
        {
            cframe++;
            if (cframe >= anims[cname].Count)
                cframe = 0;
        }

        public void PlayAnim(string anim)
        {
            cname = anim;
        }

        public void LoadAnim(string name, List<Volume> frames)
        {
            anims.Add(name, frames);
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

		public void Destroy()
		{
            List<RigidBody> b = new List<RigidBody>(body);
            foreach(var r in b)
			    this.RemoveRigidbody (r);
			this.markedForDestroy = true;
		}

		public void AddForce(Vector3 f)
		{
			if (body != null) {
                foreach(var r in body)
				    r.AddForce (new JVector (f.X, f.Y, f.Z));
			}
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
                foreach(var r in body)
				r.Position = new JVector(Position.X+me.RecursiveTransform().Position.X,Position.Y+me.RecursiveTransform().Position.Y,Position.Z+me.RecursiveTransform().Position.Z);
		}

		public void SetStatic(bool state)
		{
			if (body != null) {
                foreach (var r in body)
                {
                    r.AffectedByGravity = !state;
                    r.IsStatic = state;
                }
			}
		}

		public void ZeroGravity(bool state)
		{
			if (body != null)
                foreach (var r in body)
				r.AffectedByGravity = !state;
		}

		public void SetRotation(Vector3 x)
		{
			Rotation = Matrix4.CreateRotationX(x.X) * Matrix4.CreateRotationY(x.Y) * Matrix4.CreateRotationZ(x.Z);

			if (body != null)
                foreach (var r in body)
				r.Orientation = MatrixToJMatrix (Rotation);
		}

		public void SetRotation(Matrix4 x)
		{
			Rotation = x;

			if (body != null)
                foreach (var r in body)
				r.Orientation = MatrixToJMatrix (Rotation);
		}


		public void SetScale(Vector3 x)
		{
			Scale = x;

			if(body!=null)
                foreach (var r in body)
				r.Shape = new Jitter.Collision.Shapes.BoxShape(Scale.X+me.RecursiveTransform().Scale.X,Scale.Y+me.RecursiveTransform().Scale.Y,Scale.Z+me.RecursiveTransform().Scale.Z);
		}

		/// <summary>
		/// Gets the normals.
		/// </summary>
		/// <returns>The normals.</returns>
		public abstract Vector3[] GetNormals ();

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
			return new JMatrix (
				matrix.M11,
				matrix.M12,
				matrix.M13,
				matrix.M21,
				matrix.M22,
				matrix.M23,
				matrix.M31,
				matrix.M32,
				matrix.M33);
		}



		/// <summary>
		/// Adds the rigidbody.
		/// </summary>
		public abstract void AddRigidbody();


		/// <summary>
		/// Removes the rigidbody.
		/// </summary>
		public void RemoveRigidbody(RigidBody id)
		{
            try
            {
                CCore.GetCore().world.RemoveBody(id);
                body.Remove(id);
            }
            catch { }
		}

		public void CalculateModelMatrix()
		{ 
			ModelMatrix = Matrix4.Scale(Scale*me.RecursiveTransform().Scale) * (Rotation * Matrix4.CreateRotationX(me.RecursiveTransform().Rotation.X)
                 * Matrix4.CreateRotationY(me.RecursiveTransform().Rotation.Y)
                  * Matrix4.CreateRotationZ(me.RecursiveTransform().Rotation.Z)) * Matrix4.CreateTranslation(Position + me.RecursiveTransform().Position);
		}
 
        public abstract Vector3[] GetVerts();
        public abstract int[] GetIndices(int offset = 0);
		public abstract Vector3[] GetColorData();

		public abstract Vector2[] GetTextureCoords();
	}
}

