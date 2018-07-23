﻿using Be.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using T2Tools.Controls;
using T2Tools.Formats;
using T2Tools.Turrican;

namespace T2Tools
{
    public partial class MainForm : Form
    {
        private Game game;
        private HexBox hexBox;
        private TOCListItem selectedItem;

        private Bitmap[] currentBitmaps;
        private int currentBitmapIndex = 0;
        private int currentImgZoom = 3;

        public MainForm()
        {
            InitializeComponent();
            fileList.ListViewItemSorter = new ListViewColumnSorter();

            hexBox = new HexBox();
            hexBox.GroupSize = 4;
            hexBox.Dock = DockStyle.Fill;
            hexBox.GroupSeparatorVisible = true;
            hexBox.VScrollBarVisible = true;
            hexBox.LineInfoVisible = true;
            hexBox.Font = new Font("Consolas", 8);
            hexBox.KeyUp += HexBox_KeyUp;
            
            hexBox.SelectionStartChanged += HexBox_SelectionStartChanged;
            hexBox.SelectionLengthChanged += HexBox_SelectionLengthChanged;

            hexEditorPanel.Controls.Add(hexBox);            
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
            foreach (var entry in game.Assets.Entries.Values)
            {
                fileList.Items.Add(new TOCListItem(entry));
            }

            if (fileList.Items.Count > 0) fileList.Items[0].Selected = true;
        }

        #region files list
        private void fileSelected(TOCListItem item)
        {
            selectedItem = item;
            applyChangesButton.Visible = false;

            var hidePages = new TabPage[] { txtPage, palPage, imgPage };
            foreach(TabPage page in hidePages) if (displayTabs.TabPages.Contains(page)) displayTabs.TabPages.Remove(page);

            currentBitmapIndex = 0;
            if (currentBitmaps != null)
            {
                foreach (Bitmap bmp in currentBitmaps) bmp.Dispose();
                currentBitmaps = null;
            }

            hexBox.ByteProvider = new DynamicByteProvider(item.Entry.Data);

            switch (item.Entry.Type)
            {
                case TOCEntryType.Text:
                case TOCEntryType.Language:                    
                    txtOutput.Text = Encoding.GetEncoding("437").GetString(item.Entry.Data);
                    displayTabs.TabPages.Add(txtPage);
                    displayTabs.SelectedTab = txtPage;
                    break;

                case TOCEntryType.StaticSprite:
                    PCXImage img = new PCXImage();
                    img.Load(item.Entry.Data);
                    currentImgZoom = 3;
                    currentBitmaps = new Bitmap[] { img.Bitmap };
                    displayTabs.TabPages.Add(imgPage);
                    displayTabs.SelectedTab = imgPage;
                    bitmapControlsPanel.Visible = false;
                    break;

                case TOCEntryType.AnimatedSprite:
                    BOBFile file = new BOBFile(item.Entry.Data);
                    BOBDecoder decoder = new BOBDecoder();
                    var vgaBitmaps = decoder.DecodeFrames(file);
                    currentBitmaps = new Bitmap[vgaBitmaps.Count];
                    for (int i = 0; i < vgaBitmaps.Count; i++) currentBitmaps[i] = VGABitmapConverter.ToRGBA(vgaBitmaps[i]);
                    displayTabs.TabPages.Add(imgPage);
                    displayTabs.SelectedTab = imgPage;
                    bitmapControlsPanel.Visible = true;
                    break;

                case TOCEntryType.Palette:
                    currentImgZoom = 14;
                    currentBitmaps = new Bitmap[] { Palette.ToBitmap(item.Entry.Data) };
                    displayTabs.TabPages.Add(imgPage);
                    displayTabs.SelectedTab = imgPage;
                    bitmapControlsPanel.Visible = false;
                    break;

                case TOCEntryType.Unknown:                

                    break;
                case TOCEntryType.PixelFont:
                case TOCEntryType.Music:
                case TOCEntryType.Sound:
                case TOCEntryType.Executable:
                case TOCEntryType.DAT:
                case TOCEntryType.DIR:
                default:                   
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
            fileSelected(item);
        }

        private void writeExeButton_Click(object sender, EventArgs e)
        {
            if (!game.Assets.Dirty)
            {
                MessageBox.Show("No changes made to any file.", "Nothing to do!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (saveExeDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    byte[] data = AssetLoader.GenerateEXE(game.LoadedData.SubArray(0, 12832), game.Assets);
                    File.WriteAllBytes(saveExeDialog.FileName, data);

                    foreach (TOCListItem item in fileList.Items)
                    {
                        item.Entry.Dirty = false;
                        item.Update();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to generate new EXE file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            sectionsPanel.Invalidate();
        }
        #endregion

        #region file regions panel
        private void sectionsPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            if (game.Assets.Entries == null) return;

            SolidBrush brush = new SolidBrush(Color.White);
            Random rand = new Random(123456);

            int start, end;
            float w = sectionsPanel.Width;
            float scale = w / game.NumBytesLoaded;

            foreach (var entry in game.Assets.Entries.Values)
            {
                start = (int)(scale * entry.PackedStart);
                end = (int)(scale * entry.PackedEnd);

                Color col = Color.Red;
                if (!entry.Dirty)
                {
                    int r = 80;
                    int a = r * rand.Next(1, 3);
                    int b = r * rand.Next(1, 3);
                    int c = r * rand.Next(1, 3);
                    col = Color.FromArgb(255, 50 + a, 50 + b, 80 + c);
                }
                brush.Color = col;
                e.Graphics.FillRectangle(brush, start, 0, Math.Max(2, end - start), sectionsPanel.Height);
            }

            if (selectedItem == null) return;

            start = (int)(scale * selectedItem.Entry.PackedStart);
            end = (int)(scale * selectedItem.Entry.PackedEnd);
            brush.Color = Color.Black;
            e.Graphics.FillRectangle(brush, start, 0, Math.Max(3, end - start), sectionsPanel.Height);
        }

        private void sectionsPanel_Resize(object sender, EventArgs e)
        {
            sectionsPanel.Invalidate();
        }
        #endregion
        
        #region HEX editor
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

        private void HexBox_KeyUp(object sender, KeyEventArgs e)
        {
            applyChangesButton.Visible = true;
        }

        private void applyChangesButton_Click(object sender, EventArgs e)
        {
            selectedItem.Entry.Data = (hexBox.ByteProvider as DynamicByteProvider).Bytes.ToArray();
            selectedItem.Entry.Dirty = true;
            selectedItem.Update();
            sectionsPanel.Invalidate();
            applyChangesButton.Visible = false;
        }
        #endregion

        #region image preview
        private void imgPage_Paint(object sender, PaintEventArgs e)
        {
            int x, y, w, h;

            if (currentBitmaps != null)
            {
                Bitmap bmp = currentBitmaps[currentBitmapIndex];

                x = 10;
                y = 10;
                w = bmp.Width * currentImgZoom;
                h = bmp.Height * currentImgZoom;

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                e.Graphics.DrawImage(bmp,
                    new Rectangle(x, y, w, h),
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);

                currentBitmapIndexLabel.Text = (currentBitmapIndex + 1) + "/" + currentBitmaps.Length;
            }
        }
        #endregion

        private void prevBitmapButton_Click(object sender, EventArgs e)
        {
            currentBitmapIndex--;
            if (currentBitmapIndex < 0) currentBitmapIndex = currentBitmaps.Length - 1;
            imgPage.Invalidate();
        }

        private void nextBitmapButton_Click(object sender, EventArgs e)
        {
            currentBitmapIndex++;
            if (currentBitmapIndex > currentBitmaps.Length - 1) currentBitmapIndex = 0;
            imgPage.Invalidate();
        }
    }
}
