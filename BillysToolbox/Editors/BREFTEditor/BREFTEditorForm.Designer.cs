namespace BillysToolbox.Editors
{
    partial class BREFTEditorForm
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BREFTEditorForm));
            splitContainer1 = new SplitContainer();
            fileTreeView = new TreeView();
            splitContainer2 = new SplitContainer();
            propertyGrid = new PropertyGrid();
            pictureBox = new PictureBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            replaceImageToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(fileTreeView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(478, 450);
            splitContainer1.SplitterDistance = 206;
            splitContainer1.TabIndex = 0;
            // 
            // fileTreeView
            // 
            fileTreeView.Dock = DockStyle.Fill;
            fileTreeView.Location = new Point(0, 0);
            fileTreeView.Name = "fileTreeView";
            fileTreeView.Size = new Size(206, 450);
            fileTreeView.TabIndex = 0;
            fileTreeView.AfterSelect += fileTreeView_AfterSelect;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(propertyGrid);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(pictureBox);
            splitContainer2.Size = new Size(268, 450);
            splitContainer2.SplitterDistance = 223;
            splitContainer2.TabIndex = 0;
            // 
            // propertyGrid
            // 
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new Point(0, 0);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(268, 223);
            propertyGrid.TabIndex = 0;
            // 
            // pictureBox
            // 
            pictureBox.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(268, 223);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { replaceImageToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(161, 26);
            // 
            // replaceImageToolStripMenuItem
            // 
            replaceImageToolStripMenuItem.Image = Properties.Resources.document_import;
            replaceImageToolStripMenuItem.Name = "replaceImageToolStripMenuItem";
            replaceImageToolStripMenuItem.Size = new Size(160, 22);
            replaceImageToolStripMenuItem.Text = "Replace image...";
            replaceImageToolStripMenuItem.Click += replaceImageToolStripMenuItem_Click;
            // 
            // BREFTEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 450);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "BREFTEditorForm";
            Text = "BREFT Editor";
            Load += BREFTEditorForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TreeView fileTreeView;
        private SplitContainer splitContainer2;
        private PropertyGrid propertyGrid;
        private PictureBox pictureBox;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem replaceImageToolStripMenuItem;
    }
}