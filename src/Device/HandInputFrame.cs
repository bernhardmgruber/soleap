using System.Windows.Media.Media3D;

namespace Device
{
    public class HandInputFrame
    {
        public Vector3D HandDirection { get; set; }

        public Point3D PalmPosition { get; set; }

        public Vector3D PalmNormal { get; set; }

        public Point3D[] TipPositions { get; set; }

        public HandInputFrame()
        {
            TipPositions = new Point3D[5];
        }
    }
}