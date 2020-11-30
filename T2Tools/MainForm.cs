using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using T2Tools.Controls;
using T2Tools.Formats;
using T2Tools.Turrican;
using T2Tools.Utils;

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

        private MapMaker mapMaker;
        private TilemapMaker tilemapMaker;

        public MainForm()
        {
            InitializeComponent();
            fileList.ListViewItemSorter = new ListViewColumnSorter();

            sectionsPanel.DoubleBuffered(true);
            imgPage.DoubleBuffered(true);

            createHexEditor();        
            this.AllowDrop = true;
              this.DragEnter += new DragEventHandler(MainForm_DragEnter);
              this.DragDrop += new DragEventHandler(MainForm_DragDrop);
        }
         void MainForm_DragEnter(object sender, DragEventArgs e) {
          if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void MainForm_DragDrop(object sender, DragEventArgs e) {
          string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
          this.initForm(files[0]); // open first file
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.initForm("../../../game/T2.exe");
        }
        private void initForm(string filename)
        {
            #if DEBUG
            game = new Game(filename);
            #else
            game = new Game("T2.exe");
            #endif

            if (!game.Load())
            {
                MessageBox.Show("Failed to load game data: " + game.Error);
                return;
            }

            this.Text = "Turrican II Tools - " + filename;

            tilemapMaker = new TilemapMaker(game.Assets);

            // fill TOC list
            fileList.Items.Clear();
            foreach (var entry in game.Assets.Entries.Values)
            {
                fileList.Items.Add(new TOCListItem(entry));
            }

            // sort list by type and select first item
            if (fileList.Items.Count > 0)
            {
                ListViewColumnSorter sorter = fileList.ListViewItemSorter as ListViewColumnSorter;
                sorter.SortColumn = 1;
                sorter.Order = SortOrder.Ascending;
                fileList.Sort();
                fileList.Items[0].Selected = true;
            }
        }

#region files list
        private void fileSelected(TOCListItem item)
        {
            selectedItem = item;
            applyChangesButton.Visible = false;

            var hidePages = new TabPage[] { txtPage, imgPage, infoPage, mapPage, tfmxPage, tilesPage, entitiesPage };
            foreach(TabPage page in hidePages) if (previewTabs.TabPages.Contains(page)) previewTabs.TabPages.Remove(page);

            currentBitmapIndex = 0;
            if (currentBitmaps != null)
            {
                foreach (Bitmap bmp in currentBitmaps) bmp.Dispose();
                currentBitmaps = null;
            }

            mapMaker?.Cancel();

            if (mapPictureBox.Image != null)
            {
                var img = mapPictureBox.Image;                
                mapPictureBox.Image = null;
                img.Dispose();
            }

            hexBox.ByteProvider = new DynamicByteProvider(item.Entry.Data);

            bool preview = true;
            Description description = game.Descriptions?.ByName(item.Entry.Name);
            if (description != null && description.NoPreview) preview = false;

            if (preview)
            {
                switch (item.Entry.Type)
                {
                    case TOCEntryType.Text:
                    case TOCEntryType.Language:
                        txtOutput.Text = Encoding.GetEncoding("437").GetString(item.Entry.Data);
                        previewTabs.TabPages.Add(txtPage);
                        previewTabs.SelectedTab = txtPage;
                        break;

                    case TOCEntryType.StaticSprite:
                        PCXFile img = new PCXFile();
                        img.Load(item.Entry.Data);
                        currentImgZoom = 3;
                        imgZoomInput.Value = currentImgZoom;
                        currentBitmaps = new Bitmap[] { img.Bitmap };
                        imgPage.Text = "Sprite";
                        previewTabs.TabPages.Add(imgPage);
                        previewTabs.SelectedTab = imgPage;
                        bitmapControlsPanel.Visible = false;
                        updateImagePreview();
                        break;
                    case TOCEntryType.AnimatedSprite:
                        BOBFile bobFile = new BOBFile(item.Entry.Data);
                        BOBDecoder decoder = new BOBDecoder();
                        var vgaBitmaps = decoder.DecodeFrames(bobFile);
                        currentBitmaps = new Bitmap[vgaBitmaps.Count];
                        imgPage.Text = "Sprite Animation";
                        for (int i = 0; i < vgaBitmaps.Count; i++) currentBitmaps[i] = VGABitmapConverter.ToRGBA(vgaBitmaps[i]);
                        currentImgZoom = 10;
                        imgZoomInput.Value = currentImgZoom;
                        previewTabs.TabPages.Add(imgPage);
                        previewTabs.SelectedTab = imgPage;
                        bitmapControlsPanel.Visible = true;
                        updateImagePreview();
                        break;

                    case TOCEntryType.Palette:
                        currentImgZoom = 14;
                        imgZoomInput.Value = currentImgZoom;
                        currentBitmaps = new Bitmap[] { Palette.ToBitmap(item.Entry.Data) };
                        imgPage.Text = "Palette";
                        previewTabs.TabPages.Add(imgPage);
                        previewTabs.SelectedTab = imgPage;
                        bitmapControlsPanel.Visible = false;
                        updateImagePreview();
                        break;

                    case TOCEntryType.Tileset:
                    case TOCEntryType.CollisionInfo:
                        tilesCollisionsCheckbox.Checked = item.Entry.Type == TOCEntryType.CollisionInfo;
                        Bitmap tilesetBitmap = tilemapMaker.MakeTilesetBitmap(item.Entry, tilesCollisionsCheckbox.Checked);
                        if (tilesetBitmap != null)
                        {
                            tilesPictureBox.Image = tilesetBitmap;
                            previewTabs.TabPages.Add(tilesPage);
                            previewTabs.SelectedTab = tilesPage;
                        }
                        else MessageBox.Show("Error: Failed to generate tileset preview!");
                        break;

                    case TOCEntryType.Map:
                        previewTabs.TabPages.Add(mapPage);
                        previewTabs.SelectedTab = mapPage;                        
                        mapMakerProgressBar.Value = 0;
                        mapMakerProgressPanel.Visible = true;
                        mapMaker = new MapMaker(game.Assets, mapProgress, mapComplete);
                        if (!mapMaker.Make(item.Entry)) MessageBox.Show("Error: " + mapMaker.Error, "Failed to generate preview!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case TOCEntryType.Music:
                        previewTabs.TabPages.Add(tfmxPage);
                        previewTabs.SelectedTab = tfmxPage;
                        playSelectedTFM();
                        break;

                    case TOCEntryType.EntitiesList:
                        previewTabs.TabPages.Add(entitiesPage);
                        previewTabs.SelectedTab = entitiesPage;
                        entitiesList.Items.Clear();
                        try
                        {
                            EIBFile eibFile = new EIBFile(item.Entry.Data);
                            foreach (var entry in eibFile.Regions)
                            {
                                foreach (var point in entry.Points)
                                {
                                    entitiesList.Items.Add(new EntityListItem(point));
                                }
                            }
                            entityFileInfo.Text = $"Unknown variables in file: D={eibFile.D}, E={eibFile.E}, F={eibFile.F}";
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    case TOCEntryType.PixelFont:
                    case TOCEntryType.Sound:
                        case TOCEntryType.DIR:
                            case TOCEntryType.DAT:
                    case TOCEntryType.Executable:
                    case TOCEntryType.Unknown:
                    default:
                        break;
                }
            }

            if (description != null && !string.IsNullOrEmpty(description.Info))
            {
                infoOutput.Text = description.Info;
                previewTabs.TabPages.Add(infoPage);
                if (previewTabs.TabPages.Count == 1) previewTabs.SelectedTab = infoPage;
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
            fileList.Focus();
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

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (exportDialog.ShowDialog() != DialogResult.OK) return;
            int numSavedFiled = game.Assets.ExportTo(exportDialog.SelectedPath);
            MessageBox.Show($"Successfully saved {numSavedFiled} files to '{exportDialog.SelectedPath}'.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            bool toggle = false;

            foreach (var entry in game.Assets.Entries.Values)
            {
                start = (int)(scale * entry.PackedStart);
                end = (int)(scale * entry.PackedEnd);

                Color col = Color.Red;
                if (!entry.Dirty)
                {
                    /*int r = 80;
                    int a = r * rand.Next(1, 3);
                    int b = r * rand.Next(1, 3);
                    int c = r * rand.Next(1, 3);
                    col = Color.FromArgb(255, 50 + a, 50 + b, 80 + c);
                    */
                    col = toggle ? Color.LightGray : Color.Gray;
                    toggle = !toggle;
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
        private void createHexEditor()
        {
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
        private void updateImagePreview()
        {
            int w, h;

            if (currentBitmaps != null)
            {
                saveFrameButton.Visible = true;
                saveStripeButton.Visible = currentBitmaps.Length > 1;

                Bitmap bmp = currentBitmaps[currentBitmapIndex];

                w = bmp.Width * currentImgZoom;
                h = bmp.Height * currentImgZoom;

                Bitmap target = new Bitmap(w, h);
                using (Graphics gfx = Graphics.FromImage(target))
                {
                    gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    gfx.DrawImage(bmp,
                        new Rectangle(0, 0, w, h),
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        GraphicsUnit.Pixel);

                    imgPictureBox.Image = target;
                }
                currentBitmapIndexLabel.Text = (currentBitmapIndex + 1) + "/" + currentBitmaps.Length;
            }
        }

        private void prevBitmapButton_Click(object sender, EventArgs e)
        {
            currentBitmapIndex--;
            if (currentBitmapIndex < 0) currentBitmapIndex = currentBitmaps.Length - 1;
            updateImagePreview();
        }

        private void nextBitmapButton_Click(object sender, EventArgs e)
        {
            currentBitmapIndex++;
            if (currentBitmapIndex > currentBitmaps.Length - 1) currentBitmapIndex = 0;
            updateImagePreview();
        }

        private void imgZoomInput_Scroll(object sender, EventArgs e)
        {
            currentImgZoom = imgZoomInput.Value;
            updateImagePreview();
        }

        private void saveFrameButton_Click(object sender, EventArgs e)
        {
            saveImageDialog.FileName = selectedItem.Entry.Name + ".png";
            if (saveImageDialog.ShowDialog() != DialogResult.OK) return;
            currentBitmaps[currentBitmapIndex].Save(saveImageDialog.FileName);
        }

        private void saveStripeButton_Click(object sender, EventArgs e)
        {
            saveImageDialog.FileName = selectedItem.Entry.Name + ".png";
            if (saveImageDialog.ShowDialog() != DialogResult.OK) return;
            SpritesheetMaker.Make(currentBitmaps).Save(saveImageDialog.FileName);
        }
#endregion

        #region map preview
        private void mapProgress(int progress)
        {
            mapMakerProgressBar.Value = progress;
        }

        private void mapComplete(bool success)
        {
            mapMakerProgressPanel.Visible = false;

            if (!success)
            {
                if (string.IsNullOrEmpty(mapMaker.Error)) return;
                MessageBox.Show("Error: " + mapMaker.Error, "Failed to generate preview!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            updateMapPreview();
        }

        private void updateMapPreview()
        {
            if (mapCollisionsCheckbox.Checked || entitiesCheckbox.Checked || mapGridCheckbox.Checked)
            {
                int w = mapMaker.TilesBitmap.Width;
                int h = mapMaker.TilesBitmap.Height;
                var bmp = new Bitmap(w, h);
                using (var gfx = Graphics.FromImage(bmp))
                {
                    gfx.DrawImage(mapMaker.TilesBitmap, 0, 0, w, h);
                    if (mapCollisionsCheckbox.Checked) gfx.DrawImage(mapMaker.CollisionsBitmap, 0, 0, w, h);
                    if (entitiesCheckbox.Checked)      gfx.DrawImage(mapMaker.EntitiesBitmap, 0, 0, w, h);
                    if (mapGridCheckbox.Checked)       gfx.DrawImage(mapMaker.GridBitmap, 0, 0, w, h);
                }

                mapPictureBox.Image = bmp;
            }
            else mapPictureBox.Image = mapMaker.TilesBitmap;

            mapDetailsLabel.Text = $"Size: {mapMaker.Map.Width}x{mapMaker.Map.Height}";

            GC.Collect(); // removes unused bitmaps from memory
        }

        private void mapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            updateMapPreview();
        }
        #endregion

        #region music
        private void tfmxPlayButton_Click(object sender, EventArgs e)
        {
            playSelectedTFM();
        }

        private void playSelectedTFM()
        { 
            string samName = Path.ChangeExtension(selectedItem.Entry.Name, ".SAM");
            if (!game.Assets.Entries.ContainsKey(samName))
            {
                MessageBox.Show($"Sample file '{samName}' not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TOCEntry samples = game.Assets.Entries[samName];

            TFXTool.PlayerForm form = new TFXTool.PlayerForm(selectedItem.Entry.Name, selectedItem.Entry.Data, samples.Data);
            form.ShowDialog(this);
        }
#endregion

        #region tiles preview
        private void tilesCollisionsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            tilesPictureBox.Image = tilemapMaker.MakeTilesetBitmap(selectedItem.Entry, tilesCollisionsCheckbox.Checked);
        }

        private void saveTilesetButton_Click(object sender, EventArgs e)
        {
            saveImageDialog.FileName = selectedItem.Entry.Name + ".png";
            if (saveImageDialog.ShowDialog() != DialogResult.OK) return;
            tilesPictureBox.Image.Save(saveImageDialog.FileName);
        }


        #endregion

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                //InitialDirectory = @"D:\",
                Title = "Browse Turrican EXE files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "exe",
                Filter = "Executables (*.exe)|*.exe",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.initForm(openFileDialog1.FileName) ;
            }
        }
    }
}
