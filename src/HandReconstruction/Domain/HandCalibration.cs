using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandReconstruction.Domain
{
    /// <summary>
    /// Contains all information that has to be calibrated for a specific user (hand).
    /// This data is required by the Reconstructer.
    /// </summary>
    public class HandCalibration
    {
        /// <summary>
        /// Stores the lengths of all segments of all five fingers in mm.
        /// These are 3 values for the thumb and 4 values for all other fingers.
        /// The first value of each finger is the distance from the palm position to the start of the finger.
        /// </summary>
        public double[][] FingerSegmentLengths { get; set; }

        public HandCalibration()
        {
            FingerSegmentLengths = new double[5][];
            FingerSegmentLengths[0] = new double[3];
            FingerSegmentLengths[1] = new double[4];
            FingerSegmentLengths[2] = new double[4];
            FingerSegmentLengths[3] = new double[4];
            FingerSegmentLengths[4] = new double[4];
        }
    }
}
