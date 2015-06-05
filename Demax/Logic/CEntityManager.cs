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

namespace Demax
{
	/// <summary>
	/// Entity manager.
	/// </summary>
	public class CEntityManager
	{
		public List<CEntity> entities;

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
			CLog.WriteLine ("Entity: " + name);

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

