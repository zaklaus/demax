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
        int framebuffer = 0;

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
        public Dictionary<string, ObjVolume> volumes = new Dictionary<string, ObjVolume>();
        CShaderProgram quadProgram;
		public string activeShader = "default";
        public string rttShader = "rtt";

		Vector3[] vertdata, coldata, normdata;
		Vector2[] texcoorddata;
		int[] indicedata;
		int ibo_elements, fbTexture, dbTexture, depthB, quadB, rtexID, rtimeID;
        Bitmap fboOutput;
		//Matrix4[] mviewdata;
        DrawBuffersEnum[] dbe = {DrawBuffersEnum.ColorAttachment0};
		/// <summary>
		/// Inits the test.
		/// </summary>
		void initTest()
		{
			GL.GenBuffers(1, out ibo_elements);
            GL.GenFramebuffers(1, out framebuffer);
            GL.GenBuffers(1, out quadB);
            GL.GenRenderbuffers(1, out depthB);
            GL.GenTextures(1, out fbTexture);
            GL.GenTextures(1, out dbTexture);

            quadProgram = new CShaderProgram(DefaultShader.passthrough, DefaultShader.f_passthrough, false, false);
            rttShader = "rtt";
            rtexID = quadProgram.GetUniform("renderedTexture");
            rtimeID = quadProgram.GetUniform("time");

            shaders.Add("rtt", quadProgram);

            //updateFB(gameRenderer.ClientSize.Width, gameRenderer.ClientSize.Height);
		}

        void updateFB(int w, int h)
        {
            GL.BindTexture(TextureTarget.Texture2D, fbTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, w, h, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindTexture(TextureTarget.Texture2D, dbTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, w, h, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.BindTexture(TextureTarget.Texture2D, 0);


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, fbTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, dbTexture, 0);
            GL.DrawBuffers(1, dbe);
            #region unused

            //GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, 0, 0);

            //GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthB);
            //GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, w, h);
            //GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, 0);

            //GL.BindTexture(TextureTarget.Texture2D, dbTexture);

            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent16, w, h, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
            ////GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, fbTexture, 0);
            //GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, dbTexture, 0);
            
            #endregion
            switch(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
            {
                case FramebufferErrorCode.FramebufferIncompleteAttachment:
                    CLog.WriteLine("OpenGL FBO missing attachment");
                    break;
                case FramebufferErrorCode.FramebufferUnsupported:
                    CLog.WriteLine("OpenGL FBO unsupported");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteReadBuffer:
                    CLog.WriteLine("OpenGL FBO incomplete read buffer");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
                    CLog.WriteLine("OpenGL FBO incomplete draw buffer");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    CLog.WriteLine("OpenGL FBO wrong format ext");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    CLog.WriteLine("OpenGL FBO wrong dimension ext");
                    break;
                case FramebufferErrorCode.FramebufferComplete:
                    CLog.WriteLine("Opengl FBO OK");
                    break;
                default:
                    CLog.WriteLine("Opengl FBO FAIL");
                    break;
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            //
        }

        /// <summary>
        /// Updates our matrices for every existing volume.
        /// </summary>
        /// <param name="v"></param>
		void updateTest(Volume v)
		{
            // Halts if volume is invisible.
            if (!v.isVisible)
                return;

			try
            {
                // Calculates our matrices.
                //
			    v.CalculateModelMatrix ();
			    v.ViewProjectionMatrix = projection;
			    v.ViewModelMatrix = viewmodel;
			    v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewModelMatrix * v.ViewProjectionMatrix;
			    
                /* Populates our vertices, normals and uv coordinates */
                #region prebuffer
                int vertcount = 0;
                List<Vector3> verts = new List<Vector3>();
                List<int> inds = new List<int>();
                List<Vector3> colors = new List<Vector3>();
                List<Vector3> norms = new List<Vector3>();
                List<Vector2> texcoords = new List<Vector2>();

                verts.AddRange(v.GetVerts().ToList());
                inds.AddRange(v.GetIndices().ToList());
                colors.AddRange(v.GetColorData().ToList());
                texcoords.AddRange(v.GetTextureCoords().ToList());
                norms.AddRange(v.GetNormals().ToList());
                vertcount += v.VertCount;

                vertdata = verts.ToArray();
                indicedata = inds.ToArray();
                normdata = norms.ToArray();
                coldata = colors.ToArray();
                texcoorddata = texcoords.ToArray(); 
                #endregion

                /* --- */


                /* Sends our data to buffers */
                #region buffers
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
                #endregion buffers
                /* --- */
		    }
		    catch(Exception ex){
			    CLog.WriteLine (ex.ToString ());
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

        /// <summary>
        /// Registers our events.
        /// </summary>
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

        /// <summary>
        /// Changes focus for our controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gameRenderer_FocusedChanged(object sender, EventArgs e)
        {
            core.InputManager.Focused = !core.InputManager.Focused;
        }

		static class RequiredFeatures
		{
			public static readonly Version FramebufferObject = new Version(3, 0);
		}

        /// <summary>
        /// Handles initialization of our renderer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // Is this compatible GPU?
            //
			if (extensions.ContainsKey("ARB_fragment_program"))
			{
				CLog.WriteLine("Sorry, Your GPU is not supported!");
				core.GameRenderer.Close ();
				Console.ReadKey();
				return;
			}

            // Further initialization
            //
			initTest ();
            //updateFB(gameRenderer.ClientRectangle.Width, gameRenderer.ClientRectangle.Height);
			loaded = true;
		}

        /// <summary>
        /// Updates our viewport.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnResize(object sender, EventArgs e)
		{
			GL.Viewport (0, 0, gameRenderer.ClientRectangle.Width, gameRenderer.ClientRectangle.Height);
            fboOutput = new Bitmap(gameRenderer.ClientSize.Width, gameRenderer.ClientSize.Height);
            updateFB(gameRenderer.ClientRectangle.Width, gameRenderer.ClientRectangle.Height);
		}

		/// <summary>
		/// Sets the on demand alloc.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		public void SetOnDemandAlloc(bool state)
		{
			this.onDemand = state;
		}

        /// <summary>
        /// Handles physics and logical operations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnUpdateFrame(object sender, FrameEventArgs e)
		{
            // Let's step in our world, step to the future!
            //
			core.world.Step (1.0f / 100.0f, true);

            #region PreLogic

            foreach (CEntity en in core.EntityManager.GetEntities())
            {
                foreach (Volume v in en.GetModels())
                {
                    List<RigidBody> b = new List<RigidBody>(v.body);
                    foreach (var r in b)
                        if (v.body != null && !r.IsStatic)
                        {
                            v.Position = new Vector3(r.Position.X, r.Position.Y, r.Position.Z);
                            v.Rotation = v.JMatrixToMatrix(r.Orientation);

                            if (v.Position.Y < zeroKill)
                            {
                                if (v.killOnZero)
                                {
                                    v.Destroy();
                                }
                                else
                                {
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
            foreach (CEntity en in core.EntityManager.GetEntities())
            {
                List<Volume> l = en.GetModels();
                foreach (Volume v in l.ToList())
                {
                    if (v.markedForDestroy)
                    {
                        en.Models.Remove(v);
                        shouldFlush = true;
                    }
                }
            }
            
            #endregion

            // Updates our matrices.
            //
			viewmodel = mainCamera.GetViewMatrix();
			projection = Matrix4.CreatePerspectiveFieldOfView ((float)Math.PI/180 * CCore.GetCore().MainCamera.FieldOfView, gameRenderer.Width / (float)gameRenderer.Height, CCore.GetCore().MainCamera.nearPlane, CCore.GetCore().MainCamera.farPlane);

            // Updates volume matrices. Yeah, again...
            //
			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume v in en.GetModels()) {
					v.CalculateModelMatrix ();
					v.ViewProjectionMatrix = projection;
					v.ViewModelMatrix = viewmodel;
					v.ModelViewProjectionMatrix = v.ModelMatrix * v.ViewModelMatrix * v.ViewProjectionMatrix;

                    foreach(var anim in v.anims)
                    {
                        foreach(Volume f in anim.Value)
                        { 
                            f.CalculateModelMatrix();
                            f.ViewProjectionMatrix = projection;
                            f.ViewModelMatrix = viewmodel;
                            f.ModelViewProjectionMatrix = f.ModelMatrix * f.ViewModelMatrix * f.ViewProjectionMatrix;
                        }
                    }
                }
			}
			
			GL.ClearColor (Color.CornflowerBlue);
			GL.PointSize (5f);

			updated = true;
			time += (float)e.Time;

			CScript.BroadcastEvent ("OnUpdate");
		}

        /// <summary>
        /// Handles our glory rendering stuff.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnRenderFrame(object sender, FrameEventArgs e)
		{
            // Is our game already loaded and updated?
            //
			if (updated && loaded) {
				updated = false;
			} else {
				return;
			}

            float[] quadData = new float[] {
                -1.0f, -1.0f, 0.0f,
                 1.0f, -1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,
                 1.0f, -1.0f, 0.0f,
                 1.0f,  1.0f, 0.0f,
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, quadB);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(quadData.Length * sizeof(float)), quadData, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.Green);
            GL.DrawBuffers(1, dbe);
            GL.Viewport(0, 0, gameRenderer.ClientSize.Width, gameRenderer.ClientSize.Height);
			int indiceat = 0;

            #region RenderLoop

            foreach (CEntity entity in core.EntityManager.GetEntities())
            {
                foreach (Volume v in entity.GetModels())
                {
                    if (v.anims.Count == 0 && v.cname == "")
                        indiceat = render(v, indiceat);
                    else
                    {
                        v.TickAnim();

                        indiceat = render(v.anims[v.cname][v.cframe], indiceat);
                    }
                }
            }
            
            #endregion

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, gameRenderer.ClientSize.Width, gameRenderer.ClientSize.Height);
            GL.UseProgram(shaders[rttShader].ProgramID);
            //GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, fbTexture);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.Uniform1(rtexID, 0);
            GL.Uniform1(rtimeID, (float)(DateTime.Now.Millisecond * 10.0f));
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadB);
            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                0,
                0
                );
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.DisableVertexAttribArray(0);
            

            //guiManager.Draw();

			gameRenderer.SwapBuffers();



			deltaTime = (float)e.Time;

            CScript.BroadcastEvent("OnRender");
		}

        int render(Volume v, int indiceat)
        {
            int i = 0;

            Vector3 uniformPosition = (v.me.RecursiveTransform().Position + v.Position);
            Volume ma = v;
            float oDist = Extensions.GetDistance3D(uniformPosition, mainCamera.Position);
            foreach (var lod in v.LOD)
            {
                if (oDist > lod.Key)
                {
                    if (lod.Value != null)
                        ma = lod.Value;
                }
            }

            foreach (Volume m in v.meshes)
            {
                if (!m.isVisible)
                {
                    indiceat += m.IndiceCount;
                    continue;
                }

                // HEAVY: per-vertex check
                float farDist = 0.0f;
                float nearDist = 0xDEADBEEF;
                Vector3 farPosition = Vector3.Zero;
                Vector3 nearPosition = Vector3.Zero;
                uniformPosition = (m.me.RecursiveTransform().Position + v.Position + m.Position);

                foreach (var vert in m.vertices)
                {
                    float dist = Extensions.GetDistance3D(uniformPosition + vert, mainCamera.Position);
                    if (dist > farDist)
                    {
                        farDist = dist;
                        farPosition = uniformPosition + vert;
                    }
                    if (dist < nearDist)
                    {
                        nearDist = dist;
                        nearPosition = uniformPosition + vert;
                    }
                }
                m.CalculateBoundingBox();
                if (!Extensions.IsInside3D(uniformPosition + m.BBoxMin, uniformPosition + m.BBoxMax, mainCamera.Position))
                {
                    Vector3 target = (farPosition - mainCamera.Position).Normalized();
                    Vector3 dir = mainCamera.LookAt;
                    float place = 0.0f;
                    Vector3.Dot(ref target, ref dir, out place);
                    
                    if (farDist > 5.0f && place <= 0)
                    {
                        continue;
                    }
                    else if (farDist > mainCamera.farPlane && place > 0)
                    {
                        continue;
                    }
                }

                GL.UseProgram(shaders[CShaderProgram.LoadShaderPointer(m.Shader)].ProgramID);
                shaders[CShaderProgram.LoadShaderPointer(m.Shader)].EnableVertexAttribArrays();

                try {
                    updateTest(ma.meshes[i]);
                }
                catch
                {
                    updateTest(m);
                }
                int tex = 0;

                if (v.materials.Count == 0)
                    tex = v.TextureID;
                else
                    foreach (var mat in v.materials)
                        if (mat.name == m.matuse[0])
                        {
                            tex = mat.TextureID;
                        }

                // Sends our uniform matrices and binds our texture.
                //
                GL.BindTexture(TextureTarget.Texture2D, tex);
                GL.UniformMatrix4(shaders[activeShader].GetUniform("M"), false, ref v.ModelMatrix);
                GL.UniformMatrix4(shaders[activeShader].GetUniform("V"), false, ref v.ViewModelMatrix);
                GL.UniformMatrix4(shaders[activeShader].GetUniform("P"), false, ref v.ViewProjectionMatrix);
                GL.UniformMatrix4(shaders[activeShader].GetUniform("MVP"), false, ref v.ModelViewProjectionMatrix);

                // Does the shader support textures? Send texture if yes.
                //
                if (shaders[activeShader].GetAttribute("maintexture") != -1)
                {
                    GL.Uniform1(shaders[activeShader].GetAttribute("maintexture"), tex);
                }
                try
                {
                    GL.DrawElements(PrimitiveType.Triangles, m.IndiceCount, DrawElementsType.UnsignedInt, 0);
                }
                catch (Exception ex)
                {
                    CLog.WriteLine(ex.ToString());
                }
                indiceat += m.IndiceCount;
                i++;

                shaders[CShaderProgram.LoadShaderPointer(m.Shader)].DisableVertexAttribArrays();

            }
            return indiceat;
        }
    }
}

