using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Demax;

namespace Demax_Editor
{
    public partial class mainWindow : Form
    {
        CCore engine;
        bool loaded = false;
        double elapsed = 0;
        public mainWindow()
        {
            InitializeComponent();

            engine = new CCore();

            engine.RunInline(game);
            engine.Start();
        }

        private void game_KeyDown(object sender, KeyEventArgs e)
        {
            engine.InputManager.OnKeyDown(sender, new OpenTK.Input.KeyboardKeyEventArgs(e));
        }

        private void game_Load(object sender, EventArgs e)
        {
            loaded = true;
            engine.Renderer.OnLoad(sender, new EventArgs());
        }

        private void game_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;
            elapsed++;
            engine.Renderer.OnRenderFrame(sender, new OpenTK.FrameEventArgs(elapsed));
        }
    }
}
