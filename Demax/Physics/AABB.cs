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

