using Be.Windows.Forms;
using System;
using System.Drawing;
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
        private TOCEntry selectedEntry;

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
            hexPanel.Dock = DockStyle.Fill;
            hexPanel.Visible = false;
            
            txtPanel.Visible = false;
            txtPanel.Dock = DockStyle.Fill;
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
            selectedEntry = entry;
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
                case TOCEntryType.PixelFont:
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

            sectionsPanel.Invalidate();
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

        private void sectionsPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            if (game.Assets.Entries == null) return;

            SolidBrush brush = new SolidBrush(Color.White);
            Random rand = new Random(123456);

            int start, end;
            float w = sectionsPanel.Width;
            float scale = w / game.TotalSize;

            foreach (var pair in game.Assets.Entries)
            {
                TOCEntry entry = pair.Value;

                start = (int)(scale * entry.PackedStart);
                end = (int)(scale * entry.PackedEnd);

                int r = 80;
                int a = r * rand.Next(1, 3);
                int b = r * rand.Next(1, 3);
                int c = r * rand.Next(1, 3);
                brush.Color = Color.FromArgb(255, 50 + a, 50 + b, 80 + c);

                e.Graphics.FillRectangle(brush, start, 0, Math.Max(2, end - start), sectionsPanel.Height);
            }

            if (selectedEntry == null) return;

            start = (int)(scale * selectedEntry.PackedStart);
            end = (int)(scale * selectedEntry.PackedEnd);
            brush.Color = Color.Black;
            e.Graphics.FillRectangle(brush, start, 0, Math.Max(3, end - start), sectionsPanel.Height);
            //Pen pen = new Pen(Color.Black, 2);
            //e.Graphics.DrawLine(pen, pos, 0, pos, sectionsPanel.Height);
        }

        private void sectionsPanel_Resize(object sender, EventArgs e)
        {
            sectionsPanel.Invalidate();
        }
    }
}
