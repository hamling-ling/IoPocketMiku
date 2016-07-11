using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.Model
{
    public class Instrument
    {
        public Byte InstNum { get; private set; }
        public Byte Delta { get; private set; }
        public Byte Velocity { get; private set; }
        public bool IsEmpty { get; private set; }

        public Instrument()
        {
            InstNum = 0;
            Delta = 0;
            Velocity = 0;
            IsEmpty = true;
        }

        public Instrument(int num, int delta, int velocity)
        {
            InstNum = (byte)num;
            if (0 <= num - delta)
            {
                Delta = (byte)delta;
            }
            else
            {
                Delta = 0;
            }
            Velocity = (byte)velocity;
            IsEmpty = false;
        }
    }
}
