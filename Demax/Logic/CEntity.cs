using System;
using System.Collections.Generic;

namespace Demax
{
	/// <summary>
	/// Entity.
	/// </summary>
	public class CEntity
	{
		/// <summary>
		/// The name.
		/// </summary>
		public string Name, Tag;
		List<Volume> models;
		List<CEntity> childs;
		List<CScript> scripts;

		/// <summary>
		/// The transform.
		/// </summary>
		public CTransform Transform;
		CEntity parent;

		/// <summary>
		/// The game.
		/// </summary>
		public CCore game;



		/// <summary>
		/// Initializes a new instance of the <see cref="Demax.CEntity"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="tag">Tag.</param>
		/// <param name="par">Par.</param>
		public CEntity (string name="NewEntity", string tag="", CEntity par=null)
		{
			Name = name;
			Tag = tag;
			game = CCore.GetCore ();
			parent = par;
			models = new List<Volume> ();
			childs = new List<CEntity> ();
			scripts = new List<CScript> ();
			Transform = new CTransform (new OpenTK.Vector3( 0f, 0f, 0f ), new OpenTK.Vector3( 0f, 0f, 0f ), new OpenTK.Vector3 (
				0f,
				0f,
				0f
			));


		}

		/// <summary>
		/// Adds the script.
		/// </summary>
		/// <returns>The script.</returns>
		/// <param name="filename">Filename.</param>
		public CScript AddScript(string filename)
		{
			CScript s = new CScript (filename, this);
			scripts.Add (s);

			return s;
		}

		/// <summary>
		/// Gets the scripts.
		/// </summary>
		/// <returns>The scripts.</returns>
		public List<CScript> GetScripts()
		{
			return scripts;
		}

		/// <summary>
		/// Adds the model.
		/// </summary>
		/// <param name="vol">Vol.</param>
		public void AddModel(Volume vol)
		{
			models.Add (vol);
		}

		public CEntity CreateEntity(string name="NewChild",string tag="")
		{
			CEntity e = new CEntity(name,tag,this);
			childs.Add (e);

			return e;
		}

		/// <summary>
		/// Gets the models.
		/// </summary>
		/// <returns>The models.</returns>
		public List<Volume> GetModels()
		{
			return models;
		}

		/// <summary>
		/// Gets the models.
		/// </summary>
		/// <value>The models.</value>
		public List<Volume> Models
		{
			get {
				return models;
			}
		}

		/// <summary>
		/// Gets the child.
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="id">Identifier.</param>
		public CEntity GetChild(int id)
		{
			if (id >= 0 && id < childs.Count) {
				return childs [id];
			}

			return null;
		}

		/// <summary>
		/// Gets the name of the child by.
		/// </summary>
		/// <returns>The child by name.</returns>
		/// <param name="name">Name.</param>
		public CEntity GetChildByName(string name)
		{
			foreach (CEntity e in childs) {
				if (e.Name == name) {
					return e;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the childs.
		/// </summary>
		/// <returns>The childs.</returns>
		public List<CEntity> GetChilds()
		{
			return childs;
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <returns>The parent.</returns>
		public CEntity GetParent()
		{
			return parent;
		}

		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <returns>The root.</returns>
		public CEntity GetRoot()
		{
			CEntity ret = parent;
			if (ret == null) {
				return this;
			}

			if (ret.parent == null) {
				return ret;
			}

			do {
				if (ret.parent != null) {
					ret = ret.parent;
				}
			} while(ret != null);
			return ret;
		}

		public CTransform RecursiveTransform()
		{
			CEntity ret = parent;
			CTransform c = this.Transform;
			if (ret == null) {
				return c;
			}

			if (ret.parent == null) {
				return ret.Transform + c;
			}

			do {
				if (ret.parent != null) {
					c = c + ret.Transform;
					ret = ret.parent;
				}
			} while(ret != null);
			return c;
		}
	}
}

