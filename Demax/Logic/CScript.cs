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


		static private ScriptRuntime m_runtime = Python.CreateRuntime();
		static private ScriptEngine m_engine = m_runtime.GetEngine ("Python");
		private ScriptScope m_scope = null;
		bool invalid = false;
		static Dictionary<string, int> keys;

		static void LoadKeys(CScript c)
		{
			if (keys == null) {
				keys = new Dictionary<string, int> ();
				Key[] ekeys = (Key[])Enum.GetValues (typeof(Key));
				int _i = 0;
				Key lastkey = ekeys[14];
				foreach (Key key in ekeys) {
					if (key == lastkey)
						continue;
					lastkey = key;
					keys.Add ("Key_" + key.ToString (), (int)key);
					_i++;
				}
			}
			foreach (var k in keys) {
				c.m_scope.SetVariable (k.Key, (int)k.Value);
			}
		}

        string filename;
        CEntity e;

		/// <summary>
		/// Initializes a new instance of the <see cref="Demax.CScript"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="e">E.</param>
		public CScript (string filename, CEntity e)
		{
            this.filename = filename;
            this.e = e;
		}

        public void Init()
        {
            var time = DateTime.Now;

            // Checks if file exists. Halts on missing file.
            //
            if (!File.Exists(filename))
            {
                return;
            }
            entity = e;


            // Creates our scope for script.
            //
            m_scope = m_engine.CreateScope();
            m_scope.ImportModule("clr");
            
            var game = CCore.GetCore();

            // Sets some essential variables for our script.
            //
            m_scope.SetVariable("cam", e.game.MainCamera);
            m_scope.SetVariable("game", e.game);
            m_scope.SetVariable("gui", e.game.Renderer.guiManager);
            m_scope.SetVariable("cell", e.game.Renderer);
            m_scope.SetVariable("em", e.game.EntityManager); 
            m_scope.SetVariable("me", entity);
            m_scope.SetVariable("scr", this);


            m_scope.SetVariable("input", e.game.InputManager);

            // Build or use cached key symbols.
            // REMOVED: Use LoadKeys(this) to load legacy keys.
            //LoadKeys (this);

            // Imports several libraries for our script.
            //
            m_engine.Execute("import clr", m_scope);
            m_engine.Execute("clr.AddReference(\"Demax\")", m_scope);
            m_engine.Execute("clr.AddReference(\"OpenTK\")", m_scope);
            m_engine.Execute("clr.AddReference(\"Jitter\")", m_scope);
            m_engine.Execute("clr.AddReference(\"System\")", m_scope);
            m_engine.Execute("clr.AddReference(\"System.IO\")", m_scope);
            m_engine.Execute("from Demax import *", m_scope);
            m_engine.Execute("from System import *", m_scope);
            m_engine.Execute("from System.IO import *", m_scope);
            m_engine.Execute("from OpenTK import *", m_scope);
            m_engine.Execute("from Jitter import *", m_scope);
            src = m_engine.CreateScriptSourceFromFile(filename);

            // Executes our script and calls our OnStart event.
            try
            {
                src.Execute(m_scope);
                var OnStart = m_scope.GetVariable("OnStart");
                try
                {
                    var result = OnStart();
                }
                catch (Exception ex)
                {
                    CLog.WriteLine("Logic Error: " + ex.ToString());
                    invalid = true;
                }
            }
            catch (Exception ex)
            {
                CLog.WriteLine("Logic Error: " + ex.ToString());
                return;
            }
			

			
        }

		/// <summary>
		/// Broadcasts the event.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void BroadcastEvent(string name, object arg=null)
		{
			foreach (CEntity en in CCore.GetCore().EntityManager.GetEntities()) {
				foreach (CScript cs in en.GetScripts()) {
					cs.CallEvent (name,arg);
				}
			}
		}

		string lasterror = "";

		/// <summary>
		/// Calls the event.
		/// </summary>
		/// <param name="name">Name.</param>
		public void CallEvent(string name, object arg=null)
		{
			if(!invalid)
				try{
				    var callable = m_scope.GetVariable (name);

                    if (arg == null)
                        callable();
                    else
                        callable(arg);
				}
				catch(Exception ex){
					if (ex.ToString ().Contains ("ScopeStorage"))
						return;
				
					if(lasterror!=ex.ToString())
						CLog.WriteLine ("Logic Error: " + ex.ToString ());

					
					invalid = true;
					lasterror = ex.ToString ();
				}
		}
	}
}

