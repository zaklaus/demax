using System;
using Demax;

namespace HelloWorld
{
	public class MainClass
	{
		[STAThread]
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World - Demax engine");
			CCore e = new CCore ();

            e.RunWindowed(800, 600, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "HelloWorld", OpenTK.GameWindowFlags.FixedWindow);
		}
	}
}

