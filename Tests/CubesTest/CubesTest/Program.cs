using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demax;
namespace CubesTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World - Demax engine");
            CCore e = new CCore();

            e.RunWindowed(1280, 720, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "HelloWorld", OpenTK.GameWindowFlags.Default);

            CLevel.LoadLevel("Levels/mc.xml");
            CLevel.StartLevel("", "", 0);

            e.Start();
        }
    }
}
