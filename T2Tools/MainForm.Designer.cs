﻿namespace T2Tools
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
            this.hexPanel = new System.Windows.Forms.Panel();
            this.hexEditorPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.hexSelectionLabel = new System.Windows.Forms.Label();
            this.txtPanel = new System.Windows.Forms.Panel();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.sectionsPanel = new System.Windows.Forms.Panel();
            this.hexPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.txtPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileList
            // 
            this.fileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.typeHeader,
            this.sizeHeader});
            this.fileList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileList.FullRowSelect = true;
            this.fileList.HideSelection = false;
            this.fileList.Location = new System.Drawing.Point(12, 34);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(335, 404);
            this.fileList.TabIndex = 0;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
            this.fileList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.tocList_ColumnClick);
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
            // 
            // nameHeader
            // 
            this.nameHeader.Text = "Name";
            this.nameHeader.Width = 107;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Type";
            this.typeHeader.Width = 129;
            // 
            // sizeHeader
            // 
            this.sizeHeader.Text = "Size";
            this.sizeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // hexPanel
            // 
            this.hexPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexPanel.Controls.Add(this.hexEditorPanel);
            this.hexPanel.Controls.Add(this.panel1);
            this.hexPanel.Location = new System.Drawing.Point(353, 34);
            this.hexPanel.Name = "hexPanel";
            this.hexPanel.Size = new System.Drawing.Size(435, 404);
            this.hexPanel.TabIndex = 1;
            // 
            // hexEditorPanel
            // 
            this.hexEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEditorPanel.Location = new System.Drawing.Point(0, 0);
            this.hexEditorPanel.Name = "hexEditorPanel";
            this.hexEditorPanel.Size = new System.Drawing.Size(435, 379);
            this.hexEditorPanel.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.hexSelectionLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 379);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(435, 25);
            this.panel1.TabIndex = 0;
            // 
            // hexSelectionLabel
            // 
            this.hexSelectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.hexSelectionLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexSelectionLabel.Location = new System.Drawing.Point(30, 4);
            this.hexSelectionLabel.Name = "hexSelectionLabel";
            this.hexSelectionLabel.Size = new System.Drawing.Size(396, 17);
            this.hexSelectionLabel.TabIndex = 0;
            this.hexSelectionLabel.Text = "no selection";
            this.hexSelectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPanel
            // 
            this.txtPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPanel.Controls.Add(this.txtOutput);
            this.txtPanel.Location = new System.Drawing.Point(353, 71);
            this.txtPanel.Name = "txtPanel";
            this.txtPanel.Size = new System.Drawing.Size(435, 404);
            this.txtPanel.TabIndex = 2;
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(435, 404);
            this.txtOutput.TabIndex = 0;
            // 
            // sectionsPanel
            // 
            this.sectionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sectionsPanel.Location = new System.Drawing.Point(12, 12);
            this.sectionsPanel.Name = "sectionsPanel";
            this.sectionsPanel.Size = new System.Drawing.Size(776, 16);
            this.sectionsPanel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.sectionsPanel);
            this.Controls.Add(this.txtPanel);
            this.Controls.Add(this.hexPanel);
            this.Controls.Add(this.fileList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Turrican II Tools";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.hexPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.txtPanel.ResumeLayout(false);
            this.txtPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ColumnHeader nameHeader;
        private System.Windows.Forms.ColumnHeader sizeHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.Panel hexPanel;
        private System.Windows.Forms.Panel txtPanel;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label hexSelectionLabel;
        private System.Windows.Forms.Panel hexEditorPanel;
        private System.Windows.Forms.Panel sectionsPanel;
    }
}

