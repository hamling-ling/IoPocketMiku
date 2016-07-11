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
        #region Fields

        public static readonly Byte kDefaultVelocity = 127;

        protected IMidiOutPort m_port;
        protected readonly Object m_lock = new Object();
        string m_deviceName;
        Byte m_note = 0;
        Byte[] m_channels = new Byte[Byte.MaxValue];

        #endregion

        #region Properties

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

        #endregion

        #region Ctor

        public MidiPlayer(string deviceName, IMidiOutPort port)
        {
            Debug.Assert(port != null);
            m_port = port;
            m_deviceName = deviceName;
        }

        #endregion

        #region Public Methods

        public virtual void SetupProgram()
        {
        }

        public virtual void NoteOn(Byte note)
        {
            NoteOn(0, note, kDefaultVelocity);
        }

        public void NoteOff()
        {
            lock (m_lock)
            {
                for (Byte i = 0; i < m_channels.Count(); i++)
                {
                    if (m_channels[i] != 0)
                    {
                        var midiMsg = new MidiNoteOffMessage(i, m_channels[i], 0);
                        m_port.SendMessage(midiMsg);
                        m_channels[i] = 0;
                    }
                }
            }
            Note = 0;
            Debug.WriteLine("Note Off");
        }

        #endregion

        #region Protected Methods

        protected void NoteOn(Byte channel, Byte note, Byte velocity)
        {
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOnMessage(channel, note, velocity);
                m_port.SendMessage(midiMsg);
                m_note = note;
                m_channels[channel] = note;
            }
            Debug.WriteLine("Note On " + note);
        }

        protected void NoteOff(Byte channel)
        {
            bool allOff = false;
            lock(m_lock)
            {
                var midiMsg = new MidiNoteOffMessage(channel, m_note, kDefaultVelocity);
                m_port.SendMessage(midiMsg);
                m_channels[channel] = 0;
                allOff = !m_channels.Any(w => w != 0);
            }

            if(allOff)
            {
                Note = 0;
            }
            Debug.WriteLine("Note Off");
        }

        #endregion
    }
}
