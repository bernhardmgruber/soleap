using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace SoLeap.Common.Domain
{
    public class Finger
    {
        public FingerType Type { get; set; }

        public IDictionary<BoneType, Bone> Bones { get; set; }

        public IDictionary<JointType, Point3D> Joints { get; set; }
    }
}