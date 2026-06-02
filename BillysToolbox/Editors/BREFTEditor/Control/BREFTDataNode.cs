using kartlib.Serial;
using System.ComponentModel;

namespace ParticleEditor.Control
{
    public class BREFTDataNode : DataNode
    {
        private BREFT Breft;

        [Category("Misc"), Description("Internal file name")]
        public string Name
        {
            get { return Breft.ProjectHeader.Name; }
            private set { }
        }

        public BREFTDataNode(BREFT breft) : base(Path.GetFileName(breft.ProjectHeader.Name))
        {
            this.Breft = breft;
            SetImage("folder");

            // Add flat texture nodes
            foreach (BREFT._TableItem item in Breft.Table.Entries)
            {
                TextureDataNode textureNode = new TextureDataNode(item);
                AddChild(textureNode);
            }
        }
    }
}