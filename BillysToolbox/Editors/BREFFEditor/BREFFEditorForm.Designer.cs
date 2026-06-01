namespace BillysToolbox.Editors
{
    partial class BREFFEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BREFFEditorForm));
            splitter1 = new Splitter();
            splitContainer1 = new SplitContainer();
            fileTreeView = new TreeView();
            propertyGrid = new PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitter1
            // 
            splitter1.Location = new Point(0, 0);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(3, 404);
            splitter1.TabIndex = 0;
            splitter1.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(fileTreeView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(propertyGrid);
            splitContainer1.Size = new Size(578, 404);
            splitContainer1.SplitterDistance = 173;
            splitContainer1.TabIndex = 1;
            // 
            // fileTreeView
            // 
            fileTreeView.Dock = DockStyle.Fill;
            fileTreeView.Location = new Point(0, 0);
            fileTreeView.Name = "fileTreeView";
            fileTreeView.Size = new Size(173, 404);
            fileTreeView.TabIndex = 0;
            fileTreeView.AfterSelect += fileTreeView_AfterSelect;
            // 
            // propertyGrid
            // 
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new Point(0, 0);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(401, 404);
            propertyGrid.TabIndex = 0;
            // 
            // BREFFEditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(581, 404);
            Controls.Add(splitContainer1);
            Controls.Add(splitter1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "BREFFEditorForm";
            Text = "Particle Editor";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Splitter splitter1;
        private SplitContainer splitContainer1;
        private TreeView fileTreeView;
        private PropertyGrid propertyGrid;
    }
}