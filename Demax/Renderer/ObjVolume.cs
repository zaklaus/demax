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
using Jitter.Collision;
using Jitter;
using Jitter.DataStructures;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System.Drawing.Imaging;

namespace Demax
{
    [Serializable]
    public class ObjVolume : Volume, ICloneable
	{


		public List<Tuple<Vector3,Vector3,Vector3>> faces = new List<Tuple<Vector3, Vector3, Vector3>>();
		public override int VertCount { get { return vertices.Length; } }
		public override int NormalCount { get { return vertices.Length; } }
		public override int IndiceCount { get { return indices.Count; } }
		public override int ColorDataCount { get { return colors.Length; } }
		public List<JVector> orig_verts = new List<JVector> ();
		public List<Tuple<int,int,int>> newfaces = new List<Tuple<int, int, int>> ();
        
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
            List<int> a = new List<int>();
            foreach (var i in indices)
                a.Add(i + offset);
			return a.ToArray();
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

		public override void AddRigidbody(string type = "")
		{
            if (type == "")
                foreach (var x in meshes)
                {
                    List<TriangleVertexIndices> f = new List<TriangleVertexIndices>();
                    for (int i = 0; i < x.indices.Count; i += 3)
                    {
                        f.Add(new TriangleVertexIndices(x.indices[i], x.indices[i + 1], x.indices[i + 2]));
                    }
                    List<JVector> verts = new List<Jitter.LinearMath.JVector>();
                    foreach (var vertex in x.vertices)
                    {
                        verts.Add(new JVector(vertex.X, vertex.Y, vertex.Z));
                    }

                    Jitter.Collision.Shapes.TriangleMeshShape t = new Jitter.Collision.Shapes.TriangleMeshShape(new Octree(verts, f));

                    var z = new RigidBody(t);
                    body.Add(z);
                    z.Position = new JVector(Position.X, Position.Y, Position.Z);

                    //body.Orientation = (JVector)Rotation;
                    CCore.GetCore().world.AddBody(z);
                }
            else if (type=="box")
            {
                var z = new RigidBody(new Jitter.Collision.Shapes.BoxShape(Scale.X, Scale.Y, Scale.Z));
                body.Add(z);
                z.Position = new JVector(Position.X, Position.Y, Position.Z);
                CCore.GetCore().world.AddBody(z);
            }
		}

		public ObjVolume (CEntity e)
			: base(e)
		{
			
		}

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static ObjVolume LoadAnimOBJ(CEntity en, string name, string dirname, int skip=0)
        {
            ObjVolume o = new ObjVolume(en);
            List<ObjVolume> a = new List<ObjVolume>();

            DirectoryInfo dir = new DirectoryInfo(dirname);
            CLog.WriteLine("Loading animation " + name + "...");
            
            for(int i = 0; i < dir.GetFiles().Length; i++)
            {
                    if (i < dir.GetFiles().Length)
                        if (dir.GetFiles()[i].Extension == ".obj")
                            a.Add(LoadOBJ(en, dir.GetFiles()[i].FullName));

                string z = (((float)i / (float)dir.GetFiles("*.obj").Length) * 100).ToString();

                CLog.Write("% " + z);

                if (skip != 0 && i % 2 == 1)
                {
                    i += skip - 1;

                }
            }
            CLog.WriteLine("Done!");
            o.LoadAnim(name, a.ToList<Volume>());
            return o;
        }

        public static ObjVolume ImportAnim(CEntity en, string name, string dirname, int skip = 0)
        {
            ObjVolume o = new ObjVolume(en);
            if (CCore.GetCore().Renderer.animations.ContainsKey(name))
            {
                List<ObjVolume> an = new List<ObjVolume>();

                foreach (var na in CCore.GetCore().Renderer.animations[name].ToList<Volume>())
                {
                    an.Add((ObjVolume)na.Clone());
                }

                o.LoadAnim(name, an.ToList<Volume>());
                return o;
            }
            List<ObjVolume> a = new List<ObjVolume>();

            DirectoryInfo dir = new DirectoryInfo(dirname);
            CLog.WriteLine("Loading animation " + name + "...");
            
            for (int i = 0; i < dir.GetFiles().Length; i++)
            {
                if (i < dir.GetFiles().Length)
                    if (dir.GetFiles()[i].Extension == ".mod")
                    {
                        ObjVolume m = null;
                        if (i == 0)
                            m = ImportModel(en, dir.GetFiles()[i].FullName.Split('.')[0], true, dir.GetFiles()[i].FullName.Split('.')[0]);
                        else
                            m = ImportModel(en, dir.GetFiles()[i].FullName.Split('.')[0], false);
                        if (a.Count > 0)
                        {
                            m.indices = a[0].indices;
                            m.normals = a[0].normals;
                            m.texturecoords = a[0].texturecoords;
                            m.materials = a[0].materials;
                            m.matuse = a[0].matuse;
                            int j = 0;
                            foreach (var n in a[0].meshes)
                            {
                                m.meshes[j].materials = n.materials;
                                m.meshes[j].matuse = n.matuse;
                                j++;
                            }
                        }
                        a.Add(m);
                    }
                string z = (((float)i / (float)dir.GetFiles("*.mod").Length) * 100).ToString();

                CLog.Write("% " + z);

                if (skip != 0 && i % 2 == 1)
                {
                    i += skip - 1;

                }
            }
            CLog.WriteLine("Done!");
            o.LoadAnim(name, a.ToList<Volume>());

            if (!CCore.GetCore().Renderer.animations.ContainsKey(name))
                CCore.GetCore().Renderer.animations.Add(name, a.ToList());

            return o;
        }

        public static ObjVolume[] LoadFromFolder(CEntity en, string dirname)
        {
            List<ObjVolume> a = new List<ObjVolume>();

            DirectoryInfo dir = new DirectoryInfo(dirname);
            foreach(var file in dir.GetFiles())
            {
                if(file.Extension == ".obj")
                    a.Add(LoadOBJ(en, file.FullName.Split('.')[0]));
            }

            return a.ToArray();
        }


		public static ObjVolume LoadOBJ(CEntity en, string filename)
		{
            if (CCore.GetCore().Renderer.volumes.ContainsKey(filename))
                return (ObjVolume) CCore.GetCore().Renderer.volumes[filename].Clone();

            //filename = filename + ".obj";

            ObjVolume obj = new ObjVolume(en);
			try
			{
				using (StreamReader reader = new StreamReader(new FileStream(filename+".obj", FileMode.Open, FileAccess.Read)))
				{
					String file = reader.ReadToEnd();
					String[] parts = file.Split(new string[]{"o "}, StringSplitOptions.None);

					String[] header = parts[0].Split('\n');
					foreach(String line in header)
					{
						if (line.StartsWith ("mtllib ")) {
							String temp = line.Substring (7);
							try
							{
								using (StreamReader reader2 = new StreamReader(new FileStream(Path.Combine(Path.GetDirectoryName(filename + ".mtl"),temp), FileMode.Open, FileAccess.Read)))
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
                                            
										}
									}
								}
							}
							catch (FileNotFoundException e)
							{
								CLog.WriteLine("File not found: {0}", temp);
							}
							catch (Exception e)
							{
								CLog.WriteLine(string.Format("Error loading file: {0}, {1}", temp, e.ToString()));
							}
						}
					}
					for(int i=1; i<parts.Length;i++){
						if(i==1)
							obj.meshes.Add(LoadFromString(en, parts[i], filename + ".obj", new Tuple<int, int, int>(0,0,0)));
						else{
							var m = obj.meshes[obj.meshes.Count-1].maxIndex;
							obj.meshes.Add(LoadFromString(en, parts[i], filename + ".obj", new Tuple<int, int, int>(m.Item1,m.Item2,m.Item3)));
						}
					}
				}
			}
			catch (FileNotFoundException e)
			{
				CLog.WriteLine("File not found: {0}", filename);
			}
			catch (Exception e)
			{
				CLog.WriteLine("Error loading file: {0}", filename);
			}
			//CLog.WriteLine ("Meshes: " + obj.meshes.Count + " VertCount[0]: " + obj.meshes[0].VertCount);

            if(!CCore.GetCore().Renderer.volumes.ContainsKey(filename))
                CCore.GetCore().Renderer.volumes.Add(filename, obj);

			return obj;
		}

		public static ObjVolume LoadFromString(CEntity en, string obj, string p, Tuple<int,int,int> lastIndex)
		{
			// Seperate lines from the file
			List<String> lines = new List<string>(obj.Split('\n'));
			string relp = Path.GetDirectoryName (p);
			//CLog.WriteLine ("OBJ RelP: " + relp);

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
							CLog.WriteLine ("Error parsing vertex: {0}", line);
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
							CLog.WriteLine ("Error parsing texcoords: {0}", line);
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
							CLog.WriteLine ("Error parsing normals: {0}", line);
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
							CLog.WriteLine ("Error parsing face: {0}", line);
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
			//CLog.WriteLine ("VertIndex: "+faces[0].Item1.X+" and face count: "+faces.Count);
			//CLog.WriteLine ("VertOffset: " + (faces [0].Item1.X - lastIndex.Item1));
			int offset = (lastIndex.Item1 > 0) ? -1 : 0;

			List<Tuple<int,int,int>> newfaces = new List<Tuple<int, int, int>> ();

			for (int i = 0; i < faces.Count; i++) {
				newfaces.Add (new Tuple<int, int, int> (
					MathHelper.Clamp((int)faces [i].Item1.X - lastIndex.Item1+offset, 0, verts.Count-1), 
					MathHelper.Clamp((int)faces [i].Item2.X - lastIndex.Item2+offset, 0, texs.Count-1), 
					MathHelper.Clamp((int)faces [i].Item3.X - lastIndex.Item3+offset, 0, normals.Count-1)));
				newfaces.Add (new Tuple<int, int, int> (
					MathHelper.Clamp((int)faces [i].Item1.Y - lastIndex.Item1+offset, 0, verts.Count-1), 
					MathHelper.Clamp((int)faces [i].Item2.Y - lastIndex.Item2+offset, 0, texs.Count-1), 
					MathHelper.Clamp((int)faces [i].Item3.Y - lastIndex.Item3+offset, 0, normals.Count-1)));
				newfaces.Add (new Tuple<int, int, int> (
					MathHelper.Clamp((int)faces [i].Item1.Z - lastIndex.Item1+offset, 0, verts.Count-1), 
					MathHelper.Clamp((int)faces [i].Item2.Z - lastIndex.Item2+offset, 0, texs.Count-1), 
					MathHelper.Clamp((int)faces [i].Item3.Z - lastIndex.Item3+offset, 0, normals.Count-1)));
			}

			for (int i = 0; i < newfaces.Count; i++) {
				try{
					temp_vertices.Add(verts[newfaces[i].Item1]);
				} catch(Exception ex){
					CLog.WriteLine (ex.ToString ());
					CLog.WriteLine (verts.Count.ToString());
					CLog.WriteLine ("Indices Vertex error: " + (faces[i].Item1.X - lastIndex.Item1));
					Console.ReadKey ();
				}
				try{
					temp_normals.Add(normals[newfaces[i].Item3]);
				} catch(Exception ex){
					CLog.WriteLine (ex.ToString ());
					CLog.WriteLine ("Indices Normal error: " + (faces[i].Item3.X - lastIndex.Item3));
					Console.ReadKey ();
				}
				try{
					temp_uv.Add(texs[newfaces[i].Item2]);
				} catch(Exception ex){
					CLog.WriteLine (ex.ToString ());
					CLog.WriteLine ("Indices UV error: " + (faces[i].Item2.X - lastIndex.Item2));
					Console.ReadKey ();
				}
			}
			//vol.vertices = temp_vertices.ToArray ();
			//vol.faces = new List<Tuple<Vector3, Vector3, Vector3>> (faces);
			//vol.colors = colors.ToArray();
			//vol.texturecoords = temp_uv.ToArray();
			//vol.normals = temp_normals.ToArray ();
			vol.matuse = new Dictionary<int, string> (mu);
			vol.materials = new List<Material> (mats);
			vol.maxIndex = new Tuple<int, int, int> (max[0],max[1],max[2]);
			foreach (var z in verts)
				vol.orig_verts.Add (new JVector (z.X, z.Y, z.Z));
			vol.newfaces = new List<Tuple<int, int, int>> (newfaces);
            List<Vector3> new_vertices = new List<Vector3>();
            List<Vector3> new_normals = new List<Vector3>();
            List<Vector2> new_uvs = new List<Vector2>();
            indexVBO(temp_vertices, temp_uv, temp_normals, ref vol.indices, ref new_vertices, ref new_uvs, ref new_normals);
            vol.vertices = new_vertices.ToArray();
            vol.normals = new_normals.ToArray();
            vol.colors = colors.ToArray();
            vol.texturecoords = new_uvs.ToArray();
			//CLog.WriteLine ("Last Vertex: " + vol.vertices[vol.vertices.Count()-1].X);
			return vol;
		}

        public static void indexVBO(
            List<Vector3> in_vertices,
            List<Vector2> in_uvs,
            List<Vector3> in_normals,
            ref List<int> out_indices,
            ref List<Vector3> out_vertices,
            ref List<Vector2> out_uvs,
            ref List<Vector3> out_normals
            )
        {
            // For each input vertex
            for ( int i=0; i<in_vertices.Count; i++ ){

                    // Try to find a similar vertex in out_XXXX
                    int index=0;
                    bool found = getSimilarVertexIndex(in_vertices[i], in_uvs[i], in_normals[i], ref out_vertices, ref out_uvs, ref out_normals, ref index);

                    if ( found ){ // A similar vertex is already in the VBO, use it instead !
                        out_indices.Add( index );
                        out_normals[index] = in_normals[i];    
                    }else{ // If not, it needs to be added in the output data.
                        out_vertices.Add( in_vertices[i]);
                        out_uvs     .Add( in_uvs[i]);
                        out_normals .Add( in_normals[i]);
                        out_indices .Add( (int)out_vertices.Count - 1 );
                    }
            }
        }
        static bool getSimilarVertexIndex( 
            Vector3 in_vertex, 
            Vector2 in_uv, 
            Vector3 in_normal, 
            ref List<Vector3> out_vertices,
            ref List<Vector2> out_uvs,
            ref List<Vector3> out_normals,
            ref int result
            )
        {
            // Lame linear search
            for ( int i=0; i<out_vertices.Count; i++ ){
                    if (
                            is_near( in_vertex.X , out_vertices[i].X ) &&
                            is_near( in_vertex.Y , out_vertices[i].Y ) &&
                            is_near( in_vertex.Z , out_vertices[i].Z ) &&
                            is_near( in_uv.X     , out_uvs     [i].X ) &&
                            is_near( in_uv.Y     , out_uvs     [i].Y ) &&
                            is_near( in_normal.X , out_normals [i].X ) &&
                            is_near( in_normal.Y , out_normals [i].Y ) &&
                            is_near( in_normal.Z , out_normals [i].Z )
                    ){
                            result = i;
                            return true;
                    }
            }
            // No other vertex could be used instead.
            // Looks like we'll have to add it to the VBO.
            return false;
        }

        static bool is_near(float v1, float v2)
        {
            return Math.Abs(v1 - v2) < 0.01f;
        }

        public static void ExportModelFolder(CEntity en, string dirname, string exportdir="", bool shareTexture=false, bool NoTextures=false)
        {
            DirectoryInfo dir = new DirectoryInfo(dirname);
            int i = 1;
            string texname = "";
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == ".obj")
                {
                    if (exportdir != "")
                        dirname = exportdir;

                    if (i == 1)
                        texname = Path.Combine(dirname, file.Name.Split('.')[0]);
                    var o = LoadOBJ(en, file.FullName.Split('.')[0]);
                    
                   
                    if (NoTextures)
                        o.ExportModel(Path.Combine(dirname, file.Name.Split('.')[0]), false);
                    else if (shareTexture)
                        o.ExportModel(Path.Combine(dirname, file.Name.Split('.')[0]), true, texname);
                    else
                        o.ExportModel(Path.Combine(dirname, file.Name.Split('.')[0]));
                    CCore.GetCore().Renderer.volumes.Remove(dir.FullName);

                    string z = (((float)i / (float)dir.GetFiles("*.obj").Length) * 100).ToString();

                    CLog.Write("% " + z);

                    i++;
                }
            }
        }

        public void ExportModel(string filename, bool exportTextures=true, string texname="")
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(filename + ".mod", FileMode.Create));

            // Model count
            bw.Write(meshes.Count);

            // Models

            foreach (var m in meshes)
            {
                // Material to use
                bw.Write(m.matuse.Keys.ElementAt(0));
                bw.Write(m.matuse.Values.ElementAt(0));

                // Verts count
                bw.Write(m.VertCount);

                // Vertices
                foreach (var verts in m.vertices)
                {
                    bw.Write(verts.X);
                    bw.Write(verts.Y);
                    bw.Write(verts.Z);
                }

                // UVs count
                bw.Write(m.texturecoords.Length);

                // UVs
                foreach (var uvs in m.texturecoords)
                {
                    bw.Write(uvs.X);
                    bw.Write(uvs.Y);
                }

                // Normals count
                bw.Write(m.NormalCount);

                // Normals
                foreach (var norms in m.normals)
                {
                    bw.Write(norms.X);
                    bw.Write(norms.Y);
                    bw.Write(norms.Z);
                }

                // Indice Count
                bw.Write(m.IndiceCount);

                // Indices
                foreach (var i in m.indices)
                {
                    bw.Write(i);
                }
            }

            if (exportTextures)
            {
                BinaryWriter texw = null;

                if (texname == "")
                {
                    bw.Write(Path.GetFileNameWithoutExtension(filename) + ".tex");
                    texw = new BinaryWriter(new FileStream(filename + ".tex", FileMode.Create));
                }
                else
                {
                    bw.Write(texname + ".tex");
                    texw = new BinaryWriter(new FileStream(texname + ".tex", FileMode.Create));
                }
                texw.Write(materials.Count);
                foreach (var mat in materials)
                {
                    var orig = new Bitmap(mat.filename);
                    using (var ms = new MemoryStream())
                    {
                        orig.Save(ms, ImageFormat.Jpeg);
                        texw.Write(mat.filename);
                        texw.Write(mat.name);
                        texw.Write(ms.Length);
                        texw.Write(ms.ToArray());
                    }
                }

                texw.Flush();
                texw.Close();
            }
            
            bw.Flush();
            bw.Close();
        }

        public static ObjVolume ImportModel(CEntity e, string filename, bool ImportTextures=true, string texname = "")
        {
            if (CCore.GetCore().Renderer.volumes.ContainsKey(filename))
                return (ObjVolume)CCore.GetCore().Renderer.volumes[filename].Clone();

            ObjVolume v = new ObjVolume(e);


            var br = new BinaryReader(new FileStream(filename+".mod", FileMode.Open));
            int meshesCount = br.ReadInt32();

            for (int i = 0; i < meshesCount; i++)
            {
                ObjVolume m = new ObjVolume(e);

                m.matuse.Add(br.ReadInt32(), br.ReadString());
                List<Vector3> verts = new List<Vector3>();
                List<Vector3> norms = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();

                // Verts
                int vertcount = br.ReadInt32();
                for (int j = 0; j < vertcount; j++)
                {
                    Vector3 vertex = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                    verts.Add(vertex);
                }

                // UVs
                int texcoordcount = br.ReadInt32();
                for (int j = 0; j < texcoordcount; j++)
                {
                    Vector2 uv = new Vector2(br.ReadSingle(), br.ReadSingle());

                    uvs.Add(uv);
                }

                // Norms
                int normcount = br.ReadInt32();
                for (int j = 0; j < normcount; j++)
                {
                    Vector3 normal = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                    norms.Add(normal);
                }

                // Inds
                int indcount = br.ReadInt32();
                for (int j = 0; j < indcount; j++)
                {
                    int indice = br.ReadInt32();
                    m.indices.Add(indice);
                }

                m.vertices = verts.ToArray();
                m.normals = norms.ToArray();
                m.texturecoords = uvs.ToArray();

                v.meshes.Add(m);
            }

            if (ImportTextures)
            {
                br.ReadString(); // obsolete
                BinaryReader texr = null;
                if (texname == "")
                    texr = new BinaryReader(new FileStream(filename.Split('.')[0] + ".tex", FileMode.Open));
                else
                {
                    texr = new BinaryReader(new FileStream(texname + ".tex", FileMode.Open));
                }

                int materialcount = texr.ReadInt32();
                for (int j = 0; j < materialcount; j++)
                {
                    string filenameTex = texr.ReadString();
                    string matname = texr.ReadString();
                    int imgsize = (int)texr.ReadInt64();
                    byte[] imgdata = texr.ReadBytes(imgsize);
                    Bitmap img;
                    using (var ms = new MemoryStream(imgdata))
                    {
                        Image im = Image.FromStream(ms);
                        img = new Bitmap(im);
                    }

                    Material mat = new Material(matname, filenameTex, img);
                    v.materials.Add(mat);
                }

                texr.Close();
            }
            
            br.Close();

            if (!CCore.GetCore().Renderer.volumes.ContainsKey(filename))
                CCore.GetCore().Renderer.volumes.Add(filename, v);

            return v;
        }

        public void AddLOD(string filename, float dist, string type="mod")
        {
            if (CCore.GetCore().Renderer.volumes.ContainsKey(filename))
            {
                try {
                    LOD.Add(dist, (ObjVolume)CCore.GetCore().Renderer.volumes[filename].Clone());
                }
                catch { }
                return;
            }


            ObjVolume o = null;
            if (type == "mod")
                o = ImportModel(me, filename);
            else if (type == "obj")
                o = LoadOBJ(me, filename + ".obj");
            LOD.Add(dist, o);
        }
    }
}