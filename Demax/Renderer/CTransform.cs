using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Demax
{
	public class CTransform
	{
		public Vector3 Position, Rotation, Scale;

		public CTransform (Vector3 pos, Vector3 rot, Vector3 sca)
		{
			Position = pos;
			Rotation = rot;
			Scale = sca;
		}

		public static CTransform operator +(CTransform left, CTransform right)
		{
			return new CTransform (left.Position + right.Position, left.Rotation + right.Rotation, left.Scale + right.Scale);
		}
	}
}

