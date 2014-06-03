using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace SoLeap.Domain
{
    public class Hand
    {
        public long Id { get; private set; }

        public Point3D PalmPosition { get; private set; }

        public Vector3D PalmNormal { get; private set; }

        public double PalmWidth { get; private set; }

        public double PalmHeight { get; private set; }

        public Matrix3D PalmTransformation { get; private set; }

        public Vector3D Direction { get; private set; }

        public IList<Finger> Fingers { get; private set; }

        public Hand(long id, Point3D palmPosition, Vector3D palmNormal, double palmWidth, double palmHeight, Matrix3D palmTransformation,
            Vector3D direction, IList<Finger> fingers)
        {
            Id = id;
            PalmPosition = palmPosition;
            PalmNormal = palmNormal;
            PalmWidth = palmWidth;
            PalmHeight = palmHeight;
            PalmTransformation = palmTransformation;
            Direction = direction;
            Fingers = fingers;
        }

        public Finger GetFinger(FingerType type)
        {
            return Fingers[(int)type];
        }
    }
}
