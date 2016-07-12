using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace IoPokeMikuClient.Model
{
    public sealed class IoPokeMikuClientModel
    {
        private static IoPokeMikuClientModel s_instance = new IoPokeMikuClientModel();

        public static IoPokeMikuClientModel Instance { get { return s_instance; } }

        public MidiDeviceWatcher MidiDeviceWatcher { get; private set;}
        public PlayerSelector PlayerSelector { get; private set; }
        public CloudClient Cloud { get; private set; }

        private IoPokeMikuClientModel()
        {
            MidiDeviceWatcher = new MidiDeviceWatcher();
            Cloud = new CloudClient();
        }

        public bool Initialize()
        {
            return true;
        }

        public void Cleanup()
        {
            MidiDeviceWatcher.Stop();
        }

        public bool StartSearchingDevice()
        {
            MidiDeviceWatcher.Start();
            return true;
        }

        public void StopSearchingDevice()
        {
            MidiDeviceWatcher.Stop();
        }

        public async Task<bool> SelectMidiDevice(string deviceName)
        {
            if(deviceName == null)
            {
                // make it erro or release device ?
                return false;
            }

            var devices = MidiDeviceWatcher.GetDeviceInformationCollection();
            var device = devices.FirstOrDefault(w => w.Name == deviceName);
            if(device == null)
            {
                Debug.WriteLine("device " + deviceName + " not found");
                return false;
            }

            IMidiOutPort port = await MidiOutPort.FromIdAsync(device.Id);
            if(port == null)
            {
                return false;
            }

            PlayerSelector = new PlayerSelector(deviceName, port);
            PlayerSelector.SelectPlayer(PlayerKind.MikuSolo);

            Cloud.DataReceived += Cloud_DataReceived;

            return true;
        }

        public bool ChangePlayer(PlayerKind playerKind)
        {
            if(PlayerSelector == null)
            {
                return false;
            }

            PlayerSelector.SelectPlayer(playerKind);
            return true;
        }

        private void Cloud_DataReceived(object sender, CloudClientEventArgs args)
        {
            var player = PlayerSelector.Player;
            if(player == null)
            {
                return;
            }

            if (args.PitchInfo.midi == 0)
            {
                player.NoteOff();
            }
            else
            {
                player.NoteOn((byte)args.PitchInfo.midi);
            }
        }
    }
}
