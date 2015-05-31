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
using System.Drawing;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System.Drawing.Imaging;
using System.Globalization;

namespace Demax
{
	public class ObjVolume : Volume
	{
		Vector3[] vertices;
		Vector3[] colors;
		Vector3[] normals;
		Vector2[] texturecoords;

		public List<Tuple<Vector3,Vector3,Vector3>> faces = new List<Tuple<Vector3, Vector3, Vector3>>();
		public override int VertCount { get { return vertices.Length; } }
		public override int NormalCount { get { return vertices.Length; } }
		public override int IndiceCount { get { return faces.Count * 3; } }
		public override int ColorDataCount { get { return colors.Length; } }
		/// <summary>
		/// Get vertices for this object
		/// </summary>
		/// <returns></returns>
		public override Vector3[] GetVerts()
		{
			return vertices;
		}

		/// <summary>
		/// Get indices to draw this object
		/// </summary>
		/// <param name="offset">Number of vertices buffered before this object</param>
		/// <returns>Array of indices with offset applied</returns>
		public override int[] GetIndices(int offset = 0)
		{
			List<int> temp = new List<int>();

			foreach (var face in faces)
			{
				temp.Add((int)face.Item1.X + offset);
				temp.Add((int)face.Item1.Y + offset);
				temp.Add((int)face.Item1.Z + offset);
			}

			return temp.ToArray();
		}

		/// <summary>
		/// Get color data.
		/// </summary>
		/// <returns></returns>
		public override Vector3[] GetColorData()
		{
			return colors;
		}

		/// <summary>
		/// Get texture coordinates
		/// </summary>
		/// <returns></returns>
		public override Vector2[] GetTextureCoords()
		{
			return texturecoords;
		}

		public override Vector3[] GetNormals()
		{
			return normals;
		}

		public ObjVolume (CEntity e)
			: base(e)
		{
			
		}

		public static ObjVolume LoadFromFile(CEntity en, string filename)
		{
			ObjVolume obj = new ObjVolume(en);
			try
			{
				using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
				{
					String file = reader.ReadToEnd();
					String[] parts = file.Split(new string[]{"o "}, StringSplitOptions.None);

					String[] header = parts[0].Split('\n');
					foreach(String line in header)
					{
						if (line.StartsWith ("mtllib ")) {
							String temp = line.Substring (7);
							Console.WriteLine("Model MTL: "+temp);
							try
							{
								using (StreamReader reader2 = new StreamReader(new FileStream(Path.Combine(Path.GetDirectoryName(filename),temp), FileMode.Open, FileAccess.Read)))
								{
									string content = reader2.ReadToEnd();

									List<String> mlines = new List<string>(content.Split('\n'));

									String mname="";
									String texpath="";
									foreach(String mline in mlines)
									{
										if(mline.StartsWith("newmtl "))
										{
											String mtemp = mline.Substring(7);

											mname = mtemp;
										}
										else if(mline.StartsWith("map_Kd "))
										{
											String mtemp = mline.Substring(7);

											texpath = mtemp;

											obj.materials.Add(new Material(mname,Path.Combine(Path.GetDirectoryName(filename),texpath)));

											Console.WriteLine (mtemp);
										}
									}
								}
							}
							catch (FileNotFoundException e)
							{
								Console.WriteLine("File not found: {0}", temp);
							}
							catch (Exception e)
							{
								Console.WriteLine("Error loading file: {0}", temp);
							}
						}
					}
					for(int i=1; i<parts.Length;i++){
						if(i==1)
							obj.meshes.Add(LoadFromString(en, parts[i], filename, new Tuple<int, int, int>(0,0,0)));
						else{
							var m = obj.meshes[obj.meshes.Count-1].maxIndex;
							obj.meshes.Add(LoadFromString(en, parts[i], filename, new Tuple<int, int, int>(m.Item1,m.Item2,m.Item3)));
						}
					}
				}
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("File not found: {0}", filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error loading file: {0}", filename);
			}
			Console.WriteLine ("Meshes: " + obj.meshes.Count + " VertCount[0]: " + obj.meshes[0].VertCount);
			return obj;
		}

		public static ObjVolume LoadFromString(CEntity en, string obj, string p, Tuple<int,int,int> lastIndex)
		{
			// Seperate lines from the file
			List<String> lines = new List<string>(obj.Split('\n'));
			string relp = Path.GetDirectoryName (p);
			Console.WriteLine ("OBJ RelP: " + relp);

			// Lists to hold model data
			List<Vector3> verts = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector3> colors = new List<Vector3>();
			List<Vector2> texs = new List<Vector2>();
			List<Tuple<Vector3,Vector3,Vector3>> faces = new List<Tuple<Vector3, Vector3, Vector3>>();
			List<Vector2> uvIndices = new List<Vector2> ();
			List<Material> mats = new List<Material> ();
			Tuple<int,int,int> nextIndex = new Tuple<int, int, int> (0, 0, 0);
			Dictionary<int, string> mu = new Dictionary<int, string> ();
			int[] max = new int[]{0,0,0};
			// Read file line by line
			foreach (String line in lines)
			{
				if (line.StartsWith ("v ")) { // Vertex definition
					// Cut off beginning of line
					String temp = line.Substring (2);

					Vector3 vec = new Vector3 ();

					if (temp.Count ((char c) => c == ' ') == 2) { // Check if there's enough elements for a vertex
						String[] vertparts = temp.Split (' ');

						// Attempt to parse each part of the vertice
						bool success = float.TryParse (vertparts [0], NumberStyles.Any, new CultureInfo ("en-US"), out vec.X);
						success |= float.TryParse (vertparts [1], NumberStyles.Any, new CultureInfo ("en-US"), out vec.Y);
						success |= float.TryParse (vertparts [2], NumberStyles.Any, new CultureInfo ("en-US"), out vec.Z);


						// Dummy color/texture coordinates for now
						colors.Add (new Vector3 ((float)Math.Sin (vec.Z), (float)Math.Sin (vec.Z), (float)Math.Sin (vec.Z)));

						// If any of the parses failed, report the error
						if (!success) {
							Console.WriteLine ("Error parsing vertex: {0}", line);
						}
					}

					verts.Add (vec);
				} else if (line.StartsWith ("vt ")) {
					String temp = line.Substring (3);

					Vector2 vec = new Vector2 ();

					if (temp.Count ((char c) => c == ' ') == 1) { // Check if there's enough elements for a vertex
						String[] vertparts = temp.Split (' ');

						// Attempt to parse each part of the vertice
						bool success = float.TryParse (vertparts [0], NumberStyles.Any, new CultureInfo ("en-US"), out vec.X);
						success |= float.TryParse (vertparts [1], NumberStyles.Any, new CultureInfo ("en-US"), out vec.Y);

						texs.Add (vec);

						// If any of the parses failed, report the error
						if (!success) {
							Console.WriteLine ("Error parsing texcoords: {0}", line);
						}
					}
				} else if (line.StartsWith ("vn ")) {
					String temp = line.Substring (3);
				

					
					Vector3 vec = new Vector3 ();

					if (temp.Count ((char c) => c == ' ') == 2) { // Check if there's enough elements for a vertex
						String[] vertparts = temp.Split (' ');

						// Attempt to parse each part of the vertice
						bool success = float.TryParse (vertparts [0], NumberStyles.Any, new CultureInfo ("en-US"), out vec.X);
						success |= float.TryParse (vertparts [1], NumberStyles.Any, new CultureInfo ("en-US"), out vec.Y);
						success |= float.TryParse (vertparts [1], NumberStyles.Any, new CultureInfo ("en-US"), out vec.Z);

						normals.Add (vec);

						// If any of the parses failed, report the error
						if (!success) {
							Console.WriteLine ("Error parsing normals: {0}", line);
						}
					}
				} else if (line.StartsWith ("f ")) { // Face definition
					// Cut off beginning of line
					String temp = line.Substring (2);

					Tuple<Vector3, Vector3, Vector3 > face = new Tuple<Vector3, Vector3, Vector3> (Vector3.Zero, Vector3.Zero, Vector3.Zero);

					if (temp.Count ((char c) => c == ' ') == 2) { // Check if there's enough elements for a face
						String[] faceparts = temp.Split (' ');
						string f1 = faceparts [0].Split ('/') [0];
						string f2 = faceparts [1].Split ('/') [0];
						string f3 = faceparts [2].Split ('/') [0];
						string f4 = faceparts [0].Split ('/') [1];
						string f5 = faceparts [1].Split ('/') [1];
						string f6 = faceparts [2].Split ('/') [1];
						string f7 = faceparts [0].Split ('/') [2];
						string f8 = faceparts [1].Split ('/') [2];
						string f9 = faceparts [2].Split ('/') [2];
						int i1, i2, i3, i4, i5, i6, i7, i8, i9;

						// Attempt to parse each part of the face
						bool success = int.TryParse (f1, NumberStyles.Any, new CultureInfo ("en-US"), out i1);
						success |= int.TryParse (f2, NumberStyles.Any, new CultureInfo ("en-US"), out i2);
						success |= int.TryParse (f3, NumberStyles.Any, new CultureInfo ("en-US"), out i3);
						success |= int.TryParse (f4, NumberStyles.Any, new CultureInfo ("en-US"), out i4);
						success |= int.TryParse (f5, NumberStyles.Any, new CultureInfo ("en-US"), out i5);
						success |= int.TryParse (f6, NumberStyles.Any, new CultureInfo ("en-US"), out i6);
						success |= int.TryParse (f7, NumberStyles.Any, new CultureInfo ("en-US"), out i7);
						success |= int.TryParse (f8, NumberStyles.Any, new CultureInfo ("en-US"), out i8);
						success |= int.TryParse (f9, NumberStyles.Any, new CultureInfo ("en-US"), out i9);

						// If any of the parses failed, report the error
						if (!success) {
							Console.WriteLine ("Error parsing face: {0}", line);
						} else {
							// Decrement to get zero-based vertex numbers
							face = new Tuple<Vector3, Vector3, Vector3>(new Vector3(i1-1,i2-1,i3-1),new Vector3(i4-1,i5-1,i6-1),new Vector3(i7-1,i8-1,i9-1));
							faces.Add (face);

							if (i1 - 1 > max [0])
								max [0] = i1-1;
							if (i2 - 1 > max [0])
								max [0] = i2-1;
							if (i3 - 1 > max [0])
								max [0] = i3-1;

							if (i4 - 1 > max [1])
								max [1] = i4-1;
							if (i5 - 1 > max [1])
								max [1] = i5-1;
							if (i6 - 1 > max [1])
								max [1] = i6-1;

							if (i7 - 1 > max [2])
								max [2] = i7-1;
							if (i8 - 1 > max [2])
								max [2] = i8-1;
							if (i9 - 1 > max [2])
								max [2] = i9-1;
						}
					}
				} else if (line.StartsWith ("usemtl ")) {
					String temp = line.Substring (7);

					int matpos = faces.Count;

					mu.Add (matpos, temp);
				}
			}
			// Create the ObjVolume
			ObjVolume vol = new ObjVolume(en);
			List<Vector3> temp_vertices = new List<Vector3> ();
			List<Vector3> temp_normals = new List<Vector3> ();
			List<Vector2> temp_uv = new List<Vector2> ();
			Console.WriteLine ("VertIndex: "+faces[0].Item1.X+" and face count: "+faces.Count);
			Console.WriteLine ("VertOffset: " + (faces [0].Item1.X - lastIndex.Item1));
			int offset = (lastIndex.Item1 > 0) ? -1 : 0;
			for (int i = 0; i < faces.Count; i++) {
				try{
					temp_vertices.Add(verts[MathHelper.Clamp(((int)faces[i].Item1.X - lastIndex.Item1+offset), 0, verts.Count-1)]);
					temp_vertices.Add(verts[MathHelper.Clamp(((int)faces[i].Item1.Y - lastIndex.Item1+offset), 0, verts.Count-1)]);
					temp_vertices.Add(verts[MathHelper.Clamp(((int)faces[i].Item1.Z - lastIndex.Item1+offset), 0, verts.Count-1)]);
				} catch(Exception ex){
					Console.WriteLine (ex.ToString ());
					Console.WriteLine (verts.Count);
					Console.WriteLine ("Indices Vertex error: " + (faces[i].Item1.X - lastIndex.Item1));
					Console.ReadKey ();
				}
				try{
					temp_normals.Add(normals[MathHelper.Clamp(((int)faces[i].Item3.X - lastIndex.Item3+offset), 0, normals.Count-1)]);
					temp_normals.Add(normals[MathHelper.Clamp(((int)faces[i].Item3.Y - lastIndex.Item3+offset), 0, normals.Count-1)]);
					temp_normals.Add(normals[MathHelper.Clamp(((int)faces[i].Item3.Z - lastIndex.Item3+offset), 0, normals.Count-1)]);
				} catch(Exception ex){
					Console.WriteLine (ex.ToString ());
					Console.WriteLine ("Indices Normal error: " + (faces[i].Item3.X - lastIndex.Item3));
					Console.ReadKey ();
				}
				try{
					temp_uv.Add(texs[MathHelper.Clamp(((int)faces[i].Item2.X - lastIndex.Item2+offset), 0, texs.Count-1)]);
					temp_uv.Add(texs[MathHelper.Clamp(((int)faces[i].Item2.Y - lastIndex.Item2+offset), 0, texs.Count-1)]);
					temp_uv.Add(texs[MathHelper.Clamp(((int)faces[i].Item2.Z - lastIndex.Item2+offset), 0, texs.Count-1)]);
				} catch(Exception ex){
					Console.WriteLine (ex.ToString ());
					Console.WriteLine ("Indices UV error: " + (faces[i].Item2.X - lastIndex.Item2));
					Console.ReadKey ();
				}
			}
			vol.vertices = temp_vertices.ToArray ();
			vol.faces = new List<Tuple<Vector3, Vector3, Vector3>> (faces);
			vol.colors = colors.ToArray();
			vol.texturecoords = temp_uv.ToArray();
			vol.normals = temp_normals.ToArray ();
			vol.matuse = new Dictionary<int, string> (mu);
			vol.materials = new List<Material> (mats);
			vol.maxIndex = new Tuple<int, int, int> (max[0],max[1],max[2]);
			Console.WriteLine ("Last Vertex: " + vol.vertices[vol.vertices.Count()-1].X);
			return vol;
		}
	}
}