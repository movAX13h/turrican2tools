using System;
using System.Windows.Forms;
using T2Tools.Controls;
using T2Tools.Turrican;

namespace T2Tools
{
    public partial class MainForm : Form
    {
        private Game game;

        public MainForm()
        {
            InitializeComponent();
            fileList.ListViewItemSorter = new ListViewColumnSorter();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            game = new Game("../../../game/T2.exe");

            if (!game.Load())
            {
                MessageBox.Show("Failed to load game data: " + game.Error);
                return;
            }

            // fill TOC list
            foreach(var pair in game.Assets.Entries)
            {
                var entry = pair.Value;
                fileList.Items.Add(new TOCListItem(entry));
            }
        }

        private void tocList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnSorter sorter = fileList.ListViewItemSorter as ListViewColumnSorter;

            if (e.Column == sorter.SortColumn)
            {
                if (sorter.Order == SortOrder.Ascending)
                {
                    sorter.Order = SortOrder.Descending;
                }
                else
                {
                    sorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            fileList.Sort();
        }
    }
}
