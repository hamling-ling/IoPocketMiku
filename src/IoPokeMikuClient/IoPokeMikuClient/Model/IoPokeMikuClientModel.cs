﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace IoPokeMikuClient.Model
{
    public enum SourceKind
    {
        CloudSource,
        BleSource
    }

    public sealed class IoPokeMikuClientModel
    {
        private static IoPokeMikuClientModel s_instance = new IoPokeMikuClientModel();

        public static IoPokeMikuClientModel Instance { get { return s_instance; } }

        public BleDeviceWatcher BleDeviceWatcher { get; private set; }
        public MidiDeviceWatcher MidiDeviceWatcher { get; private set; }
        public PlayerSelector PlayerSelector { get; private set; }
        public TunerSource Source { get; private set; }

        public SourceKind SourceKind { get; private set;}

        private IoPokeMikuClientModel()
        {
            MidiDeviceWatcher = new MidiDeviceWatcher();
            BleDeviceWatcher = new BleDeviceWatcher();
        }

        public bool Initialize(SourceKind source)
        {
            if (source == SourceKind.CloudSource)
            {
                Source = new CloudClient();
            }
            else
            {
                Source = new BleClient();
            }
            SourceKind = source;

            return true;
        }

        public void Cleanup()
        {
            MidiDeviceWatcher.Stop();
        }

        public bool StartSearchingMidiDevice()
        {
            MidiDeviceWatcher.Start();
            return true;
        }

        public void StopSearchingMidiDevice()
        {
            MidiDeviceWatcher.Stop();
        }

        public bool StartSearchingSourceDevice()
        {
            if(SourceKind != SourceKind.BleSource)
            {
                throw new NotImplementedException();
            }

            BleDeviceWatcher.Start();
            return true;
        }

        public void StopSearchingSourceDevice()
        {
            if (SourceKind != SourceKind.BleSource)
            {
                throw new NotImplementedException();
            }

            BleDeviceWatcher.Stop();
        }

        public async Task<bool> SelectMidiDevice(DeviceInformation device)
        {
            if(device == null)
            {
                // make it erro or release device ?
                return false;
            }

            var devices = MidiDeviceWatcher.GetDeviceInformationCollection();
            var freshDevice = devices.FirstOrDefault(w => w.Id == device.Id);
            if(freshDevice == null)
            {
                Debug.WriteLine("device " + device.Name + " not found");
                return false;
            }

            IMidiOutPort port = await MidiOutPort.FromIdAsync(device.Id);
            if(port == null)
            {
                return false;
            }

            PlayerSelector = new PlayerSelector(freshDevice.Name, port);
            PlayerSelector.SelectPlayer(PlayerKind.MikuSolo);

            Source.DataReceived += Cloud_DataReceived;

            return true;
        }

        public async Task<bool> SelectSourceDevice(DeviceInformation device)
        {
            if (SourceKind != SourceKind.BleSource)
            {
                throw new NotImplementedException();
            }

            bool result = false;
            try
            {
                result = await Source.Connect(device);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result;
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
