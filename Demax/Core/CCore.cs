using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
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
		Camera mainCamera;
		GameWindow gameRenderer;

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
			world = new World (colsys);
			gameRenderer = new GameWindow(800,600,new GraphicsMode(32,24,8,4),"Demax",GameWindowFlags.Default);
			mainCamera = new Camera ();
			entityManager = new CEntityManager ();
			inputManager = new CInputManager ();
			renderer = new CRenderer ();

			gameRenderer.Run (60.0f);
		}
	}
}

