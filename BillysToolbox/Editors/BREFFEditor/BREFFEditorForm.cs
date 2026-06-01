using kartlib.Serial;
using ParticleEditor.Control;

namespace BillysToolbox.Editors
{
    public partial class BREFFEditorForm : Form, IEditor
    {
        static ImageList _imageList;
        public static ImageList ImageList
        {
            get
            {
                if (_imageList == null)
                {
                    _imageList = new ImageList();
                    _imageList.Images.Add("page", Properties.Resources.blmap);
                    _imageList.Images.Add("box", Properties.Resources.folder);
                    _imageList.Images.Add("folder", Properties.Resources.folder);
                    _imageList.Images.Add("light", Properties.Resources.help);
                    _imageList.Images.Add("bolt", Properties.Resources.flame);
                }
                return _imageList;
            }
        }

        BREFF FileInstance;
        U8? ParentInstance;

        public BREFFEditorForm(BREFF fileInstance)
        {
            FileInstance = fileInstance;
            
            InitializeComponent();
            fileTreeView.ImageList = ImageList;
            PopulateUI();
        }

        public BREFFEditorForm(BREFF fileInstance, U8? parentInstance)
        {
            FileInstance = fileInstance;
            ParentInstance = parentInstance;
            
            InitializeComponent();
            fileTreeView.ImageList = ImageList;
            PopulateUI();
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(FileInstance.Filename);
            sfd.Filter = "BREFF Files (*.breff)|*.breff";

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
            BREFFDataNode breffNode = new BREFFDataNode(FileInstance);
            fileTreeView.Nodes.Add(breffNode.GetTreeNode());
        }

        private void fileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (fileTreeView.SelectedNode != null)
                propertyGrid.SelectedObject = fileTreeView.SelectedNode.Tag;
        }
    }
}
