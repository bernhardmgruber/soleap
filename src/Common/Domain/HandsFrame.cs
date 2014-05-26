using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Common.Domain
{
    public class Finger
    {

    }

    public class Hand
    {
        public Vector3D HandDirection { get; set; }

        public Point3D PalmPosition { get; set; }

        public Vector3D PalmNormal { get; set; }

        public IList<Finger> Fingers { get; set; }
    }

    public class HandsFrame
    {
        IList<Hand> Hands { get; set; }
    }
}
