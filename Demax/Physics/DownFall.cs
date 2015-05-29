using System;
using System.Collections.Generic;
using OpenTK;

namespace Demax
{
	public class DownFall
	{
		CCore core;
		Dictionary<int,float> fallspeed = new Dictionary<int, float>();
		Dictionary<int,bool> changed = new Dictionary<int,bool>();

		public DownFall ()
		{
			core = CCore.GetCore ();
		}

		/// <summary>
		/// Does the fall.
		/// </summary>
		public void DoFall()
		{
			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume a in en.Models) {
					foreach (CEntity en2 in core.EntityManager.GetEntities()) {
						foreach (Volume b in en2.Models) {
							ModSpeed (a.GetHashCode (), 0f, true);
							ModSpeed (b.GetHashCode (), 0f, true);
							if (!changed.ContainsKey (a.GetHashCode()))
								changed.Add (a.GetHashCode(), false);
							if (AABB.Intersect3D (new Vector3[]{ a.Position, a.Scale }, new Vector3[]{ b.Position, b.Scale }) == 1) {
								if (a.isStatic || fallspeed [a.GetHashCode ()] == 0f) {
									ModSpeed (b.GetHashCode (), 0f);
								} else if (b.isStatic || fallspeed [b.GetHashCode ()] == 0f) {
									ModSpeed (a.GetHashCode (), 0f);
								} else if (!a.isStatic && !b.isStatic) {
									fallspeed [a.GetHashCode ()] = fallspeed [b.GetHashCode ()] = 0f;
								}
							} else {
								if (a.isStatic || fallspeed[a.GetHashCode()]==0f) {
									ModSpeed (b.GetHashCode (), 0.0001f, true);
								} else if (b.isStatic  || fallspeed[b.GetHashCode()]==0f) {
									ModSpeed (a.GetHashCode (), 0.0001f, true);
								}
							}

							if (!a.isStatic && !changed [a.GetHashCode ()]) {
								a.Position = a.Position - new Vector3 (0, fallspeed [a.GetHashCode ()], 0);
								changed [a.GetHashCode ()] = true;
							}							
						}
					}
				}
			}
			foreach (CEntity en in core.EntityManager.GetEntities()) {
				foreach (Volume a in en.Models) {
					if (!changed.ContainsKey (a.GetHashCode()))
						changed.Add (a.GetHashCode(), false);
					else
						changed [a.GetHashCode()] = false;
				}
			}
		}

		/// <summary>
		/// Mods the speed.
		/// </summary>
		/// <param name="hash">Hash.</param>
		/// <param name="speed">Speed.</param>
		public void ModSpeed(int hash, float speed, bool add=false)
		{
			

			if (!fallspeed.ContainsKey (hash))
				fallspeed.Add (hash, speed);
			else if (add)
				fallspeed [hash] += speed;
			else
				fallspeed [hash] = speed;
		}
	}
}

