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
    public class PokeMiku : ObservableObject
    {
        Byte kMidiChannel = 0;
        Byte kDefaultVelocity = 127;

        IMidiOutPort m_port;
        readonly Object m_lock = new Object();
        string m_deviceName;
        Byte m_note = 0;

        public string DeviceName
        {
            get { return m_deviceName; }
        }

        public Byte Note {
            get { return m_note; }
            private set
            {
                Set(() => Note, ref m_note, value);
            }
        }

        public PokeMiku(string deviceName, IMidiOutPort port)
        {
            Debug.Assert(port != null);
            m_port = port;
            m_deviceName = deviceName;
        }

        public void SetLylic(byte x)
        {
            var buf = StrHex2ByteStream("F0 43 79 09 11 0A 01 00 F7");
            var sysExMsg = new MidiSystemExclusiveMessage(buf);
            m_port.SendMessage(sysExMsg);
            Debug.WriteLine("SysEx Message sent");
        }

        IBuffer StrHex2ByteStream(string hexStr)
        {
            var dataWriter = new DataWriter();

            var lit = new[] { " " };
            var split = hexStr.Split(new[] { ' ' });
            foreach (var item in split)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var strByte = Convert.ToByte(item, 16);
                dataWriter.WriteByte(strByte);
            }
            return dataWriter.DetachBuffer();
        }

        public void NoteOn(Byte note)
        {
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOnMessage(kMidiChannel, note, kDefaultVelocity);
                m_port.SendMessage(midiMsg);
                m_note = note;
            }
            Debug.WriteLine("Note On " + note);
        }

        public void NoteOff()
        {
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOffMessage(kMidiChannel, m_note, kDefaultVelocity);
                m_port.SendMessage(midiMsg);
            }
            Debug.WriteLine("Note Off");
        }
    }
}
