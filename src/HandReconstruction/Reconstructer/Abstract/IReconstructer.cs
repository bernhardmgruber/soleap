using Device;
using HandReconstruction.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandReconstruction.Reconstructer.Abstract
{
    public interface IReconstructer
    {
        ReconstructedHand Process(HandInputFrame input);
    }
}
