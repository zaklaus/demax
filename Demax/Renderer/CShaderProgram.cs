using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	public class AttributeInfo
	{
		public String name = "";
		public int address = -1;
		public int size = 0;
		public ActiveAttribType type;
	}

	public class UniformInfo
	{
		public String name = "";
		public int address = -1;
		public int size = 0;
		public ActiveUniformType type;
	}

	public class CShaderProgram
	{
		public int ProgramID = -1;
		public int VShaderID = -1;
		public int FShaderID = -1;
		public int AttributeCount = 0;
		public int UniformCount = 0;
		static string path = @"Shaders";

		public Dictionary<String, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
		public Dictionary<String, UniformInfo> Uniforms = new Dictionary<string, UniformInfo>();
		public Dictionary<String, uint> Buffers = new Dictionary<string, uint>();

		public CShaderProgram ()
		{
			ProgramID = GL.CreateProgram();
		}

		public CShaderProgram(String vshader, String fshader, bool fromFile = false)
		{
			ProgramID = GL.CreateProgram();

			if (fromFile)
			{
				LoadShaderFromFile(vshader, ShaderType.VertexShader);
				LoadShaderFromFile(fshader, ShaderType.FragmentShader);
			}
			else
			{
				LoadShaderFromString(vshader, ShaderType.VertexShader);
				LoadShaderFromString(fshader, ShaderType.FragmentShader);
			}

			Link();
			GenBuffers();
		}

		void loadShader(string code, ShaderType type, out int address)
		{
			address = GL.CreateShader (type);
			GL.ShaderSource (address, code);
			GL.CompileShader (address);
			GL.AttachShader (ProgramID, address);
			Console.WriteLine (GL.GetShaderInfoLog (address));
		}

		/// <summary>
		/// Loads the shader from string.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <param name="type">Type.</param>
		public void LoadShaderFromString(String code, ShaderType type)
		{
			if (type == ShaderType.VertexShader)
			{
				loadShader(code, type, out VShaderID);
			}
			else if (type == ShaderType.FragmentShader)
			{
				loadShader(code, type, out FShaderID);
			}
		}

		/// <summary>
		/// Loads the shader from file.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="type">Type.</param>
		public void LoadShaderFromFile(String filename, ShaderType type)
		{
			using (StreamReader sr = new StreamReader(Path.Combine(path,filename)))
			{
				if (type == ShaderType.VertexShader)
				{
					loadShader(sr.ReadToEnd(), type, out VShaderID);
				}
				else if (type == ShaderType.FragmentShader)
				{
					loadShader(sr.ReadToEnd(), type, out FShaderID);
				}
			}
		}

		/// <summary>
		/// Link this instance.
		/// </summary>
		public void Link()
        {
			GL.LinkProgram (ProgramID);
			GL.UseProgram (ProgramID);
 
            Console.WriteLine(GL.GetProgramInfoLog(ProgramID));
 
            GL.GetProgram(ProgramID, ProgramParameter.ActiveAttributes, out AttributeCount);
            GL.GetProgram(ProgramID, ProgramParameter.ActiveUniforms, out UniformCount);
 
            for (int i = 0; i < AttributeCount; i++)
            {
                AttributeInfo info = new AttributeInfo();
                int length = 0;
 
                StringBuilder name = new StringBuilder();
 
                GL.GetActiveAttrib(ProgramID, i, 256, out length, out info.size, out info.type, name);
 
                info.name = name.ToString();
                info.address = GL.GetAttribLocation(ProgramID, info.name);
                Attributes.Add(name.ToString(), info);
            }
 
            for (int i = 0; i < UniformCount; i++)
            {
                UniformInfo info = new UniformInfo();
                int length = 0;
 
                StringBuilder name = new StringBuilder();
 
                GL.GetActiveUniform(ProgramID, i, 256, out length, out info.size, out info.type, name);
 
                info.name = name.ToString();
                Uniforms.Add(name.ToString(), info);
                info.address = GL.GetUniformLocation(ProgramID, info.name);
            }
        }

		/// <summary>
		/// Generates the buffers.
		/// </summary>
		public void GenBuffers()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				Buffers.Add(Attributes.Values.ElementAt(i).name, buffer);
			}

			for (int i = 0; i < Uniforms.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				Buffers.Add(Uniforms.Values.ElementAt(i).name, buffer);
			}
		}

		/// <summary>
		/// Enables the vertex attrib arrays.
		/// </summary>
		public void EnableVertexAttribArrays()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				GL.EnableVertexAttribArray(Attributes.Values.ElementAt(i).address);
			}
		}

		/// <summary>
		/// Disables the vertex attrib arrays.
		/// </summary>
		public void DisableVertexAttribArrays()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				GL.DisableVertexAttribArray(Attributes.Values.ElementAt(i).address);
			}
		}

		public int GetAttribute(string name)
		{
			if (Attributes.ContainsKey(name))
			{
				return Attributes[name].address;
			}
			else
			{
				return -1;
			}
		}

		public int GetUniform(string name)
		{
			if (Uniforms.ContainsKey(name))
			{
				return Uniforms[name].address;
			}
			else
			{
				return -1;
			}
		}

		public uint GetBuffer(string name)
		{
			if (Buffers.ContainsKey(name))
			{
				return Buffers[name];
			}
			else
			{
				return 0;
			}
		}
	}
}

