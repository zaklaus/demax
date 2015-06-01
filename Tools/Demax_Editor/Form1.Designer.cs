namespace Demax_Editor
{
    partial class mainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.game = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // game
            // 
            this.game.BackColor = System.Drawing.Color.Black;
            this.game.Location = new System.Drawing.Point(12, 59);
            this.game.Name = "game";
            this.game.Size = new System.Drawing.Size(984, 658);
            this.game.TabIndex = 0;
            this.game.VSync = false;
            this.game.Load += new System.EventHandler(this.game_Load);
            this.game.Paint += new System.Windows.Forms.PaintEventHandler(this.game_Paint);
            this.game.KeyDown += new System.Windows.Forms.KeyEventHandler(this.game_KeyDown);
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.game);
            this.Name = "mainWindow";
            this.Text = "Demax Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl game;
    }
}

