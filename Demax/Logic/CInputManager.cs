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
using OpenTK;
using OpenTK.Input;

namespace Demax
{
	/// <summary>
	/// Handles game input.
	/// </summary>
	public class CInputManager
	{
		CCore core;
		/// <summary>
		/// The focused.
		/// </summary>
		public bool Focused = true;
		Dictionary<Key,bool> keys;

		Vector2 lastMousePos = new Vector2();

		/// <summary>
		/// The axis.
		/// </summary>
		public Vector2 Axis = new Vector2();

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		public Dictionary<Key,bool> Keys
		{
			get {
				return keys;
			}
		}

		/// <summary>
		/// Gets the state.
		/// </summary>
		/// <returns><c>true</c>, if state was gotten, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		public bool GetState(int id)
		{
			try{
				return keys [(Key)id];
			}
			catch{
				keys.Add ((Key)id, false);
				return false;
			}
		}

		/// <summary>
		/// Initializes game input for GameWindow.
		/// </summary>
		public CInputManager ()
		{
			core = CCore.GetCore ();

			keys = new Dictionary<Key,bool> ();

			/*Key latestKey = Key.Z;
			var ekeys = Enum.GetValues (typeof(Key));
			for (int i = 0; i < ekeys.Length; i++) {
				if (latestKey != (Key)ekeys.GetValue (i)) {
					latestKey = (Key)ekeys.GetValue (i);
					keys.Add ((Key)ekeys.GetValue (i), false);
				}
			}*/

		}

		/// <summary>
		/// Resets the cursor.
		/// </summary>
		public void ResetCursor()
		{
			OpenTK.Input.Mouse.SetPosition(core.GameRenderer.Bounds.Left + core.GameRenderer.Bounds.Width / 2, core.GameRenderer.Bounds.Top + core.GameRenderer.Bounds.Height / 2);
			lastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
		}

		/// <summary>
		/// Ticks the cursor.
		/// </summary>
		public void TickCursor()
		{
			if (Focused)
			{
				Vector2 delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
				Axis = delta;

				//CScript.BroadcastEvent ("OnMouse");
				//core.MainCamera.AddRotation(delta.X, delta.Y);
				ResetCursor();
			}
		}

		/// <summary>
		/// Raises the key down event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			//CLog.WriteLine ("Key Down: " + e.Key.ToString ());
			try{
				keys [e.Key] = true;
			}
			catch{
				keys.Add (e.Key, true);
			}

            CScript.BroadcastEvent("OnKeyDown", e.Key.ToString());
		}

		/// <summary>
		/// Raises the key up event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnKeyUp(object sender, KeyboardKeyEventArgs e)
		{
			//CLog.WriteLine ("Key Up: " + e.Key.ToString ());
			try{
				keys [e.Key] = false;
			}
			catch{
				keys.Add (e.Key, false);
			}

            CScript.BroadcastEvent("OnKeyUp", e.Key.ToString());
		}

		/// <summary>
		/// Raises the key press event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnKeyPress(object sender, KeyPressEventArgs e)
		{
            CScript.BroadcastEvent("OnKeyPress", e.KeyChar.ToString());
		}
	}
}

