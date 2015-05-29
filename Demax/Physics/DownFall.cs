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

