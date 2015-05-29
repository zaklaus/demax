using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	/// <summary>
	/// Shading.
	/// </summary>
	public class CShading
	{
		int pgmID, vsID, fsID;

		/// <summary>
		/// Gets the program ID.
		/// </summary>
		/// <value>The program ID.</value>
		public int ProgramID
		{
			get {
				return pgmID;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Demax.CShading"/> class.
		/// </summary>
		public CShading ()
		{
			pgmID = GL.CreateProgram ();
			LoadShader ("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
			LoadShader ("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
			Console.WriteLine (GL.GetProgramInfoLog (pgmID));
			GL.LinkProgram (pgmID);
			GL.UseProgram (pgmID);
		}

		void LoadShader(string filename, ShaderType type, int program, out int address)
		{
			address = GL.CreateShader (type);

			if (!File.Exists (filename)) {
				Console.WriteLine ("Shader Error: shader not found; " + filename);
			}

			using (StreamReader sr = new StreamReader (filename)) {
				GL.ShaderSource (address, sr.ReadToEnd ());
			}
			GL.CompileShader (address);
			GL.AttachShader (program, address);
			Console.WriteLine (GL.GetShaderInfoLog (address) + " PID: "+pgmID);

		}
	}
}

