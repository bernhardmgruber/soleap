using System.Windows.Media.Media3D;

namespace SoLeap.Common.Domain
{
    public class Bone
    {
        public BoneType Type { get; set; }

        public Point3D NextJoint { get; set; }

        public Point3D PrevJoint { get; set; }

        public float Width { get; set; }
    }
}