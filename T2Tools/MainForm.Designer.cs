namespace T2Tools
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.fileList = new System.Windows.Forms.ListView();
            this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.endHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hexEditorPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.applyChangesButton = new System.Windows.Forms.Button();
            this.hexSelectionLabel = new System.Windows.Forms.Label();
            this.sectionsPanel = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.previewTabs = new System.Windows.Forms.TabControl();
            this.hexPage = new System.Windows.Forms.TabPage();
            this.txtPage = new System.Windows.Forms.TabPage();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.palPage = new System.Windows.Forms.TabPage();
            this.imgPage = new System.Windows.Forms.TabPage();
            this.imgZoomInput = new System.Windows.Forms.TrackBar();
            this.bitmapControlsPanel = new System.Windows.Forms.Panel();
            this.prevBitmapButton = new System.Windows.Forms.Button();
            this.nextBitmapButton = new System.Windows.Forms.Button();
            this.currentBitmapIndexLabel = new System.Windows.Forms.Label();
            this.infoPage = new System.Windows.Forms.TabPage();
            this.infoOutput = new System.Windows.Forms.TextBox();
            this.mapPage = new System.Windows.Forms.TabPage();
            this.mapDetailsLabel = new System.Windows.Forms.Label();
            this.mapCollisionsCheckbox = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mapMakerProgressPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.mapMakerProgressBar = new System.Windows.Forms.ProgressBar();
            this.mapPictureBox = new System.Windows.Forms.PictureBox();
            this.tfmxPage = new System.Windows.Forms.TabPage();
            this.tfmxPlayButton = new System.Windows.Forms.Button();
            this.writeExeButton = new System.Windows.Forms.Button();
            this.saveExeDialog = new System.Windows.Forms.SaveFileDialog();
            this.tilesPage = new System.Windows.Forms.TabPage();
            this.tilesPictureBox = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tilesCollisionsCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.previewTabs.SuspendLayout();
            this.hexPage.SuspendLayout();
            this.txtPage.SuspendLayout();
            this.imgPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgZoomInput)).BeginInit();
            this.bitmapControlsPanel.SuspendLayout();
            this.infoPage.SuspendLayout();
            this.mapPage.SuspendLayout();
            this.panel2.SuspendLayout();
            this.mapMakerProgressPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).BeginInit();
            this.tfmxPage.SuspendLayout();
            this.tilesPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilesPictureBox)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileList
            // 
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.typeHeader,
            this.sizeHeader,
            this.startHeader,
            this.endHeader});
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileList.FullRowSelect = true;
            this.fileList.HideSelection = false;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.MultiSelect = false;
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(425, 521);
            this.fileList.TabIndex = 0;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
            this.fileList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.tocList_ColumnClick);
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
            // 
            // nameHeader
            // 
            this.nameHeader.Text = "Name";
            this.nameHeader.Width = 101;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Type";
            this.typeHeader.Width = 105;
            // 
            // sizeHeader
            // 
            this.sizeHeader.Text = "Size";
            this.sizeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // startHeader
            // 
            this.startHeader.Text = "Start";
            this.startHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.startHeader.Width = 65;
            // 
            // endHeader
            // 
            this.endHeader.Text = "End";
            this.endHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.endHeader.Width = 66;
            // 
            // hexEditorPanel
            // 
            this.hexEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEditorPanel.Location = new System.Drawing.Point(3, 3);
            this.hexEditorPanel.Name = "hexEditorPanel";
            this.hexEditorPanel.Size = new System.Drawing.Size(653, 464);
            this.hexEditorPanel.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.applyChangesButton);
            this.panel1.Controls.Add(this.hexSelectionLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 467);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(653, 25);
            this.panel1.TabIndex = 0;
            // 
            // applyChangesButton
            // 
            this.applyChangesButton.Location = new System.Drawing.Point(-1, 3);
            this.applyChangesButton.Name = "applyChangesButton";
            this.applyChangesButton.Size = new System.Drawing.Size(120, 23);
            this.applyChangesButton.TabIndex = 1;
            this.applyChangesButton.Text = "APPLY CHANGES";
            this.applyChangesButton.UseVisualStyleBackColor = true;
            this.applyChangesButton.Visible = false;
            this.applyChangesButton.Click += new System.EventHandler(this.applyChangesButton_Click);
            // 
            // hexSelectionLabel
            // 
            this.hexSelectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.hexSelectionLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexSelectionLabel.Location = new System.Drawing.Point(30, 4);
            this.hexSelectionLabel.Name = "hexSelectionLabel";
            this.hexSelectionLabel.Size = new System.Drawing.Size(614, 17);
            this.hexSelectionLabel.TabIndex = 0;
            this.hexSelectionLabel.Text = "no selection";
            this.hexSelectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sectionsPanel
            // 
            this.sectionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sectionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sectionsPanel.Location = new System.Drawing.Point(6, 6);
            this.sectionsPanel.Name = "sectionsPanel";
            this.sectionsPanel.Size = new System.Drawing.Size(1036, 23);
            this.sectionsPanel.TabIndex = 3;
            this.sectionsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.sectionsPanel_Paint);
            this.sectionsPanel.Resize += new System.EventHandler(this.sectionsPanel_Resize);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(6, 34);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.previewTabs);
            this.splitContainer1.Size = new System.Drawing.Size(1096, 521);
            this.splitContainer1.SplitterDistance = 425;
            this.splitContainer1.TabIndex = 4;
            // 
            // previewTabs
            // 
            this.previewTabs.Controls.Add(this.hexPage);
            this.previewTabs.Controls.Add(this.txtPage);
            this.previewTabs.Controls.Add(this.palPage);
            this.previewTabs.Controls.Add(this.imgPage);
            this.previewTabs.Controls.Add(this.infoPage);
            this.previewTabs.Controls.Add(this.mapPage);
            this.previewTabs.Controls.Add(this.tfmxPage);
            this.previewTabs.Controls.Add(this.tilesPage);
            this.previewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewTabs.Location = new System.Drawing.Point(0, 0);
            this.previewTabs.Name = "previewTabs";
            this.previewTabs.SelectedIndex = 0;
            this.previewTabs.Size = new System.Drawing.Size(667, 521);
            this.previewTabs.TabIndex = 5;
            // 
            // hexPage
            // 
            this.hexPage.Controls.Add(this.hexEditorPanel);
            this.hexPage.Controls.Add(this.panel1);
            this.hexPage.Location = new System.Drawing.Point(4, 22);
            this.hexPage.Name = "hexPage";
            this.hexPage.Padding = new System.Windows.Forms.Padding(3);
            this.hexPage.Size = new System.Drawing.Size(659, 495);
            this.hexPage.TabIndex = 0;
            this.hexPage.Text = "HEX Editor";
            this.hexPage.UseVisualStyleBackColor = true;
            // 
            // txtPage
            // 
            this.txtPage.Controls.Add(this.txtOutput);
            this.txtPage.Location = new System.Drawing.Point(4, 22);
            this.txtPage.Name = "txtPage";
            this.txtPage.Padding = new System.Windows.Forms.Padding(3);
            this.txtPage.Size = new System.Drawing.Size(659, 495);
            this.txtPage.TabIndex = 1;
            this.txtPage.Text = "Text";
            this.txtPage.UseVisualStyleBackColor = true;
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(3, 3);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(653, 489);
            this.txtOutput.TabIndex = 0;
            // 
            // palPage
            // 
            this.palPage.Location = new System.Drawing.Point(4, 22);
            this.palPage.Name = "palPage";
            this.palPage.Padding = new System.Windows.Forms.Padding(3);
            this.palPage.Size = new System.Drawing.Size(659, 495);
            this.palPage.TabIndex = 2;
            this.palPage.Text = "Palette";
            this.palPage.UseVisualStyleBackColor = true;
            // 
            // imgPage
            // 
            this.imgPage.Controls.Add(this.imgZoomInput);
            this.imgPage.Controls.Add(this.bitmapControlsPanel);
            this.imgPage.Location = new System.Drawing.Point(4, 22);
            this.imgPage.Name = "imgPage";
            this.imgPage.Padding = new System.Windows.Forms.Padding(3);
            this.imgPage.Size = new System.Drawing.Size(659, 495);
            this.imgPage.TabIndex = 3;
            this.imgPage.Text = "Image";
            this.imgPage.UseVisualStyleBackColor = true;
            this.imgPage.Paint += new System.Windows.Forms.PaintEventHandler(this.imgPage_Paint);
            // 
            // imgZoomInput
            // 
            this.imgZoomInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.imgZoomInput.AutoSize = false;
            this.imgZoomInput.BackColor = System.Drawing.Color.White;
            this.imgZoomInput.Location = new System.Drawing.Point(241, 352);
            this.imgZoomInput.Maximum = 14;
            this.imgZoomInput.Minimum = 1;
            this.imgZoomInput.Name = "imgZoomInput";
            this.imgZoomInput.Size = new System.Drawing.Size(104, 26);
            this.imgZoomInput.TabIndex = 1;
            this.imgZoomInput.TickStyle = System.Windows.Forms.TickStyle.None;
            this.imgZoomInput.Value = 3;
            this.imgZoomInput.Scroll += new System.EventHandler(this.imgZoomInput_Scroll);
            // 
            // bitmapControlsPanel
            // 
            this.bitmapControlsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bitmapControlsPanel.Controls.Add(this.prevBitmapButton);
            this.bitmapControlsPanel.Controls.Add(this.nextBitmapButton);
            this.bitmapControlsPanel.Controls.Add(this.currentBitmapIndexLabel);
            this.bitmapControlsPanel.Location = new System.Drawing.Point(6, 349);
            this.bitmapControlsPanel.Name = "bitmapControlsPanel";
            this.bitmapControlsPanel.Size = new System.Drawing.Size(135, 29);
            this.bitmapControlsPanel.TabIndex = 0;
            // 
            // prevBitmapButton
            // 
            this.prevBitmapButton.Location = new System.Drawing.Point(3, 3);
            this.prevBitmapButton.Name = "prevBitmapButton";
            this.prevBitmapButton.Size = new System.Drawing.Size(29, 23);
            this.prevBitmapButton.TabIndex = 1;
            this.prevBitmapButton.Text = "<";
            this.prevBitmapButton.UseVisualStyleBackColor = true;
            this.prevBitmapButton.Click += new System.EventHandler(this.prevBitmapButton_Click);
            // 
            // nextBitmapButton
            // 
            this.nextBitmapButton.Location = new System.Drawing.Point(102, 3);
            this.nextBitmapButton.Name = "nextBitmapButton";
            this.nextBitmapButton.Size = new System.Drawing.Size(29, 23);
            this.nextBitmapButton.TabIndex = 1;
            this.nextBitmapButton.Text = ">";
            this.nextBitmapButton.UseVisualStyleBackColor = true;
            this.nextBitmapButton.Click += new System.EventHandler(this.nextBitmapButton_Click);
            // 
            // currentBitmapIndexLabel
            // 
            this.currentBitmapIndexLabel.Location = new System.Drawing.Point(38, 3);
            this.currentBitmapIndexLabel.Name = "currentBitmapIndexLabel";
            this.currentBitmapIndexLabel.Size = new System.Drawing.Size(58, 23);
            this.currentBitmapIndexLabel.TabIndex = 0;
            this.currentBitmapIndexLabel.Text = "0/0";
            this.currentBitmapIndexLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // infoPage
            // 
            this.infoPage.Controls.Add(this.infoOutput);
            this.infoPage.Location = new System.Drawing.Point(4, 22);
            this.infoPage.Name = "infoPage";
            this.infoPage.Padding = new System.Windows.Forms.Padding(3);
            this.infoPage.Size = new System.Drawing.Size(659, 495);
            this.infoPage.TabIndex = 4;
            this.infoPage.Text = "Information";
            this.infoPage.UseVisualStyleBackColor = true;
            // 
            // infoOutput
            // 
            this.infoOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoOutput.Location = new System.Drawing.Point(3, 3);
            this.infoOutput.Multiline = true;
            this.infoOutput.Name = "infoOutput";
            this.infoOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infoOutput.Size = new System.Drawing.Size(653, 489);
            this.infoOutput.TabIndex = 1;
            // 
            // mapPage
            // 
            this.mapPage.Controls.Add(this.panel2);
            this.mapPage.Controls.Add(this.mapDetailsLabel);
            this.mapPage.Controls.Add(this.mapCollisionsCheckbox);
            this.mapPage.Location = new System.Drawing.Point(4, 22);
            this.mapPage.Name = "mapPage";
            this.mapPage.Size = new System.Drawing.Size(659, 495);
            this.mapPage.TabIndex = 5;
            this.mapPage.Text = "Map";
            this.mapPage.UseVisualStyleBackColor = true;
            // 
            // mapDetailsLabel
            // 
            this.mapDetailsLabel.Location = new System.Drawing.Point(491, 15);
            this.mapDetailsLabel.Name = "mapDetailsLabel";
            this.mapDetailsLabel.Size = new System.Drawing.Size(153, 23);
            this.mapDetailsLabel.TabIndex = 5;
            this.mapDetailsLabel.Text = "Size...";
            this.mapDetailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mapCollisionsCheckbox
            // 
            this.mapCollisionsCheckbox.AutoSize = true;
            this.mapCollisionsCheckbox.Location = new System.Drawing.Point(22, 20);
            this.mapCollisionsCheckbox.Name = "mapCollisionsCheckbox";
            this.mapCollisionsCheckbox.Size = new System.Drawing.Size(69, 17);
            this.mapCollisionsCheckbox.TabIndex = 4;
            this.mapCollisionsCheckbox.Text = "Collisions";
            this.mapCollisionsCheckbox.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.mapMakerProgressPanel);
            this.panel2.Controls.Add(this.mapPictureBox);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(659, 495);
            this.panel2.TabIndex = 3;
            // 
            // mapMakerProgressPanel
            // 
            this.mapMakerProgressPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mapMakerProgressPanel.Controls.Add(this.label1);
            this.mapMakerProgressPanel.Controls.Add(this.mapMakerProgressBar);
            this.mapMakerProgressPanel.Location = new System.Drawing.Point(202, 195);
            this.mapMakerProgressPanel.Name = "mapMakerProgressPanel";
            this.mapMakerProgressPanel.Size = new System.Drawing.Size(275, 73);
            this.mapMakerProgressPanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Generating map preview ...";
            // 
            // mapMakerProgressBar
            // 
            this.mapMakerProgressBar.Location = new System.Drawing.Point(18, 31);
            this.mapMakerProgressBar.Name = "mapMakerProgressBar";
            this.mapMakerProgressBar.Size = new System.Drawing.Size(241, 23);
            this.mapMakerProgressBar.Step = 1;
            this.mapMakerProgressBar.TabIndex = 0;
            // 
            // mapPictureBox
            // 
            this.mapPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mapPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.mapPictureBox.Name = "mapPictureBox";
            this.mapPictureBox.Size = new System.Drawing.Size(339, 372);
            this.mapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.mapPictureBox.TabIndex = 2;
            this.mapPictureBox.TabStop = false;
            // 
            // tfmxPage
            // 
            this.tfmxPage.Controls.Add(this.tfmxPlayButton);
            this.tfmxPage.Location = new System.Drawing.Point(4, 22);
            this.tfmxPage.Name = "tfmxPage";
            this.tfmxPage.Padding = new System.Windows.Forms.Padding(3);
            this.tfmxPage.Size = new System.Drawing.Size(659, 495);
            this.tfmxPage.TabIndex = 6;
            this.tfmxPage.Text = "Music";
            this.tfmxPage.UseVisualStyleBackColor = true;
            // 
            // tfmxPlayButton
            // 
            this.tfmxPlayButton.Location = new System.Drawing.Point(25, 25);
            this.tfmxPlayButton.Name = "tfmxPlayButton";
            this.tfmxPlayButton.Size = new System.Drawing.Size(75, 23);
            this.tfmxPlayButton.TabIndex = 0;
            this.tfmxPlayButton.Text = "PLAY";
            this.tfmxPlayButton.UseVisualStyleBackColor = true;
            this.tfmxPlayButton.Click += new System.EventHandler(this.tfmxPlayButton_Click);
            // 
            // writeExeButton
            // 
            this.writeExeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.writeExeButton.Location = new System.Drawing.Point(1048, 6);
            this.writeExeButton.Name = "writeExeButton";
            this.writeExeButton.Size = new System.Drawing.Size(54, 23);
            this.writeExeButton.TabIndex = 5;
            this.writeExeButton.Text = "WRITE";
            this.writeExeButton.UseVisualStyleBackColor = true;
            this.writeExeButton.Click += new System.EventHandler(this.writeExeButton_Click);
            // 
            // saveExeDialog
            // 
            this.saveExeDialog.FileName = "T2mod.EXE";
            this.saveExeDialog.Filter = "EXE Files (*.exe)|*.exe|All Files|*.*";
            this.saveExeDialog.RestoreDirectory = true;
            // 
            // tilesPage
            // 
            this.tilesPage.Controls.Add(this.label2);
            this.tilesPage.Controls.Add(this.tilesCollisionsCheckbox);
            this.tilesPage.Controls.Add(this.panel3);
            this.tilesPage.Location = new System.Drawing.Point(4, 22);
            this.tilesPage.Name = "tilesPage";
            this.tilesPage.Size = new System.Drawing.Size(659, 495);
            this.tilesPage.TabIndex = 7;
            this.tilesPage.Text = "Tiles";
            this.tilesPage.UseVisualStyleBackColor = true;
            // 
            // tilesPictureBox
            // 
            this.tilesPictureBox.Location = new System.Drawing.Point(0, 0);
            this.tilesPictureBox.Name = "tilesPictureBox";
            this.tilesPictureBox.Size = new System.Drawing.Size(100, 50);
            this.tilesPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tilesPictureBox.TabIndex = 0;
            this.tilesPictureBox.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.Controls.Add(this.tilesPictureBox);
            this.panel3.Location = new System.Drawing.Point(0, 59);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(660, 436);
            this.panel3.TabIndex = 1;
            // 
            // tilesCollisionsCheckbox
            // 
            this.tilesCollisionsCheckbox.AutoSize = true;
            this.tilesCollisionsCheckbox.Location = new System.Drawing.Point(22, 20);
            this.tilesCollisionsCheckbox.Name = "tilesCollisionsCheckbox";
            this.tilesCollisionsCheckbox.Size = new System.Drawing.Size(107, 17);
            this.tilesCollisionsCheckbox.TabIndex = 5;
            this.tilesCollisionsCheckbox.Text = "Collisions (yellow)";
            this.tilesCollisionsCheckbox.UseVisualStyleBackColor = true;
            this.tilesCollisionsCheckbox.CheckedChanged += new System.EventHandler(this.tilesCollisionsCheckbox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(482, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 23);
            this.label2.TabIndex = 6;
            this.label2.Text = "Size...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1108, 561);
            this.Controls.Add(this.writeExeButton);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.sectionsPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Turrican II Tools";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.previewTabs.ResumeLayout(false);
            this.hexPage.ResumeLayout(false);
            this.txtPage.ResumeLayout(false);
            this.txtPage.PerformLayout();
            this.imgPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgZoomInput)).EndInit();
            this.bitmapControlsPanel.ResumeLayout(false);
            this.infoPage.ResumeLayout(false);
            this.infoPage.PerformLayout();
            this.mapPage.ResumeLayout(false);
            this.mapPage.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.mapMakerProgressPanel.ResumeLayout(false);
            this.mapMakerProgressPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).EndInit();
            this.tfmxPage.ResumeLayout(false);
            this.tilesPage.ResumeLayout(false);
            this.tilesPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilesPictureBox)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ColumnHeader nameHeader;
        private System.Windows.Forms.ColumnHeader sizeHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label hexSelectionLabel;
        private System.Windows.Forms.Panel hexEditorPanel;
        private System.Windows.Forms.Panel sectionsPanel;
        private System.Windows.Forms.ColumnHeader startHeader;
        private System.Windows.Forms.ColumnHeader endHeader;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button writeExeButton;
        private System.Windows.Forms.SaveFileDialog saveExeDialog;
        private System.Windows.Forms.Button applyChangesButton;
        private System.Windows.Forms.TabControl previewTabs;
        private System.Windows.Forms.TabPage hexPage;
        private System.Windows.Forms.TabPage txtPage;
        private System.Windows.Forms.TabPage palPage;
        private System.Windows.Forms.TabPage imgPage;
        private System.Windows.Forms.Panel bitmapControlsPanel;
        private System.Windows.Forms.Button prevBitmapButton;
        private System.Windows.Forms.Button nextBitmapButton;
        private System.Windows.Forms.Label currentBitmapIndexLabel;
        private System.Windows.Forms.TrackBar imgZoomInput;
        private System.Windows.Forms.TabPage infoPage;
        private System.Windows.Forms.TextBox infoOutput;
        private System.Windows.Forms.TabPage mapPage;
        private System.Windows.Forms.Panel mapMakerProgressPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar mapMakerProgressBar;
        private System.Windows.Forms.PictureBox mapPictureBox;
        private System.Windows.Forms.TabPage tfmxPage;
        private System.Windows.Forms.Button tfmxPlayButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox mapCollisionsCheckbox;
        private System.Windows.Forms.Label mapDetailsLabel;
        private System.Windows.Forms.TabPage tilesPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox tilesCollisionsCheckbox;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox tilesPictureBox;
    }
}

