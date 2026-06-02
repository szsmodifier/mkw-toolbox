using kartlib.Serial;
using System.ComponentModel;

namespace ParticleEditor.Control
{
    internal class AnimationDataNode : DataNode
    {
        private BREFF._Animation AnimationItem;

        [Category("General Info"), Description("Should be 0xAB or 0xAC")]
        public byte Identifier
        {
            get { return AnimationItem.Identifier; }
            set { AnimationItem.Identifier = value; }
        }

        [Category("Configuration")]
        public byte KindType
        {
            get { return AnimationItem.KindType; }
            set { AnimationItem.KindType = value; }
        }

        [Category("Configuration")]
        public byte CurveFlag
        {
            get { return AnimationItem.CurveFlag; }
            set { AnimationItem.CurveFlag = value; }
        }

        [Category("Configuration")]
        public byte KindEnable
        {
            get { return AnimationItem.KindEnable; }
            set { AnimationItem.KindEnable = value; }
        }

        [Category("Configuration")]
        public byte ProcessFlag
        {
            get { return AnimationItem.ProcessFlag; }
            set { AnimationItem.ProcessFlag = value; }
        }

        [Category("Playback")]
        public byte LoopCount
        {
            get { return AnimationItem.LoopCount; }
            set { AnimationItem.LoopCount = value; }
        }

        [Category("Playback")]
        public ushort FrameCount
        {
            get { return AnimationItem.FrameCount; }
            set { AnimationItem.FrameCount = value; }
        }

        [Category("Playback")]
        public ushort RandomSeed
        {
            get { return AnimationItem.RandomSeed; }
            set { AnimationItem.RandomSeed = value; }
        }

        public AnimationDataNode(BREFF._Animation animItem, string nodeName) : base(nodeName)
        {
            this.AnimationItem = animItem;
            SetImage("page");
        }
    }
}