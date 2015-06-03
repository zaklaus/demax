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

            e.RunWindowed(800, 600, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "HelloWorld", OpenTK.GameWindowFlags.FixedWindow);

            CEntity x = e.EntityManager.CreateEntity("cubes");
            x.Transform.Position = new OpenTK.Vector3(0, -6, -20);
            x.AddScript("Scripts/room.py");

            e.Start();
        }
    }
}
