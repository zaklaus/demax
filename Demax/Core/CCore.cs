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
using OpenTK.Platform;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter;
using Jitter.Collision;

namespace Demax
{
	/// <summary>
	/// Main class handling all aspects of engine.
	/// </summary>
	public class CCore
	{
		static CCore instance;

		/// <summary>
		/// Gets the core.
		/// </summary>
		/// <returns>The core.</returns>
		public static CCore GetCore()
		{
			return instance;
		}

		/// <summary>
		/// Determines whether this instance is alive.
		/// </summary>
		/// <returns><c>true</c> if this instance is alive; otherwise, <c>false</c>.</returns>
		public void IsAlive()
		{
			Console.WriteLine ("Core is alive...");
		}

		CRenderer renderer;
		CInputManager inputManager;
		CEntityManager entityManager;
		Camera mainCamera = new Camera();
		GameWindow gameRenderer;
        public GLControl inlineRenderer;
        public string type = "window";

		/// <summary>
		/// GameWindow class.
		/// </summary>
		/// <value>The game renderer.</value>
		public GameWindow GameRenderer
		{
			get 
			{
				return gameRenderer;
			}
		}

		/// <summary>
		/// Gets the main camera.
		/// </summary>
		/// <value>The main camera.</value>
		public Camera MainCamera
		{
			get {
				return mainCamera;
			}
			set {
				mainCamera = value;
			}
		}

		/// <summary>
		/// Gets the renderer.
		/// </summary>
		/// <value>The renderer.</value>
		public CRenderer Renderer
		{
			get
			{
				return renderer;
			}
		}

		public CRenderer GetCell()
		{
			return renderer;
		}

		/// <summary>
		/// Gets the input manager.
		/// </summary>
		/// <value>The input manager.</value>
		public CInputManager InputManager
		{
			get
			{
				return inputManager;
			}
		}

		/// <summary>
		/// Gets the entity manager.
		/// </summary>
		/// <value>The entity manager.</value>
		public CEntityManager EntityManager
		{
			get {
				return entityManager;
			}
		}

		/// <summary>
		/// Quit this instance.
		/// </summary>
		public void Quit()
		{
			gameRenderer.Close ();

		}
		public CollisionSystem colsys;
		public World world;



		/// <summary>
		/// Initializes game engine.
		/// </summary>
		public CCore()
		{
			Console.WriteLine ("DeMax Engine");
			instance = this;
			colsys = new CollisionSystemSAP ();
            colsys.UseTriangleMeshNormal = false;
            world = new World(colsys);
            renderer = new CRenderer();
            
			entityManager = new CEntityManager ();
			inputManager = new CInputManager ();
		}

		/// <summary>
		/// Run this instance.
		/// </summary>
		public void RunWindowed(int width, int height, GraphicsMode mode, string title, GameWindowFlags flags)
		{
			gameRenderer = new GameWindow(width, height, mode, title, flags);
            type = "window";
		}

        public void RunInline(GLControl gl)
        {
            inlineRenderer = gl;
            type = "inline";
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
		public void Start()
		{
            renderer.Init();
			gameRenderer.Run (60.0f);
		}
	}
}

