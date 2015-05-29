using System;
using System.Collections.Generic;

namespace Demax
{
	/// <summary>
	/// Entity manager.
	/// </summary>
	public class CEntityManager
	{
		List<CEntity> entities;

		/// <summary>
		/// Initializes a new instance of the <see cref="Demax.CEntityManager"/> class.
		/// </summary>
		public CEntityManager ()
		{
			entities = new List<CEntity> ();
		}

		/// <summary>
		/// Creates the entity.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="tag">Tag.</param>
		public CEntity CreateEntity(string name="NewEntity",string tag="")
		{
			CEntity e = new CEntity(name,tag);
			entities.Add(e);
			Console.WriteLine ("Entity: " + name);

			return e;
		}

		/// <summary>
		/// Gets the entities.
		/// </summary>
		/// <returns>The entities.</returns>
		public List<CEntity> GetEntities()
		{
			return entities;
		}

		/// <summary>
		/// Removes the entity.
		/// </summary>
		/// <param name="e">E.</param>
		public void RemoveEntity(CEntity e)
		{
			entities.Remove(e);
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="id">Identifier.</param>
		public CEntity GetEntity(int id)
		{
			if (id < entities.Count && id >= 0) {
				return entities [id];
			}

			return null;
		}

		/// <summary>
		/// Gets the name of the entity by.
		/// </summary>
		/// <returns>The entity by name.</returns>
		/// <param name="name">Name.</param>
		public CEntity GetEntityByName(string name)
		{
			foreach (CEntity e in entities) {
				if (e.Name == name) {
					return e;
				}
			}

			return null;
		}
	}
}

