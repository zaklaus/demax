using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Demax;

namespace ModelExporter
{
    class Exporter
    {
        static void Main(string[] args)
        {
            CCore e = new CCore();
            e.RunWindowed(320, 240, new OpenTK.Graphics.GraphicsMode(32, 16, 8, 4), "HelloWorld", OpenTK.GameWindowFlags.Default);

            if (!Directory.Exists("import"))
                Directory.CreateDirectory("import");
            if (!Directory.Exists("export"))
                Directory.CreateDirectory("export");
            if (!Directory.Exists("anim"))
                Directory.CreateDirectory("anim");

            ObjVolume.ExportModelFolder(null, "import", "export");
            ObjVolume.ExportModelFolder(null, "anim", "export", true);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
