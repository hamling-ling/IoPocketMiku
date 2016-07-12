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
        public Int16 Delta { get; private set; }
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
                Delta = (Int16)delta;
            }
            else
            {
                Delta = 0;
            }
            Velocity = (byte)velocity;
            IsEmpty = false;
        }

        public virtual Byte GetNote(Byte note)
        {
            Int16 iNote = (Int16)note;
            Int16 outNote = (Int16)(iNote + Delta);
            if(outNote < 0)
            {
                return note;
            } else
            {
                return (Byte)outNote;
            }
        }
    }

    public class Percussion : Instrument
    {
        Byte m_note;

        public Percussion() : base() { }

        public Percussion(int num, int note, int velocity) : base(num, 0, velocity)
        {
            m_note = (Byte)note;
        }

        public override byte GetNote(byte note)
        {
            return m_note;
        }
    }
}
