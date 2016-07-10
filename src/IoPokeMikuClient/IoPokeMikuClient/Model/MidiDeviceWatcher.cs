//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace IoPokeMikuClient.Model
{
    /// <summary>
    /// DeviceWatcher class to monitor adding/removing MIDI devices on the fly
    /// </summary>
    public class MidiDeviceWatcher
    {
        internal DeviceWatcher m_deviceWatcher = null;
        internal DeviceInformationCollection m_deviceInformationCollection = null;
        bool m_enumerationCompleted = false;
        ObservableCollection<string> m_portList = null;
        readonly string m_midiSelector;

        public ObservableCollection<string> PortList { get { return m_portList; } }

        /// <summary>
        /// Constructor: Initialize and hook up Device Watcher events
        /// </summary>
        /// <param name="midiSelectorString">MIDI Device Selector</param>
        internal MidiDeviceWatcher()
        {
            m_midiSelector = MidiOutPort.GetDeviceSelector();
            m_deviceWatcher = DeviceInformation.CreateWatcher(m_midiSelector);
            m_portList = new ObservableCollection<string>();

            m_deviceWatcher.Added += DeviceWatcher_Added;
            m_deviceWatcher.Removed += DeviceWatcher_Removed;
            m_deviceWatcher.Updated += DeviceWatcher_Updated;
            m_deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        /// <summary>
        /// Destructor: Remove Device Watcher events
        /// </summary>
        ~MidiDeviceWatcher()
        {
            m_deviceWatcher.Added -= DeviceWatcher_Added;
            m_deviceWatcher.Removed -= DeviceWatcher_Removed;
            m_deviceWatcher.Updated -= DeviceWatcher_Updated;
            m_deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
        }

        /// <summary>
        /// Start the Device Watcher
        /// </summary>
        internal void Start()
        {
            if(m_deviceWatcher.Status != DeviceWatcherStatus.Started)
            {
                m_deviceWatcher.Start();
            }
        }

        /// <summary>
        /// Stop the Device Watcher
        /// </summary>
        internal void Stop()
        {
            if(m_deviceWatcher.Status != DeviceWatcherStatus.Stopped)
            {
                m_deviceWatcher.Stop();
            }
        }

        /// <summary>
        /// Get the DeviceInformationCollection
        /// </summary>
        /// <returns></returns>
        internal DeviceInformationCollection GetDeviceInformationCollection()
        {
            return m_deviceInformationCollection;
        }

        /// <summary>
        /// Add any connected MIDI devices to the list
        /// </summary>
        private async void UpdateDevices()
        {
            // Get a list of all MIDI devices
            m_deviceInformationCollection = await DeviceInformation.FindAllAsync(m_midiSelector);

            // If no devices are found, update the ListBox
            if ((m_deviceInformationCollection == null) || (m_deviceInformationCollection.Count == 0))
            {
                // Start with a clean list
                m_portList.Clear();

                m_portList.Add("No MIDI ports found");
            }
            // If devices are found, enumerate them and add them to the list
            else
            {
                // Start with a clean list
                m_portList.Clear();

                foreach (var device in m_deviceInformationCollection)
                {
                    m_portList.Add(device.Name);
                }
            }
        }

        /// <summary>
        /// Update UI on device added
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // If all devices have been enumerated
            if (m_enumerationCompleted)
            {
                UpdateDevices();
            }
        }

        /// <summary>
        /// Update UI on device removed
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // If all devices have been enumerated
            if (m_enumerationCompleted)
            {
                UpdateDevices();
            }
        }

        /// <summary>
        /// Update UI on device updated
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // If all devices have been enumerated
            if (m_enumerationCompleted)
            {
                UpdateDevices();
            }
        }

        /// <summary>
        /// Update UI on device enumeration completed.
        /// </summary>
        /// <param name="sender">The active DeviceWatcher instance</param>
        /// <param name="args">Event arguments</param>
        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            m_enumerationCompleted = true;
            UpdateDevices();
        }
    }
}
