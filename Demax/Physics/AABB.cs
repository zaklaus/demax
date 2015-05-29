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
using OpenTK;
namespace Demax
{
	public static class AABB
	{
		public static int Intersect3D(Vector3[] a, Vector3[] b)
		{
			Vector3[] Min = new Vector3[]{Vector3.Zero,Vector3.Zero};
			Vector3[] Max = new Vector3[]{Vector3.Zero,Vector3.Zero};
			//A
			Min[0].X = a[0].X - a[1].X/2;
			Min[0].Y = a[0].Y - a[1].Y/2;
			Min[0].Z = a[0].Z - a[1].Z/2;

			Max[0].X = a[0].X + a[1].X/2;
			Max[0].Y = a[0].Y + a[1].Y/2;
			Max[0].Z = a[0].Z + a[1].Z/2;

			//B
			Min[1].X = b[0].X - b[1].X/2;
			Min[1].Y = b[0].Y - b[1].Y/2;
			Min[1].Z = b[0].Z - b[1].Z/2;

			Max[1].X = b[0].X + b[1].X/2;
			Max[1].Y = b[0].Y + b[1].Y/2;
			Max[1].Z = b[0].Z + b[1].Z/2;

			if (Max [0].X >= Min [1].X && Min [0].X <= Max [1].X &&
				Max [0].Y >= Min [1].Y && Min [0].Y <= Max [1].Y &&
			   Min [0].Z <= Max [1].Z && Max [0].Z >= Min [1].Z) {
				//Console.WriteLine ("AABB");
				return 1;
			}
			//} else if ()
			//	return 2;
			return 0;
		}

	}
}

