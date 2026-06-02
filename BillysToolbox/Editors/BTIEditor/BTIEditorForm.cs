using kartlib.Serial;

namespace BillysToolbox.Editors
{
    public partial class BTIEditorForm : Form, IEditor
    {
        BTI FileInstance;
        U8? ParentInstance;

        public BTIEditorForm(BTI fileInstance)
        {
            FileInstance = fileInstance;
            InitializeComponent();
        }

        public BTIEditorForm(BTI fileInstance, U8? parentInstance)
        {
            FileInstance = fileInstance;
            ParentInstance = parentInstance;
            InitializeComponent();
        }

        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = Path.GetFileNameWithoutExtension(FileInstance.Filename);
            sfd.Filter = "BTI Files (*.bti,*.btiMat,*.btiEnv)|*.bti,*.btiMat,*.btiEnv";

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

        private void BTIEditorForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = FileInstance;
            pictureBox1.BackgroundImage = FileInstance.Image;
        }
    }
}
