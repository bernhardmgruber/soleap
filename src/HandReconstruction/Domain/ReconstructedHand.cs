using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HandReconstruction.Domain
{
    /// <summary>
    /// Holds 
    /// </summary>
    public class ReconstructedHand
    {
        public Vector3D HandDirection { get; set; }

        public Point3D PalmPosition { get; set; }

        public Vector3D PalmNormal { get; set; }

        public Point3D[][] FingerJointPositions { get; set; }

        public ReconstructedHand()
        {
            FingerJointPositions = new Point3D[5][];
            FingerJointPositions[0] = new Point3D[3];
            FingerJointPositions[1] = new Point3D[4];
            FingerJointPositions[2] = new Point3D[4];
            FingerJointPositions[3] = new Point3D[4];
            FingerJointPositions[4] = new Point3D[4];
        }
    }
}
