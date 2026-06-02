using kartlib.Serial;
using System.ComponentModel;

namespace ParticleEditor.Control
{
    public class BREFFDataNode : DataNode
    {
        private BREFF Breff;

        [Category("Misc"), Description("Internal file name")]
        public string Name
        {
            get { return Breff.ProjectHeader.Name; }
            private set { }
        }

        public BREFFDataNode(BREFF breff) : base(Path.GetFileName(breff.ProjectHeader.Name))
        {
            this.Breff = breff;
            SetImage("folder");

            foreach (BREFF._TableItem item in Breff.Table.Entries)
            {
                SubfileDataNode subfileNode = new SubfileDataNode(item);
                AddChild(subfileNode);
            }

            if (Breff.AnimationTable.ParticleAnimations.Count <= 0) return;

            DataNode particleAnimsFolder = new DataNode("Particle Animations");
            particleAnimsFolder.SetImage("folder");

            for (int i = 0; i < Breff.AnimationTable.ParticleAnimations.Count; i++)
            {
                particleAnimsFolder.AddChild(new AnimationDataNode(Breff.AnimationTable.ParticleAnimations[i], $"Animation {i}"));
            }

            AddChild(particleAnimsFolder);
        }
    }
}
