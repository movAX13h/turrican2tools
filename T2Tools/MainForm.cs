using Be.Windows.Forms;
using System;
using System.Text;
using System.Windows.Forms;
using T2Tools.Controls;
using T2Tools.Turrican;

namespace T2Tools
{
    public partial class MainForm : Form
    {
        private Game game;
        private HexBox hexBox;

        public MainForm()
        {
            InitializeComponent();
            fileList.ListViewItemSorter = new ListViewColumnSorter();

            hexBox = new HexBox();
            hexBox.Width = hexPanel.Width;
            hexBox.Height = hexPanel.Height;
            hexBox.GroupSize = 4;
            hexBox.Dock = DockStyle.Fill;
            hexBox.GroupSeparatorVisible = true;
            hexBox.VScrollBarVisible = true;
            hexBox.LineInfoVisible = true;
            hexBox.Font = new System.Drawing.Font("Consolas", 8);
            
            hexBox.SelectionStartChanged += HexBox_SelectionStartChanged;
            hexBox.SelectionLengthChanged += HexBox_SelectionLengthChanged;

            hexEditorPanel.Controls.Add(hexBox);
            hexPanel.Top = 12;
            hexPanel.Visible = false;
            
            txtPanel.Visible = false;
            txtPanel.Top = 12;
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
            foreach (var pair in game.Assets.Entries)
            {
                var entry = pair.Value;
                fileList.Items.Add(new TOCListItem(entry));
            }

            if (fileList.Items.Count > 0) fileList.Items[0].Selected = true;
        }

        #region HEX preview
        private void updateHexSelectionLabel()
        {
            hexSelectionLabel.Text = "Selection offset: " + hexBox.SelectionStart.ToString();
            if (hexBox.SelectionLength > 1) hexSelectionLabel.Text += " (" + hexBox.SelectionLength + " bytes)";
        }

        private void HexBox_SelectionLengthChanged(object sender, EventArgs e)
        {
            updateHexSelectionLabel();
        }

        private void HexBox_SelectionStartChanged(object sender, EventArgs e)
        {
            updateHexSelectionLabel();
        }
        #endregion

        #region TOC List
        private void fileSelected(TOCEntry entry)
        {
            hexPanel.Visible = false;
            txtPanel.Visible = false;

            switch (entry.Type)
            {
                case TOCEntryType.Text:
                case TOCEntryType.Language:                    
                    txtOutput.Text = Encoding.GetEncoding("437").GetString(entry.Data);
                    txtPanel.Visible = true;
                    break;

                case TOCEntryType.Unknown:
                case TOCEntryType.StaticSprite:
                case TOCEntryType.AnimatedSprite:
                case TOCEntryType.Font:
                case TOCEntryType.Palette:
                case TOCEntryType.Music:
                case TOCEntryType.Sound:
                case TOCEntryType.Executable:
                case TOCEntryType.DAT:
                case TOCEntryType.DIR:

                default:
                    hexBox.ByteProvider = new DynamicByteProvider(entry.Data);
                    hexPanel.Visible = true;
                    break;
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

        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileList.SelectedItems.Count == 0) return;

            TOCListItem item = (TOCListItem)(fileList.SelectedItems[0]);
            fileSelected(item.Entry);
        }

        #endregion
    }
}
