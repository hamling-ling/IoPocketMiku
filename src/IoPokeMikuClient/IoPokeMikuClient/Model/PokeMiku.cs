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
    public class PokeMiku : MidiPlayer
    {
        Byte kMidiChannel = 0;

        public PokeMiku(string deviceName, IMidiOutPort port) : base(deviceName, port)
        {
        }

        public override void SetupProgram()
        {
            SetLylic(0);
        }

        public void SetLylic(byte x)
        {
            var buf = StrHex2ByteStream("F0 43 79 09 11 0A 01 00 F7");
            var sysExMsg = new MidiSystemExclusiveMessage(buf);
            m_port.SendMessage(sysExMsg);
            Debug.WriteLine("SysEx Message sent");
        }

        public void NoteOn(Byte note)
        {
            base.NoteOn(kMidiChannel, note);
        }
    }
}
