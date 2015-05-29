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
//using System;
//using System.Drawing;
//using System.IO;
//using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;
//using OpenTK.Input;
//
//namespace Demax
//{
//	/// <summary>
//	/// Shading.
//	/// </summary>
//	public class CShading
//	{
//		int pgmID, vsID, fsID;
//
//		/// <summary>
//		/// Gets the program ID.
//		/// </summary>
//		/// <value>The program ID.</value>
//		public int ProgramID
//		{
//			get {
//				return pgmID;
//			}
//		}
//		/// <summary>
//		/// Initializes a new instance of the <see cref="Demax.CShading"/> class.
//		/// </summary>
//		public CShading ()
//		{
//			pgmID = GL.CreateProgram ();
//			LoadShader ("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
//			LoadShader ("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);
//			Console.WriteLine (GL.GetProgramInfoLog (pgmID));
//			GL.LinkProgram (pgmID);
//			GL.UseProgram (pgmID);
//		}
//
//		void LoadShader(string filename, ShaderType type, int program, out int address)
//		{
//			address = GL.CreateShader (type);
//
//			if (!File.Exists (filename)) {
//				Console.WriteLine ("Shader Error: shader not found; " + filename);
//			}
//
//			using (StreamReader sr = new StreamReader (filename)) {
//				GL.ShaderSource (address, sr.ReadToEnd ());
//			}
//			GL.CompileShader (address);
//			GL.AttachShader (program, address);
//			Console.WriteLine (GL.GetShaderInfoLog (address) + " PID: "+pgmID);
//
//		}
//	}
//}
//
