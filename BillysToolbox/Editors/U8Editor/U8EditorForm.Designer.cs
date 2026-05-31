namespace BillysToolbox.Editors
{
    partial class U8EditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(U8EditorForm));
            splitContainer = new SplitContainer();
            folderTree = new TreeView();
            fileListView = new ListView();
            nameColumn = new ColumnHeader();
            sizeColumn = new ColumnHeader();
            typeColumn = new ColumnHeader();
            statusStrip = new StatusStrip();
            itemCountStatusLabel = new ToolStripStatusLabel();
            selectionStatusLabel = new ToolStripStatusLabel();
            fileRightClick = new ContextMenuStrip(components);
            copyToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            renameToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exportFileToolStripMenuItem = new ToolStripMenuItem();
            importFilesToolStripMenuItem = new ToolStripMenuItem();
            replaceFileToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            newFolderToolStripMenuItem = new ToolStripMenuItem();
            folderRightClick = new ContextMenuStrip(components);
            folderPasteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            folderRenameToolStripMenuItem = new ToolStripMenuItem();
            folderDeleteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            folderExportToolStripMenuItem = new ToolStripMenuItem();
            folderImportFilesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            folderNewFolderToolStripMenuItem = new ToolStripMenuItem();
            importFilesToolStripMenuItem1 = new ToolStripMenuItem();
            newFolderToolStripMenuItem1 = new ToolStripMenuItem();
            blankRightClick = new ContextMenuStrip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            statusStrip.SuspendLayout();
            fileRightClick.SuspendLayout();
            folderRightClick.SuspendLayout();
            blankRightClick.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(folderTree);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(fileListView);
            splitContainer.Size = new Size(800, 417);
            splitContainer.SplitterDistance = 209;
            splitContainer.TabIndex = 1;
            // 
            // folderTree
            // 
            folderTree.Dock = DockStyle.Fill;
            folderTree.Location = new Point(0, 0);
            folderTree.Name = "folderTree";
            folderTree.Size = new Size(209, 417);
            folderTree.TabIndex = 0;
            folderTree.DoubleClick += folderTree_DoubleClick;
            // 
            // fileListView
            // 
            fileListView.Columns.AddRange(new ColumnHeader[] { nameColumn, sizeColumn, typeColumn });
            fileListView.Dock = DockStyle.Fill;
            fileListView.FullRowSelect = true;
            fileListView.LabelEdit = true;
            fileListView.Location = new Point(0, 0);
            fileListView.Name = "fileListView";
            fileListView.Size = new Size(587, 417);
            fileListView.TabIndex = 0;
            fileListView.UseCompatibleStateImageBehavior = false;
            fileListView.View = View.Details;
            fileListView.AfterLabelEdit += fileListView_AfterLabelEdit;
            fileListView.ItemActivate += fileListView_ItemActivate;
            fileListView.SelectedIndexChanged += fileListView_SelectedIndexChanged;
            fileListView.MouseClick += fileListView_MouseClick;
            fileListView.MouseUp += fileListView_MouseUp;
            // 
            // nameColumn
            // 
            nameColumn.Text = "Name";
            nameColumn.Width = 240;
            // 
            // sizeColumn
            // 
            sizeColumn.Text = "Size";
            sizeColumn.Width = 80;
            // 
            // typeColumn
            // 
            typeColumn.Text = "Type";
            typeColumn.Width = 206;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.Transparent;
            statusStrip.Items.AddRange(new ToolStripItem[] { itemCountStatusLabel, selectionStatusLabel });
            statusStrip.Location = new Point(0, 420);
            statusStrip.Name = "statusStrip";
            statusStrip.RightToLeft = RightToLeft.No;
            statusStrip.Size = new Size(800, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // itemCountStatusLabel
            // 
            itemCountStatusLabel.Name = "itemCountStatusLabel";
            itemCountStatusLabel.Size = new Size(51, 17);
            itemCountStatusLabel.Text = "0 items |";
            // 
            // selectionStatusLabel
            // 
            selectionStatusLabel.Name = "selectionStatusLabel";
            selectionStatusLabel.Size = new Size(0, 17);
            // 
            // fileRightClick
            // 
            fileRightClick.Items.AddRange(new ToolStripItem[] { copyToolStripMenuItem, cutToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator1, renameToolStripMenuItem, deleteToolStripMenuItem, toolStripSeparator2, exportFileToolStripMenuItem, importFilesToolStripMenuItem, replaceFileToolStripMenuItem, toolStripSeparator3, newFolderToolStripMenuItem });
            fileRightClick.Name = "contextMenuStrip1";
            fileRightClick.Size = new Size(152, 220);
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Image = Properties.Resources.page_copy;
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(151, 22);
            copyToolStripMenuItem.Text = "Copy";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Image = Properties.Resources.cut;
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Size = new Size(151, 22);
            cutToolStripMenuItem.Text = "Cut";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Image = Properties.Resources.page_paste;
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new Size(151, 22);
            pasteToolStripMenuItem.Text = "Paste";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(148, 6);
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Image = Properties.Resources.textfield;
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(151, 22);
            renameToolStripMenuItem.Text = "Rename";
            renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Image = Properties.Resources.cross;
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(151, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(148, 6);
            // 
            // exportFileToolStripMenuItem
            // 
            exportFileToolStripMenuItem.Image = Properties.Resources.document_export;
            exportFileToolStripMenuItem.Name = "exportFileToolStripMenuItem";
            exportFileToolStripMenuItem.Size = new Size(151, 22);
            exportFileToolStripMenuItem.Text = "Export file(s)...";
            exportFileToolStripMenuItem.Click += exportFileToolStripMenuItem_Click;
            // 
            // importFilesToolStripMenuItem
            // 
            importFilesToolStripMenuItem.Image = Properties.Resources.document_import;
            importFilesToolStripMenuItem.Name = "importFilesToolStripMenuItem";
            importFilesToolStripMenuItem.Size = new Size(151, 22);
            importFilesToolStripMenuItem.Text = "Import file(s)...";
            importFilesToolStripMenuItem.Click += importFilesToolStripMenuItem_Click;
            // 
            // replaceFileToolStripMenuItem
            // 
            replaceFileToolStripMenuItem.Image = Properties.Resources.page_white_edit;
            replaceFileToolStripMenuItem.Name = "replaceFileToolStripMenuItem";
            replaceFileToolStripMenuItem.Size = new Size(151, 22);
            replaceFileToolStripMenuItem.Text = "Replace file...";
            replaceFileToolStripMenuItem.Click += replaceFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(148, 6);
            // 
            // newFolderToolStripMenuItem
            // 
            newFolderToolStripMenuItem.Image = Properties.Resources.folder_add;
            newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            newFolderToolStripMenuItem.Size = new Size(151, 22);
            newFolderToolStripMenuItem.Text = "New folder";
            newFolderToolStripMenuItem.Click += newFolderToolStripMenuItem_Click;
            // 
            // folderRightClick
            // 
            folderRightClick.Items.AddRange(new ToolStripItem[] { folderPasteToolStripMenuItem, toolStripSeparator4, folderRenameToolStripMenuItem, folderDeleteToolStripMenuItem, toolStripSeparator5, folderExportToolStripMenuItem, folderImportFilesToolStripMenuItem, toolStripSeparator6, folderNewFolderToolStripMenuItem });
            folderRightClick.Name = "contextMenuStrip1";
            folderRightClick.Size = new Size(152, 154);
            // 
            // folderPasteToolStripMenuItem
            // 
            folderPasteToolStripMenuItem.Image = Properties.Resources.page_paste;
            folderPasteToolStripMenuItem.Name = "folderPasteToolStripMenuItem";
            folderPasteToolStripMenuItem.Size = new Size(151, 22);
            folderPasteToolStripMenuItem.Text = "Paste";
            folderPasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(148, 6);
            // 
            // folderRenameToolStripMenuItem
            // 
            folderRenameToolStripMenuItem.Image = Properties.Resources.textfield;
            folderRenameToolStripMenuItem.Name = "folderRenameToolStripMenuItem";
            folderRenameToolStripMenuItem.Size = new Size(151, 22);
            folderRenameToolStripMenuItem.Text = "Rename";
            folderRenameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            // 
            // folderDeleteToolStripMenuItem
            // 
            folderDeleteToolStripMenuItem.Image = Properties.Resources.cross;
            folderDeleteToolStripMenuItem.Name = "folderDeleteToolStripMenuItem";
            folderDeleteToolStripMenuItem.Size = new Size(151, 22);
            folderDeleteToolStripMenuItem.Text = "Delete";
            folderDeleteToolStripMenuItem.Click += folderDeleteToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(148, 6);
            // 
            // folderExportToolStripMenuItem
            // 
            folderExportToolStripMenuItem.Image = Properties.Resources.folder_go;
            folderExportToolStripMenuItem.Name = "folderExportToolStripMenuItem";
            folderExportToolStripMenuItem.Size = new Size(151, 22);
            folderExportToolStripMenuItem.Text = "Export folder...";
            folderExportToolStripMenuItem.Click += folderExportToolStripMenuItem_Click;
            // 
            // folderImportFilesToolStripMenuItem
            // 
            folderImportFilesToolStripMenuItem.Image = Properties.Resources.document_import;
            folderImportFilesToolStripMenuItem.Name = "folderImportFilesToolStripMenuItem";
            folderImportFilesToolStripMenuItem.Size = new Size(151, 22);
            folderImportFilesToolStripMenuItem.Text = "Import file(s)...";
            folderImportFilesToolStripMenuItem.Click += importFilesToolStripMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(148, 6);
            // 
            // folderNewFolderToolStripMenuItem
            // 
            folderNewFolderToolStripMenuItem.Image = Properties.Resources.folder_add;
            folderNewFolderToolStripMenuItem.Name = "folderNewFolderToolStripMenuItem";
            folderNewFolderToolStripMenuItem.Size = new Size(151, 22);
            folderNewFolderToolStripMenuItem.Text = "New folder";
            folderNewFolderToolStripMenuItem.Click += folderNewFolderToolStripMenuItem_Click;
            // 
            // importFilesToolStripMenuItem1
            // 
            importFilesToolStripMenuItem1.Image = Properties.Resources.document_import;
            importFilesToolStripMenuItem1.Name = "importFilesToolStripMenuItem1";
            importFilesToolStripMenuItem1.Size = new Size(151, 22);
            importFilesToolStripMenuItem1.Text = "Import file(s)...";
            importFilesToolStripMenuItem1.Click += importFilesToolStripMenuItem1_Click;
            // 
            // newFolderToolStripMenuItem1
            // 
            newFolderToolStripMenuItem1.Image = Properties.Resources.folder_add;
            newFolderToolStripMenuItem1.Name = "newFolderToolStripMenuItem1";
            newFolderToolStripMenuItem1.Size = new Size(151, 22);
            newFolderToolStripMenuItem1.Text = "New folder";
            newFolderToolStripMenuItem1.Click += newFolderToolStripMenuItem1_Click;
            // 
            // blankRightClick
            // 
            blankRightClick.Items.AddRange(new ToolStripItem[] { importFilesToolStripMenuItem1, newFolderToolStripMenuItem1 });
            blankRightClick.Name = "blankRightClick";
            blankRightClick.Size = new Size(152, 48);
            // 
            // U8EditorForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 442);
            Controls.Add(statusStrip);
            Controls.Add(splitContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "U8EditorForm";
            Text = "U8 Editor";
            Load += U8EditorForm_Load;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            fileRightClick.ResumeLayout(false);
            folderRightClick.ResumeLayout(false);
            blankRightClick.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer;
        private TreeView folderTree;
        private ListView fileListView;
        private ColumnHeader nameColumn;
        private ColumnHeader sizeColumn;
        private ColumnHeader typeColumn;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel itemCountStatusLabel;
        private ToolStripStatusLabel selectionStatusLabel;
        private ContextMenuStrip fileRightClick;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exportFileToolStripMenuItem;
        private ToolStripMenuItem importFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem newFolderToolStripMenuItem;
        private ContextMenuStrip folderRightClick;
        private ToolStripMenuItem folderPasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem folderRenameToolStripMenuItem;
        private ToolStripMenuItem folderDeleteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem folderExportToolStripMenuItem;
        private ToolStripMenuItem folderImportFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem folderNewFolderToolStripMenuItem;
        private ToolStripMenuItem replaceFileToolStripMenuItem;
        private ToolStripMenuItem importFilesToolStripMenuItem1;
        private ToolStripMenuItem newFolderToolStripMenuItem1;
        private ContextMenuStrip blankRightClick;
    }
}