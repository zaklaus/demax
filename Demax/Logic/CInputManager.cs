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
			return keys [(Key)id];
		}

		/// <summary>
		/// Initializes game input for GameWindow.
		/// </summary>
		public CInputManager ()
		{
			core = CCore.GetCore ();

			keys = new Dictionary<Key,bool> ();

			Key latestKey = Key.Z;
			var ekeys = Enum.GetValues (typeof(Key));
			for (int i = 0; i < ekeys.Length; i++) {
				if (latestKey != (Key)ekeys.GetValue (i)) {
					latestKey = (Key)ekeys.GetValue (i);
					keys.Add ((Key)ekeys.GetValue (i), false);
				}
			}

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
			//Console.WriteLine ("Key Down: " + e.Key.ToString ());
			keys [e.Key] = true;
		}

		/// <summary>
		/// Raises the key up event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnKeyUp(object sender, KeyboardKeyEventArgs e)
		{
			//Console.WriteLine ("Key Up: " + e.Key.ToString ());
			keys [e.Key] = false;
		}

		/// <summary>
		/// Raises the key press event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void OnKeyPress(object sender, KeyPressEventArgs e)
		{

		}
	}
}

