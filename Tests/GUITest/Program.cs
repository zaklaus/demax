using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demax;

namespace GUITest
{
    class Program
    {
        static void Main(string[] args)
        {
            CCore e = new CCore();
            e.RunWindowed(800, 600, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "GUI Test", OpenTK.GameWindowFlags.FixedWindow);
            
            var c = e.EntityManager.CreateEntity("gui");
            c.AddScript("gui.py");

            e.Start();
        }
    }
}
