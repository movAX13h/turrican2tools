using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T2Tools
{
    public partial class MainForm : Form
    {
        private Turrican2 game;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            game = new Turrican2("../../../game/T2.exe");

            if (!game.Load())
            {
                MessageBox.Show("Failed to load game data: " + game.Error);
                return;
            }

            // fill TOC list
            
            for(int i = 0; i < game.TOC.Entries.Count; ++i)
            {
                var entry = game.TOC.Entries[i];
                var item = new ListViewItem(i.ToString());
                item.SubItems.Add(entry.Name);
                item.SubItems.Add(entry.Size.ToString());
                listView1.Items.Add(item);
            }
        }
    }
}
