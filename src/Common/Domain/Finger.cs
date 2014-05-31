using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace SoLeap.Common.Domain
{
    public class Finger
    {
        public FingerType Type { get; private set; }

        public IList<Bone> Bones { get; private set; }

        public IList<Point3D> Joints { get; private set; }

        public Finger(FingerType type, IList<Bone> bones, IList<Point3D> joints)
        {
            Type = type;
            Bones = bones;
            Joints = joints;
        }

        public Bone GetBone(BoneType type)
        {
            return Bones[(int)type];
        }

        public Point3D GetJoint(JointType type)
        {
            return Joints[(int)type];
        }
    }
}