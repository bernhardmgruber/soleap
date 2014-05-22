using HandReconstruction.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandReconstruction.Reconstructer.Abstract
{
    public abstract class AbstractReconstructor : IReconstructer
    {
        protected HandCalibration calibration;

        public AbstractReconstructor(HandCalibration calibration)
        {
            this.calibration = calibration;
        }
        public abstract ReconstructedHand Process(Device.HandInputFrame input);
    }
}
