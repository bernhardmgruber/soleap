using System.Collections.Generic;

namespace SoLeap.Common.Domain
{
    public class HandsFrame
    {
        public long Id { get; set; }

        public IList<Hand> Hands { get; set; }
    }
}
