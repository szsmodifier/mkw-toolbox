using ParticleEditor.Control;
using kartlib.Serial;

namespace BillysToolbox.Editors
{
    public partial class BREFTEditorForm : Form, IEditor
    {
        static ImageList _imageList;
        public static ImageList ImageList
        {
            get
            {
                if (_imageList == null)
                {
                    _imageList = new ImageList();
                    _imageList.Images.Add("folder", Properties.Resources.folder);
                    _imageList.Images.Add("decal", Properties.Resources.Decal);
                }
                return _imageList;
            }
        }

        BREFT FileInstance;
        U8? ParentInstance;

        public BREFTEditorForm(BREFT fileInstance)
        {
            FileInstance = fileInstance;
            InitializeComponent();
        }

        public BREFTEditorForm(BREFT fileInstance, U8? parentInstance)
        {
            FileInstance = fileInstance;
            ParentInstance = parentInstance;
            InitializeComponent();
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(FileInstance.Filename);
            sfd.Filter = "BREFT Files (*.breft)|*.breft";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                byte[] buffer = FileInstance.Write();
                File.WriteAllBytes(sfd.FileName, buffer);
            }
        }

        public void Save()
        {
            if (ParentInstance != null)
            {
                int index = ParentInstance.FindIndexFromName(FileInstance.Filename);
                if (index > 0)
                {
                    ParentInstance.Nodes[index].Data = FileInstance.Write();
                }
            }
            else if (!File.Exists(FileInstance.Filename))
            {
                SaveAs();
                return;
            }
            else
            {
                byte[] buffer = FileInstance.Write();
                File.WriteAllBytes(FileInstance.Filename, buffer);
            }
        }

        private void PopulateUI()
        {
            fileTreeView.Nodes.Clear();
            BREFTDataNode breffNode = new BREFTDataNode(FileInstance);
            fileTreeView.Nodes.Add(breffNode.GetTreeNode());
        }

        private void fileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (fileTreeView.SelectedNode != null)
            {
                propertyGrid.SelectedObject = fileTreeView.SelectedNode.Tag;
                if (fileTreeView.SelectedNode.Tag.GetType() == typeof(TextureDataNode))
                {
                    pictureBox.BackgroundImage = (fileTreeView.SelectedNode.Tag as TextureDataNode).Item.Texture.Image;
                }
            }
        }

        private void replaceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTreeView.SelectedNode?.Tag is TextureDataNode texNode)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select Replacement Image";
                    ofd.Filter = "Image Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All Files (*.*)|*.*";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap newImg = new Bitmap(ofd.FileName);
                        texNode.Item.Texture.ReplaceImage(newImg, texNode.Item.Texture.FormatEnum);
                        propertyGrid.Refresh();
                    }
                }
            }
        }

        private void BREFTEditorForm_Load(object sender, EventArgs e)
        {
            fileTreeView.ContextMenuStrip = contextMenuStrip1;
            fileTreeView.ImageList = ImageList;
            PopulateUI();

            Application.Idle += ForceExpandOnIdle;
        }

        private void ForceExpandOnIdle(object? sender, EventArgs e)
        {
            Application.Idle -= ForceExpandOnIdle;
            fileTreeView.ExpandAll();
        }
    }
}
