using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace IoPokeMikuClient.Model
{
    public enum PlayerKind
    {
        MikuSolo,
        MikuChorus,
        Orchestra,
    }

    public class PlayerSelector
    {
        public MidiPlayer Player { get; private set; }

        private string m_deviceName;
        private IMidiOutPort m_port;

        public PlayerSelector(string deviceName, IMidiOutPort port)
        {
            m_deviceName = deviceName;
            m_port = port;
        }

        public void SelectPlayer(PlayerKind playerKind)
        {
            MidiPlayer nextPlayer = null;
            if(PlayerKind.MikuSolo == playerKind)
            {
                nextPlayer = new PokeMiku(m_deviceName, m_port);
            } else if(PlayerKind.MikuChorus == playerKind)
            {
                // t.b.d
            } else if(PlayerKind.Orchestra == playerKind)
            {
                nextPlayer = new Orchestra(m_deviceName, m_port);
            } else
            {
                nextPlayer = new PokeMiku(m_deviceName, m_port);
            }

            if(Player != null)
            {
                Player.NoteOff();
            }

            Player = nextPlayer;
            Player.SetupProgram();
        }
    }
}
