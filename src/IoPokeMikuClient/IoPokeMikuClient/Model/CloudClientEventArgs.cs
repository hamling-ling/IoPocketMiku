using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.Model
{
    public class CloudClientEventArgs : EventArgs
    {
        public PitchInfo PitchInfo { get; private set; }

        public CloudClientEventArgs(PitchInfo info)
        {
            PitchInfo = info;
        }
    }
}
