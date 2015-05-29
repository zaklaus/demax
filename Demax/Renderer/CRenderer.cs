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
		CCore core;
		Camera mainCamera;
		DownFall df = new DownFall();

		bool updated, onDemand, loaded = false;
		float time = 0f;
		public float deltaTime, lastTime = 0.0f;

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

		Dictionary<string, CShaderProgram> shaders = new Dictionary<string, CShaderProgram>();
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
			CCore.GetCore ().MainCamera.Move (0, 10f, 0);

			shaders.Add("default", new CShaderProgram("vs.glsl", "fs.glsl", true));
			shaders.Add("textured", new CShaderProgram("vs_tex.glsl", "fs_tex.glsl", true));
			shaders.Add("lighted", new CShaderProgram("vs_light.glsl", "fs_light.glsl", true));

			activeShader = "lighted";

			var cubes = core.EntityManager.CreateEntity ("Cubes");
			cubes.Transform.Position = new Vector3 (0, -6, -20);
			cubes.AddScript ("cubes.py");

//			loadImage (Path.Combine ("Textures", "img01.jpg"));
//			loadImage (Path.Combine ("Textures", "img02.jpg"));
//			loadImage (Path.Combine ("Textures", "img03.jpg"));
//
//			Random rand = new Random ();
//
//			for (int i = 0; i < 30; i++) {
//				TexturedCube fall = new TexturedCube(cubes);
//				fall.Position = new Vector3 ((float)rand.NextDouble()*50f - 25f, (float)rand.NextDouble()*80f + 5f, (float)rand.NextDouble()*50f - 25f);
//				fall.TextureID = (rand.Next(0,100) > 50) ? textures ["img02.jpg"] : textures ["img01.jpg"];
//				fall.Scale = new Vector3 (5f, 5f, 5f);
//				fall.AddRigidbody ();
//				cubes.AddModel (fall);
//			}
//
//			TexturedCube floor = new TexturedCube(cubes);
//			floor.Position = new Vector3 (0, 0, 0);
//			floor.Scale = new Vector3 (100, 1, 100);
//			floor.TextureID = textures ["img03.jpg"];
//			floor.isStatic = true;
//			floor.AddRigidbody ();
//			floor.body.AffectedByGravity = false;
//			floor.body.IsStatic = true;
//			cubes.AddModel (floor);
		}


		void updateTest()
		{

			List<Vector3> verts = new List<Vector3>();
			List<int> inds = new List<int>();
			List<Vector3> colors = new List<Vector3>();
			List<Vector3> norms = new List<Vector3>();
			List<Vector2> texcoords = new List<Vector2>();


			int vertcount = 0;

			foreach(CEntity en in core.EntityManager.GetEntities())
			{
				foreach (Volume v in en.GetModels())
				{
					verts.AddRange(v.GetVerts().ToList());
					inds.AddRange(v.GetIndices(vertcount).ToList());
					colors.AddRange(v.GetColorData().ToList());
					texcoords.AddRange(v.GetTextureCoords());
					norms.AddRange (v.GetNormals ());
					vertcount += v.VertCount;
				}
			}

			vertdata = verts.ToArray();
			indicedata = inds.ToArray();
			normdata = norms.ToArray ();
			coldata = colors.ToArray();
			texcoorddata = texcoords.ToArray();

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

			GL.ClearColor (Color.CornflowerBlue);
			GL.PointSize (5f);
		}

		/// <summary>
		/// Flush this instance.
		/// </summary>
		public void Flush()
		{
			updateTest ();
		}

		/// <summary>
		/// Initializes game renderer. (OpenGL)
		/// </summary>
		public CRenderer ()
		{
			core = CCore.GetCore ();
			gameRenderer = core.GameRenderer;
			mainCamera = core.MainCamera;


			gameRenderer.Load += OnLoad;
			gameRenderer.Resize += OnResize;
			gameRenderer.UpdateFrame += OnUpdateFrame;
			gameRenderer.RenderFrame += OnRenderFrame;
			gameRenderer.FocusedChanged += (object sender, EventArgs e) => { core.InputManager.Focused = !core.InputManager.Focused; };

			gameRenderer.KeyDown += core.InputManager.OnKeyDown;
			gameRenderer.KeyUp += core.InputManager.OnKeyUp;
			gameRenderer.KeyPress += core.InputManager.OnKeyPress;
		}
		static class RequiredFeatures
		{
			public static readonly Version FramebufferObject = new Version(3, 0);
		}
		void OnLoad(object sender, EventArgs e)
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

			updateTest ();

			loaded = true;
		}

		void OnResize(object sender, EventArgs e)
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

		void OnUpdateFrame(object sender, FrameEventArgs e)
		{

			//df.DoFall ();

			core.world.Step (1.0f / 100.0f, true);

			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume v in en.GetModels()) {
					if (v.body != null && !v.body.IsStatic) {
						v.Position = new Vector3 (v.body.Position.X, v.body.Position.Y, v.body.Position.Z);
						v.Rotation = v.JMatrixToMatrix (v.body.Orientation);
					}
				}
			}

			core.MainCamera.UpdateCamera ();

			core.InputManager.TickCursor ();

			if(onDemand)
				updateTest ();


			// Modelview Buffer
			Matrix4 viewmodel = mainCamera.GetViewMatrix();
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView ((float)Math.PI / 4, gameRenderer.Width / (float)gameRenderer.Height, 0.1f, 500.0f);


			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume v in en.GetModels()) {
					v.CalculateModelMatrix ();
					v.ViewProjectionMatrix = projection;
					v.ViewModelMatrix = viewmodel;
					v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewModelMatrix * v.ViewProjectionMatrix;
				}
			}

			updated = true;
			time += (float)e.Time;

			CScript.BroadcastEvent ("OnUpdate");
		}

		void OnRenderFrame(object sender, FrameEventArgs e)
		{
			if (updated && loaded) {
				updated = false;
			} else {
				return;
			}

			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//Matrix4 modelview = Matrix4.LookAt (Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);


			GL.UseProgram (shaders [activeShader].ProgramID);

			shaders[activeShader].EnableVertexAttribArrays();

			int indiceat = 0;

			foreach (CEntity entity in core.EntityManager.GetEntities())
			{
				foreach (Volume v in entity.GetModels())
				{
					GL.BindTexture(TextureTarget.Texture2D, v.TextureID);
					GL.UniformMatrix4(shaders[activeShader].GetUniform("M"), false, ref v.ModelMatrix);
					GL.UniformMatrix4(shaders[activeShader].GetUniform("V"), false, ref v.ViewModelMatrix);
					GL.UniformMatrix4(shaders[activeShader].GetUniform("P"), false, ref v.ViewProjectionMatrix);
					GL.UniformMatrix4(shaders[activeShader].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

					if (shaders[activeShader].GetAttribute("maintexture") != -1)
					{
						GL.Uniform1(shaders[activeShader].GetAttribute("maintexture"), v.TextureID);
					}

					GL.DrawElements(BeginMode.Triangles, v.IndiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
					indiceat += v.IndiceCount;
				}
			}

			shaders[activeShader].DisableVertexAttribArrays();

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

