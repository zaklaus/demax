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
		}
	}
}

