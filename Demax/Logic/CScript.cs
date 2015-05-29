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


		private ScriptRuntime m_runtime = Python.CreateRuntime();
		private ScriptEngine m_engine = null;
		private ScriptScope m_scope = null;
		bool invalid = false;

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

			m_engine = m_runtime.GetEngine ("Python");

			m_scope = m_engine.CreateScope();
			m_scope.ImportModule ("clr");
			m_scope.SetVariable ("me", entity);
			var game = CCore.GetCore ();

			m_scope.SetVariable ("cam", e.game.MainCamera);
			m_scope.SetVariable ("game", e.game);
			m_scope.SetVariable ("cell", e.game.Renderer);

			m_scope.SetVariable ("input", e.game.InputManager);

			Key[] ekeys = (Key[])Enum.GetValues (typeof(Key)); int _i = 0;
			Console.WriteLine ("FK: " + ekeys [14].ToString());
			foreach(Key key in ekeys)
			{
				m_scope.SetVariable ("Key_"+key.ToString(), (int)key);
				_i++;
			}
			m_engine.Execute ("import clr", m_scope);
			m_engine.Execute ("clr.AddReference(\"Demax\")", m_scope);
			m_engine.Execute ("clr.AddReference(\"OpenTK\")", m_scope);
			m_engine.Execute ("clr.AddReference(\"System\")", m_scope);
			m_engine.Execute ("clr.AddReference(\"System.IO\")", m_scope);
			m_engine.Execute ("from Demax import *", m_scope);
			m_engine.Execute ("from System import *", m_scope);
			m_engine.Execute ("from System.IO import *", m_scope);
			m_engine.Execute ("from OpenTK import *", m_scope);
			m_engine.Execute ("print(\"Script()\")", m_scope);
			src = m_engine.CreateScriptSourceFromFile (filename);
			try{
				
			src.Execute(m_scope);
			
			}
			catch(Exception ex){
				Console.WriteLine ("Logic Error: " + ex.ToString ());
				return;
			}
			var OnStart = m_scope.GetVariable ("OnStart");

			try{
				var result = OnStart ();
			}
			catch(Exception ex) {
				Console.WriteLine ("Logic Error: " + ex.ToString ());
				invalid = true;
			}
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

		string lasterror = "";

		/// <summary>
		/// Calls the event.
		/// </summary>
		/// <param name="name">Name.</param>
		public void CallEvent(string name, string args="")
		{
			if(!invalid)
				try{
				var callable = m_scope.GetVariable (name);
				callable ();
				}
				catch(Exception ex){
					if (ex.ToString ().Contains ("ScopeStorage"))
						return;
				
					if(lasterror!=ex.ToString())
						Console.WriteLine ("Logic Error: " + ex.ToString ());

					
					invalid = true;
					lasterror = ex.ToString ();
				}
		}
	}
}

