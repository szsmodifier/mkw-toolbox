using kartlib.Serial;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BillysToolbox.Editors
{
    public partial class U8EditorForm : Form, IEditor
    {
        private U8 FileInstance;
        private bool Compressed;
        private List<U8._Node> Nodes
        {
            get { return FileInstance.Nodes; }
            set { FileInstance.Nodes = value; }
        }
        private ImageList Icons;
        private Dictionary<List<string>, int> FileTypes;
        private int CurrentFolder;

        public U8EditorForm(U8 fileInstance, bool compressed = true)
        {
            Compressed = compressed;
            FileInstance = fileInstance;
            Icons = new ImageList();
            FileTypes = new Dictionary<List<string>, int>()
            {
                // Effects
                { new List<string> { ".breft", "Binary Revolution Effect Texture Project" }, 1 },
                { new List<string> { ".breff", "Binary Revolution Effect Project" }, 11 },

                // Menu files
                { new List<string> { ".bmg", "Binary Message Group" }, 1 },
                { new List<string> { ".brctr", "Binary Revolution Control" }, 1 },
                { new List<string> { ".brfnt", "Binary Revolution Font" }, 1 },
                { new List<string> { ".brlan", "Binary Revolution Layout Animation" }, 1 },
                { new List<string> { ".brlyt", "Binary Revolution Layout" }, 1 },

                { new List<string> { ".thp", "THP Movie File" }, 1 },
                { new List<string> { ".tpl", "Texture Palette Library" }, 1 },

                // Resources
                { new List<string> { ".brres", "Binary Revolution Resource" }, 3 },
                { new List<string> { ".brmdl", "Binary Revolution Model Resource" }, 1 },
                { new List<string> { ".brtex", "Binary Revolution Texture Resource" }, 1 },
                { new List<string> { ".brcha", "Binary Revolution Character Animation Resource" }, 1 },
                { new List<string> { ".brcla", "Binary Revolution Color Animation Resource" }, 1 },
                { new List<string> { ".brplt", "Binary Revolution Palette Resource" }, 1 },
                { new List<string> { ".brsca", "Binary Revolution Scene Animation Resource" }, 1 },
                { new List<string> { ".brsha", "Binary Revolution Shape Animation Resource" }, 1 },
                { new List<string> { ".brtpa", "Binary Revolution Texture Pattern Animation Resource" }, 1 },
                { new List<string> { ".brtsa", "Binary Revolution Texture SRT Animation Resource" }, 1 },
                { new List<string> { ".brvia", "Binary Revolution Visibility Animation Resource" }, 1 },

                { new List<string> { ".kcl", "KCL Collision File" }, 4 },

                // Mario Kart Wii files
                { new List<string> { ".kmp", "Mario Kart Wii Map Parameters" }, 2 },
                { new List<string> { ".krm", "Mario Kart Wii Rumble" }, 1 },
                { new List<string> { ".bsp", "Binary Settings and Physics" }, 1 },
                { new List<string> { ".btiEnv", "Binary Texture Information" }, 1 },
                { new List<string> { ".bti", "Binary Texture Information" }, 1 },
                { new List<string> { ".btiMat", "Binary Texture Information" }, 1 },
                { new List<string> { ".rkc", "Mario Kart Wii Competition File" }, 1 },
                { new List<string> { ".rkg", "Mario Kart Wii Ghost File" }, 1 },
                { new List<string> { ".ikp", "Inverse Kinematics Parameters" }, 1 },
                { new List<string> { ".bcp", "Binary Camera Parameters" }, 1 },
                { new List<string> { ".bmm", "Binary Mii Material" }, 10 },
                { new List<string> { ".brsar", "Binary Revolution Sound Archive" }, 1 },
                { new List<string> { ".brwar", "Binary Revolution Wave Archive" }, 1 },
                { new List<string> { ".brstm", "Binary Revolution Stream Sound" }, 1 },
                { new List<string> { ".brseq", "Binary Revolution Sequence" }, 1 },
                { new List<string> { ".brwsd", "Binary Revolution Wave Sound Data" }, 1 },
                { new List<string> { ".brasd", "Binary Revolution Animation Sound Data" }, 1 },
                { new List<string> { ".bdof", "Binary Depth of Field" }, 8 },
                { new List<string> { ".bdof1", "Binary Depth of Field" }, 8 },
                { new List<string> { ".bblm", "Binary Bloom" }, 9 },
                { new List<string> { ".bblm1", "Binary Bloom" }, 9 },
                { new List<string> { ".blmap", "Binary Light Map" }, 6 },
                { new List<string> { ".blmap1", "Binary Light Map" }, 6 },
                { new List<string> { ".bfg", "Binary Fog" }, 7 },
                { new List<string> { ".bfg1", "Binary Fog" }, 7 },
                { new List<string> { ".blight", "Binary Lighting" }, 5 },
                { new List<string> { ".blight1", "Binary Lighting" }, 5 },
            };

            InitializeComponent();
        }

        public void Save()
        {
            if (!File.Exists(FileInstance.Filename))
            {
                SaveAs();
                return;
            }

            byte[] buffer = FileInstance.Write();
            if (Compressed) buffer = YAZ0.Compress(buffer, YAZ0.CompressionAlgorithm.Fast);
            try { 
                File.WriteAllBytes(FileInstance.Filename, buffer);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
}

        public void SaveAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.FileName = Path.GetFileNameWithoutExtension(FileInstance.Filename);
                sfd.Filter = "SZS Files (*.szs)|*.szs|ARC Files (*.arc)|*arc";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = FileInstance.Write();
                    if (sfd.FilterIndex == 1)
                        buffer = YAZ0.Compress(buffer, YAZ0.CompressionAlgorithm.Fast);
                    try
                    {
                        File.WriteAllBytes(sfd.FileName, buffer);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private ListViewItem CreateListViewItem(int itemData)
        {
            ListViewItem item = new ListViewItem(Nodes[itemData].Name);
            item.Tag = itemData;
            if (Nodes[itemData].Type == U8._Node.NodeType.Directory)
            {
                item.ImageIndex = 0;
                //item.SubItems.Add("...");
            }
            else
            {
                item.SubItems.Add(Nodes[itemData].DataSize.ToString());
                item.ImageIndex = 1;
                string extension = Path.GetExtension(item.Text);
                foreach (KeyValuePair<List<string>, int> type in FileTypes)
                {
                    if (extension.ToLower().CompareTo(type.Key[0]) == 0)
                    {
                        item.SubItems.Add(type.Key[1]);
                        item.ImageIndex = type.Value;
                    }
                }
            }
            return item;
        }

        private void PopulateListView(int index)
        {
            if (Nodes[index].Type != U8._Node.NodeType.Directory)
                return;

            CurrentFolder = index;

            fileListView.BeginUpdate();
            fileListView.Items.Clear();

            // Add back option
            if (index != 0)
            {
                ListViewItem backItem = new ListViewItem("...");
                backItem.Tag = (int)Nodes[index].DataOffset;
                backItem.ImageIndex = 0;
                fileListView.Items.Add(backItem);
            }

            // Add given node's children
            int[] children = FileInstance.GetChildren(index);
            foreach (int i in children)
            {
                ListViewItem item = CreateListViewItem(i);
                fileListView.Items.Add(item);
            }

            // Update label
            itemCountStatusLabel.Text = children.Length + " items |";
            selectionStatusLabel.Text = "";

            fileListView.EndUpdate();
        }

        private void PopulateTree()
        {
            folderTree.Nodes.Clear();
            List<TreeNode> treeNodes = new List<TreeNode>();
            for (int i = 1; i < Nodes.Count; i++)
            {
                if (Nodes[i].Type != U8._Node.NodeType.Directory)
                    continue;

                TreeNode node = new TreeNode(Nodes[i].Name);
                node.Tag = i;
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
                treeNodes.Add(node);

                uint nodeParent = Nodes[i].DataOffset;
                if (nodeParent == 0)
                {
                    folderTree.Nodes.Add(node);
                }
                else
                {
                    TreeNode parentNode = treeNodes.FirstOrDefault(n => (uint)(int)n.Tag == nodeParent);
                    if (parentNode != null)
                    {
                        parentNode.Nodes.Add(node);
                    }
                }
            }

            folderTree.ExpandAll();
        }

        private void U8EditorForm_Load(object sender, EventArgs e)
        {
            Icons.Images.Add("folder", Properties.Resources.folder);
            Icons.Images.Add("page", Properties.Resources.page);
            Icons.Images.Add("kmp", Properties.Resources.kmp);
            Icons.Images.Add("kcl", Properties.Resources.kcl);
            Icons.Images.Add("brres", Properties.Resources.brres);
            Icons.Images.Add("blight", Properties.Resources.help);
            Icons.Images.Add("blmap", Properties.Resources.blmap);
            Icons.Images.Add("bfg", Properties.Resources.bfg);
            Icons.Images.Add("bdof", Properties.Resources.bdof);
            Icons.Images.Add("bblm", Properties.Resources.bblm);
            Icons.Images.Add("bmm", Properties.Resources.mii);
            Icons.Images.Add("breff", Properties.Resources.flame);
            Icons.ImageSize = new Size(16, 16);
            folderTree.ImageList = Icons;
            fileListView.SmallImageList = Icons;

            // Change text
            Text += " - " + Path.GetFileName(FileInstance.Filename);

            PopulateListView(0);
            PopulateTree();

            Application.Idle += ForceExpandOnIdle;
        }
        
        private void ForceExpandOnIdle(object? sender, EventArgs e)
        {
            Application.Idle -= ForceExpandOnIdle;
            folderTree.ExpandAll();
        }

        private void fileListView_ItemActivate(object sender, EventArgs e)
        {
            ListViewItem item = ((ListView)sender).SelectedItems[0];
            if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.Directory)
            {
                PopulateListView((int)item.Tag);
            }
            else if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
            {
                string name = Nodes[(int)item.Tag].Name;
                byte[] data = Nodes[(int)item.Tag].Data ?? new byte[0];
                Form? editor = EditorFactory.GetEditor(data, name, FileInstance);
                if (editor != null)
                {
                    editor.MdiParent = this.MdiParent;
                    editor.Show();
                }
            }
        }

        private void folderTree_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = ((TreeView)sender).SelectedNode;
            if (Nodes[(int)node.Tag].Type == U8._Node.NodeType.Directory)
            {
                PopulateListView((int)node.Tag);
            }
        }

        private void fileListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = ((ListView)sender).SelectedItems.Count;
            if (selected == 0)
            {
                selectionStatusLabel.Text = "";
            }
            else
            {
                selectionStatusLabel.Text = selected + " selected |";
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];
            item.BeginEdit();
        }

        private void fileListView_MouseClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count != 0)
            {
                ListViewItem item = listView.SelectedItems[0];
                if (e.Button == MouseButtons.Right)
                {
                    if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
                        fileRightClick.Show(Cursor.Position);
                    else
                        folderRightClick.Show(Cursor.Position);
                }
            }
        }

        private void fileListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            if (e.Label != null)
            {
                FileInstance.Rename((int)item.Tag, e.Label);
                PopulateTree();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;

            List<ListViewItem> itemsToDelete = new List<ListViewItem>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                itemsToDelete.Add(item);
            }

            itemsToDelete.Sort((a, b) => ((int)b.Tag).CompareTo((int)a.Tag));

            foreach (ListViewItem item in itemsToDelete)
            {
                if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
                {
                    FileInstance.RemoveFile((int)item.Tag);
                }
                else if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.Directory)
                {
                    FileInstance.RemoveFolder((int)item.Tag);
                }
            }

            PopulateListView(CurrentFolder);
            PopulateTree();
        }

        private void exportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0)
            {
                return;
            }
            else if (listView.SelectedItems.Count == 1)
            {
                ListViewItem item = listView.SelectedItems[0];
                if (Nodes[(int)item.Tag].Data == null) return;

                string extension = Path.GetExtension(Nodes[(int)item.Tag].Name);
                string noDot = extension.Replace(".", "").ToUpper();
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Nodes[(int)item.Tag].Name;
                sfd.Filter = noDot + " Files (*" + extension + ")|*" + extension + "|All Files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try { 
                        File.WriteAllBytes(sfd.FileName, Nodes[(int)item.Tag].Data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        string savePath = Path.Combine(dialog.FileName, Path.GetFileName(Nodes[(int)item.Tag].Name));
                        try { 
                            File.WriteAllBytes(savePath, Nodes[(int)item.Tag].Data);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void importFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try { 
                        byte[] buffer = File.ReadAllBytes(filename);
                        FileInstance.AddFile((int)item.Tag, buffer, filename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                PopulateListView(FileInstance.GetNodeFolder((int)item.Tag));
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;

            ((MainForm)MdiParent).Clipboard.Clear();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
                {
                    KeyValuePair<string, byte[]> clipboardItem = new KeyValuePair<string, byte[]>(
                        Nodes[(int)item.Tag].Name,
                        Nodes[(int)item.Tag].Data
                    );
                    ((MainForm)MdiParent).Clipboard.Insert(0, clipboardItem);
                }
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            if (((MainForm)MdiParent).Clipboard.Count == 0) return;

            foreach (KeyValuePair<string, byte[]> cbItem in ((MainForm)MdiParent).Clipboard)
            {
                Debug.WriteLine(cbItem);
                FileInstance.AddFile((int)item.Tag, cbItem.Value, cbItem.Key);
            }

            PopulateListView(FileInstance.GetNodeFolder((int)item.Tag));
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            FileInstance.AddFolder((int)item.Tag, "New Folder");
            PopulateListView(FileInstance.GetNodeFolder((int)item.Tag));
            PopulateTree();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;

            ((MainForm)MdiParent).Clipboard.Clear();

            List<int> indicesToCut = new List<int>();

            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
                {
                    KeyValuePair<string, byte[]> clipboardItem = new KeyValuePair<string, byte[]>(
                        Nodes[(int)item.Tag].Name,
                        Nodes[(int)item.Tag].Data
                    );
                    ((MainForm)MdiParent).Clipboard.Insert(0, clipboardItem);

                    indicesToCut.Add((int)item.Tag);
                }
            }

            indicesToCut.Sort((a, b) => b.CompareTo(a));

            foreach (int index in indicesToCut)
            {
                FileInstance.RemoveFile(index);
            }

            PopulateListView(CurrentFolder);
        }

        private void folderDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteToolStripMenuItem_Click(sender, e);
        }

        private void replaceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.File)
            {
                string extension = Path.GetExtension(Nodes[(int)item.Tag].Name);
                string noDot = extension.Replace(".", "").ToUpper();
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = noDot + " Files (*" + extension + ")|*" + extension + "|All Files (*.*)|*.*";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Nodes[(int)item.Tag].Data = File.ReadAllBytes(openFileDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void folderExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count == 0) return;
            ListViewItem item = listView.SelectedItems[0];

            if (Nodes[(int)item.Tag].Type == U8._Node.NodeType.Directory)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string rootPath = Path.Combine(dialog.FileName, Nodes[(int)item.Tag].Name);
                    try { 
                        Directory.CreateDirectory(rootPath);
                        int[] children = FileInstance.GetChildren((int)item.Tag);
                        folderExportHelper(children, rootPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void folderExportHelper(int[] children, string path)
        {
            foreach (int child in children)
            {
                if (Nodes[child].Type == U8._Node.NodeType.File)
                {
                    string filePath = Path.Combine(path, Nodes[child].Name);
                    try { 
                        File.WriteAllBytes(filePath, Nodes[child].Data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (Nodes[child].Type == U8._Node.NodeType.Directory)
                {
                    string newPath = Path.Combine(path, Nodes[child].Name);
                    try { 
                        Directory.CreateDirectory(newPath);
                        folderExportHelper(FileInstance.GetChildren(child), newPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void folderNewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFolderToolStripMenuItem_Click(sender, e);
        }

        private void importFilesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count != 0) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "All Files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in ofd.FileNames)
                    {
                        try { 
                            byte[] buffer = File.ReadAllBytes(file);
                            FileInstance.AddFile(CurrentFolder, buffer, file);
                            PopulateListView(CurrentFolder);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Couldn't open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void fileListView_MouseUp(object sender, MouseEventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count != 0) return;

            if (e.Button == MouseButtons.Right)
            {
                blankRightClick.Show(Cursor.Position);
            }
        }

        private void newFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListView listView = fileListView;
            if (listView.SelectedItems.Count != 0) return;

            FileInstance.AddFolder(CurrentFolder, "New Folder");
            PopulateListView(CurrentFolder);
            PopulateTree();
        }
    }
}
