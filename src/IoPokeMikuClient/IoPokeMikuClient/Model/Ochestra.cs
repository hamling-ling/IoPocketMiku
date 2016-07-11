using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;
using Windows.Storage.Streams;

namespace IoPokeMikuClient.Model
{
    public class Ochestra : MidiPlayer
    {
        static readonly Byte kChannelNum = 12;
        List<Byte> instsL = new List<Byte>();
        List<Byte> instsM = new List<Byte>();
        List<Byte> instsH = new List<Byte>();
        Byte m_maxChannel = 0;

        public Ochestra(string deviceName, IMidiOutPort port) : base(deviceName, port)
        {
            instsL = new List<Byte>{
                33,// Accoustic Bass
                43, //Cello
                44, // Contrabass
                58, // trombone
                58, // tuba
                61, // horn
                71 // basoon
            };
            instsM = new List<Byte> {
                49, // String Ensemble1
                50, // String Ensemble 2
                56, // ochestra all
                57, // trumpet
                62, // brass section
                69, // Oboe
                70, // english horn
                72, // clarinet
            };
            instsH = new List<Byte> {
                55, // voice ooh
                41, // Violin
                42, // Viola
                73, // piccolo
                74, // flute
            };
        }

        public override void SetupProgram()
        {
            byte counter = 0;
            foreach (var inst in instsL)
            {
                var msg = new MidiProgramChangeMessage(Convert.ToByte(counter), Convert.ToByte(inst));
                counter++;
                m_port.SendMessage(msg);
            }
            m_maxChannel = counter;
        }

        public void NoteOn(Byte note)
        {
            Byte octave = 0xFF & 12;
            Byte counter = 0;
            for(Byte i = counter; i < instsL.Count(); i++)
            {
                base.NoteOn(i, (byte)(note - octave));
                counter++;
            }
            for (Byte i = counter; i < instsM.Count(); i++)
            {
                base.NoteOn(i, (byte)(note));
                counter++;
            }
            for (Byte i = 0; i < instsH.Count(); i++)
            {
                base.NoteOn(i, (byte)(note + octave));
            }
        }
    }
}
