﻿using System.Windows.Media.Media3D;

namespace SoLeap.Domain
{
    public class Bone
    {
        public BoneType Type { get; private set; }

        public Point3D NextJoint { get; private set; }

        public Point3D PrevJoint { get; private set; }

        public double Width { get; private set; }

        public Bone(BoneType type, Point3D nextJoint, Point3D prevJoint, double width)
        {
            Type = type;
            NextJoint = nextJoint;
            PrevJoint = prevJoint;
            Width = width;
        }
    }
}