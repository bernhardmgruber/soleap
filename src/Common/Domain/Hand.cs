using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace SoLeap.Common.Domain
{
    public class Hand
    {
        public Point3D PalmPosition { get; set; }

        public Vector3D PalmNormal { get; set; }

        public float PalmWidth { get; set; }

        public float PalmHeight { get; set; }

        public Vector3D Direction { get; set; }

        public IDictionary<FingerType, Finger> Fingers { get; set; }
    }
}