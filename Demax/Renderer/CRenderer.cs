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
using System.IO;
using System.Drawing.Imaging;
using System.Linq;
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
	/// <summary>
	/// Handles rendering and shader compilation.
	/// </summary>
	public class CRenderer
	{
		GameWindow gameRenderer;
        GLControl inlineRenderer;
		CCore core;
		Camera mainCamera;
        public GUIManager guiManager = new GUIManager();

		bool updated, onDemand, loaded = false;
		float time = 0f;
		float deltaTime, lastTime = 0.0f;
		float zeroKill = -100.0f;
		Matrix4 viewmodel, projection = Matrix4.Identity;
		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <value>The time.</value>
		public float Time
		{
			get {
				return time;
			}
		}

		/// <summary>
		/// Gets the delta time.
		/// </summary>
		/// <value>The delta time.</value>
		public float DeltaTime
		{
			get {
				return deltaTime;
			}
		}

		public void SetZeroKill(float k)
		{
			zeroKill = k;
		}

		public Dictionary<string, CShaderProgram> shaders = new Dictionary<string, CShaderProgram>();
		public Dictionary<string, int> textures = new Dictionary<string, int>();

		string activeShader = "default";

		Vector3[] vertdata, coldata, normdata;
		Vector2[] texcoorddata;
		int[] indicedata;
		int ibo_elements;
		//Matrix4[] mviewdata;

		/// <summary>
		/// Inits the test.
		/// </summary>
		void initTest()
		{
			GL.GenBuffers(1, out ibo_elements);
		}


		void updateTest(Volume v)
		{
				try{
				
				v.CalculateModelMatrix ();
				v.ViewProjectionMatrix = projection;
				v.ViewModelMatrix = viewmodel;
				v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewModelMatrix * v.ViewProjectionMatrix;
				int vertcount = 0;

				//foreach(CEntity en in core.EntityManager.GetEntities())
				//{
					//foreach (Volume v in en.GetModels())
					//{
				if (!v.isVisible)
					return;

				List<Vector3> verts = new List<Vector3>();
				List<int> inds = new List<int>();
				List<Vector3> colors = new List<Vector3>();
				List<Vector3> norms = new List<Vector3>();
				List<Vector2> texcoords = new List<Vector2>();
				//		if (v.meshes.Count > 0) {
				//			foreach (Volume mesh in v.meshes) {
								
								verts.AddRange(v.GetVerts().ToList());
								inds.AddRange(v.GetIndices().ToList());
								colors.AddRange(v.GetColorData().ToList());
								texcoords.AddRange(v.GetTextureCoords().ToList());
								norms.AddRange (v.GetNormals ().ToList());
								vertcount += v.VertCount;
				//			}
				//		}
				//	}
				//}

				vertdata = verts.ToArray();
				indicedata = inds.ToArray();
				normdata = norms.ToArray ();
				coldata = colors.ToArray();
				texcoorddata = texcoords.ToArray();

				activeShader = CShaderProgram.LoadShaderPointer(v.Shader);

				GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vPosition"));

				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
				GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 0, 0);

				if (shaders [activeShader].GetAttribute ("vNormal") != -1) {
					GL.BindBuffer (BufferTarget.ArrayBuffer, shaders [activeShader].GetBuffer ("vNormal"));

					GL.BufferData<Vector3> (BufferTarget.ArrayBuffer, (IntPtr)(normdata.Length * Vector3.SizeInBytes), normdata, BufferUsageHint.StaticDraw);
					GL.VertexAttribPointer (shaders [activeShader].GetAttribute ("vNormal"), 3, VertexAttribPointerType.Float, false, 0, 0);
				}
				// Indice Buffer
				GL.BindBuffer (BufferTarget.ElementArrayBuffer, ibo_elements);
				GL.BufferData<int> (BufferTarget.ElementArrayBuffer,
					(IntPtr)(indicedata.Length * sizeof(int)),
					indicedata,
					BufferUsageHint.StaticDraw);

				if (shaders[activeShader].GetAttribute("vColor") != -1)
				{
					GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("vColor"));
					GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(coldata.Length * Vector3.SizeInBytes), coldata, BufferUsageHint.StaticDraw);
					GL.VertexAttribPointer(shaders[activeShader].GetAttribute("vColor"), 3, VertexAttribPointerType.Float, true, 0, 0);
				}

				if (shaders[activeShader].GetAttribute("texcoord") != -1)
				{
					GL.BindBuffer(BufferTarget.ArrayBuffer, shaders[activeShader].GetBuffer("texcoord"));
					GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(texcoorddata.Length * Vector2.SizeInBytes), texcoorddata, BufferUsageHint.StaticDraw);
					GL.VertexAttribPointer(shaders[activeShader].GetAttribute("texcoord"), 2, VertexAttribPointerType.Float, true, 0, 0);
				}
			}
			catch(Exception ex){
				Console.WriteLine (ex.ToString ());
			}
		}

		/// <summary>
		/// Flush this instance.
		/// </summary>
		public void Flush(Volume e)
		{
			updateTest (e);
		}

		/// <summary>
		/// Initializes game renderer. (OpenGL)
		/// </summary>
		public CRenderer ()
		{
			core = CCore.GetCore ();
		}

        public void Init()
        {
            gameRenderer = core.GameRenderer;
            mainCamera = core.MainCamera;
            inlineRenderer = core.inlineRenderer;

            if (core.type == "window")
            {
                gameRenderer.Load += OnLoad;
                gameRenderer.Resize += OnResize;
                gameRenderer.UpdateFrame += OnUpdateFrame;
                gameRenderer.RenderFrame += OnRenderFrame;
                gameRenderer.FocusedChanged += gameRenderer_FocusedChanged;

                gameRenderer.KeyDown += core.InputManager.OnKeyDown;
                gameRenderer.KeyUp += core.InputManager.OnKeyUp;
                gameRenderer.KeyPress += core.InputManager.OnKeyPress;
            }
            else if(core.type == "inline")
            {
        
            }
        }

        public void gameRenderer_FocusedChanged(object sender, EventArgs e)
        {
            core.InputManager.Focused = !core.InputManager.Focused;
        }

		static class RequiredFeatures
		{
			public static readonly Version FramebufferObject = new Version(3, 0);
		}
		public void OnLoad(object sender, EventArgs e)
		{
			gameRenderer.VSync = VSyncMode.On;
			GL.Enable (EnableCap.DepthTest);
			GL.Enable (EnableCap.CullFace);

			Dictionary<string, bool> extensions =
				new Dictionary<string, bool>();
			int count = GL.GetInteger(GetPName.NumExtensions);
			for (int i = 0; i < count; i++)
			{
				string extension = GL.GetString(StringNameIndexed.Extensions, i);
				extensions.Add(extension, true);
			}

			if (extensions.ContainsKey("ARB_fragment_program"))
			{
				Console.WriteLine("Sorry, Your GPU is not supported!");
				core.GameRenderer.Close ();
				Console.ReadKey();
				return;
			}


			initTest ();
			/* TEST CODE */

			/* TEST CODE */

			//updateTest ();

			loaded = true;
		}

        public void OnResize(object sender, EventArgs e)
		{
			GL.Viewport (gameRenderer.ClientRectangle.X, gameRenderer.ClientRectangle.Y, gameRenderer.ClientRectangle.Width, gameRenderer.ClientRectangle.Height);

		}

		/// <summary>
		/// Sets the on demand alloc.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public void SetOnDemandAlloc(bool state)
		{
			this.onDemand = state;
		}

        public void OnUpdateFrame(object sender, FrameEventArgs e)
		{

			//df.DoFall ();

			core.world.Step (1.0f / 100.0f, true);

			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume v in en.GetModels()) {
                    List<RigidBody> b = new List<RigidBody>(v.body);
                    foreach(var r in b)
					if (v.body != null && !r.IsStatic) {
						v.Position = new Vector3 (r.Position.X, r.Position.Y, r.Position.Z);
						v.Rotation = v.JMatrixToMatrix (r.Orientation);

						if (v.Position.Y < zeroKill) {
							if (v.killOnZero) {
								v.Destroy ();
							} else {
								/*v.RemoveRigidbody ();
								v.Position = Vector3.Zero;
								v.Rotation = Matrix4.Identity;
								v.isVisible = false;*/
							}
						}
					}
				}
			}
			bool shouldFlush = false;
			foreach (CEntity en in core.EntityManager.GetEntities()) {
				List<Volume> l = en.GetModels ();
				foreach (Volume v in l.ToList()) {
					if (v.markedForDestroy) {
						en.Models.Remove (v);
						shouldFlush = true;
					}
				}
			}

			//if(onDemand || shouldFlush)
				//updateTest ();

			viewmodel = mainCamera.GetViewMatrix();
			projection = Matrix4.CreatePerspectiveFieldOfView ((float)Math.PI / 4, gameRenderer.Width / (float)gameRenderer.Height, CCore.GetCore().MainCamera.nearPlane, CCore.GetCore().MainCamera.farPlane);


			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume v in en.GetModels()) {
					v.CalculateModelMatrix ();
					v.ViewProjectionMatrix = projection;
					v.ViewModelMatrix = viewmodel;
					v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewModelMatrix * v.ViewProjectionMatrix;
				}
			}
			// Modelview Buffer

			GL.ClearColor (Color.CornflowerBlue);
			GL.PointSize (5f);

			updated = true;
			time += (float)e.Time;

			CScript.BroadcastEvent ("OnUpdate");
		}

        public void OnRenderFrame(object sender, FrameEventArgs e)
		{
			if (updated && loaded) {
				updated = false;
			} else {
				return;
			}

			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			int indiceat = 0;

			foreach (CEntity entity in core.EntityManager.GetEntities())
			{
				foreach (Volume v in entity.GetModels())
				{
					int i=0;
					foreach (Volume m in v.meshes) {
						GL.UseProgram (shaders [CShaderProgram.LoadShaderPointer(m.Shader)].ProgramID);
						shaders[CShaderProgram.LoadShaderPointer(m.Shader)].EnableVertexAttribArrays();

						updateTest (m);
						if (!m.isVisible) {
							indiceat += m.IndiceCount;
							continue;
						}
						int tex = 0;

						if (v.materials.Count == 0)
							tex = v.TextureID;
						else
						//TODO: proper multi-texture handling
							foreach (var mat in v.materials)
								if (mat.name == m.matuse [0]) {
									tex = mat.TextureID;
								}
						

						GL.BindTexture (TextureTarget.Texture2D, tex);
						GL.UniformMatrix4 (shaders [activeShader].GetUniform ("M"), false, ref v.ModelMatrix);
						GL.UniformMatrix4 (shaders [activeShader].GetUniform ("V"), false, ref v.ViewModelMatrix);
						GL.UniformMatrix4 (shaders [activeShader].GetUniform ("P"), false, ref v.ViewProjectionMatrix);
						GL.UniformMatrix4 (shaders [activeShader].GetUniform ("MVP"), false, ref v.ModelViewProjectionMatrix);

						if (shaders [activeShader].GetAttribute ("maintexture") != -1) {
							GL.Uniform1 (shaders [activeShader].GetAttribute ("maintexture"), tex);
						}
						try{
							//if(v.materials.Count == 0)
								GL.DrawElements (PrimitiveType.Triangles, m.IndiceCount, DrawElementsType.UnsignedInt, 0);
							//else
								//GL.DrawArrays(PrimitiveType.Triangles, 0, m.IndiceCount);
						} catch(Exception ex){
							Console.WriteLine (ex.ToString ());
						}
						indiceat += m.IndiceCount;
						i++;

						shaders[CShaderProgram.LoadShaderPointer(m.Shader)].DisableVertexAttribArrays();
					}
				}
			}

            //guiManager.Draw();

			gameRenderer.SwapBuffers();



			deltaTime = (float)e.Time;

			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (CScript cs in en.GetScripts()) {
					cs.CallEvent ("OnRender");
				}
			}
		}



		private JVector M2V(JMatrix wantedOrientation)
		{
			// this is something like correction = wantedPosition - position
			JMatrix q = JMatrix.Inverse(wantedOrientation);// * testBody.Orientation;
			JVector axis;

			float x = q.M32 - q.M23;
			float y = q.M13 - q.M31;
			float z = q.M21 - q.M12;

			float r = JMath.Sqrt(x * x + y * y + z * z);
			float t = q.M11 + q.M22 + q.M33;

			float angle = (float)Math.Atan2(r, t - 1);
			axis = new JVector(x, y, z) * angle;

			if (r != 0.0f) axis = axis * (1.0f / r);

			return axis;
		}
    }
}

