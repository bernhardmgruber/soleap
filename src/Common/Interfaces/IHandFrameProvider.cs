using Common.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public delegate void HandFrameReadHandler(HandsFrame frame);

    public interface IHandFrameProvider
    {
        event HandFrameReadHandler FrameReady;
    }
}
