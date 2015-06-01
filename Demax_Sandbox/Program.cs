using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demax;

namespace Demax_Sandbox
{
    class Program
    {
        CCore engine;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Init();
        }

        void Init()
        {
            engine = new CCore();
            engine.RunWindowed(1024, 768, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "DeMax Sandbox", OpenTK.GameWindowFlags.Default);


        }
    }
}
