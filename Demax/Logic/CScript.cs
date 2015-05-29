using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	public class CScript
	{
		CEntity entity;
		ScriptSource src;

		private ScriptEngine m_engine = Python.CreateEngine();
		private ScriptScope m_scope = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Demax.CScript"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="e">E.</param>
		public CScript (string filename, CEntity e)
		{
			if (!File.Exists (filename)) {
				return;
			}
			entity = e;

			m_scope = m_engine.CreateScope();
			m_scope.SetVariable ("me", entity);
			var game = CCore.GetCore ();
			game.IsAlive ();
			if (CCore.GetCore () == null || game == null)
				Console.WriteLine ("weird error.");


			m_scope.SetVariable ("cam", e.game.MainCamera);
			m_scope.SetVariable ("game", e.game);
			m_scope.SetVariable ("input", e.game.InputManager);

			Key[] ekeys = (Key[])Enum.GetValues (typeof(Key)); int _i = 0;
			Console.WriteLine ("FK: " + ekeys [14].ToString());
			foreach(Key key in ekeys)
			{
				m_scope.SetVariable ("Key_"+key.ToString(), (int)key);
				_i++;
			}

			src = m_engine.CreateScriptSourceFromFile (filename);
			try{
			src.Execute(m_scope);
			}
			catch(Exception ex){
				Console.WriteLine ("Logic Error: " + ex.ToString ());
				return;
			}
			var OnStart = m_scope.GetVariable ("OnStart");
			var result = OnStart ();
		}

		/// <summary>
		/// Broadcasts the event.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void BroadcastEvent(string name)
		{
			foreach (CEntity en in CCore.GetCore().EntityManager.GetEntities()) {
				foreach (CScript cs in en.GetScripts()) {
					cs.CallEvent (name);
				}
			}
		}

		/// <summary>
		/// Calls the event.
		/// </summary>
		/// <param name="name">Name.</param>
		public void CallEvent(string name)
		{
			try{
			var callable = m_scope.GetVariable (name);
			callable ();
			}
			catch{
			}
		}
	}
}

