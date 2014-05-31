using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace SoLeap.Common.Domain
{
    public class Hand
    {
        public Point3D PalmPosition { get; private set; }

        public Vector3D PalmNormal { get; private set; }

        public float PalmWidth { get; private set; }

        public float PalmHeight { get; private set; }

        public Vector3D Direction { get; private set; }

        public IList<Finger> Fingers { get; private set; }

        public Hand(Point3D palmPosition, Vector3D palmNormal, float palmWidth, float palmHeight,
            Vector3D direction, IList<Finger> fingers)
        {
            PalmPosition = palmPosition;
            PalmNormal = palmNormal;
            PalmWidth = palmWidth;
            PalmHeight = palmHeight;
            Direction = direction;
            Fingers = fingers;
        }

        public Finger GetFinger(FingerType type)
        {
            return Fingers[(int)type];
        }
    }
}