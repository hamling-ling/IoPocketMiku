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
    public class MidiPlayer : ObservableObject
    {
        Byte kDefaultVelocity = 127;

        protected IMidiOutPort m_port;
        protected readonly Object m_lock = new Object();
        string m_deviceName;
        Byte m_note = 0;
        Byte[] m_notes = new Byte[256];

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

        public MidiPlayer(string deviceName, IMidiOutPort port)
        {
            Debug.Assert(port != null);
            m_port = port;
            m_deviceName = deviceName;
        }

        public virtual void SetupProgram()
        {
        }

        protected IBuffer StrHex2ByteStream(string hexStr)
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

        protected void NoteOn(Byte channel, Byte note)
        {
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOnMessage(channel, note, kDefaultVelocity);
                m_port.SendMessage(midiMsg);
                m_note = note;
                m_notes[channel] = note;
            }
            Debug.WriteLine("Note On " + note);
        }

        protected void NoteOff(Byte channel)
        {
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOffMessage(channel, m_note, kDefaultVelocity);
                m_port.SendMessage(midiMsg);
                m_notes[channel] = 0;
            }
            Debug.WriteLine("Note Off");
        }

        public void NoteOff()
        {
            lock (m_lock)
            {
                for (Byte i = 0; i < Byte.MaxValue; i++)
                {
                    if (m_notes[i] != 0)
                    {
                        var midiMsg = new MidiNoteOffMessage(i, m_note, kDefaultVelocity);
                        m_port.SendMessage(midiMsg);
                        m_notes[i] = 0;
                    }
                }
            }
            Debug.WriteLine("Note Off");
        }
    }
}
