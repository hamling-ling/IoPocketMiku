using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace IoPokeMikuClient.Model
{
    public class BleDeviceWatcher
    {
        static readonly string kDeviceNamePrefix = "KurumiTuner";

        ObservableCollection<DeviceInformation> m_deviceList = null;

        private DeviceWatcher m_watcher;

        public ObservableCollection<DeviceInformation> DeviceList
        {
            get {
                return m_deviceList;
            }
        }

        internal BleDeviceWatcher()
        {
            m_deviceList = new ObservableCollection<DeviceInformation>();
        }

        ~BleDeviceWatcher()
        {

        }

        internal void Start()
        {
            if (m_watcher != null)
            {
                StopBleDeviceWatcher();
            }

            StartBleDeviceWatcher();

            Debug.WriteLine("Device watcher started.");
        }

        internal void Stop()
        {
            StopBleDeviceWatcher();
        }

        private void StopBleDeviceWatcher()
        {
            if (m_watcher != null)
            {
                // Unregister the event handlers.
                m_watcher.Added -= DeviceWatcher_Added;
                m_watcher.Updated -= DeviceWatcher_Updated;
                m_watcher.Removed -= DeviceWatcher_Removed;
                m_watcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                m_watcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                m_watcher.Stop();
                m_watcher = null;
            }

            Debug.WriteLine("Start enumerating");
        }

        /// <summary>
        ///     Starts a device watcher that looks for all nearby BT devices (paired or unpaired). Attaches event handlers and
        ///     populates the collection of devices.
        /// </summary>
        private void StartBleDeviceWatcher()
        {
            Debug.WriteLine("Stop enumerating");

            // Additional properties we would like about the device.
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            // BT_Code: Currently Bluetooth APIs don't provide a selector to get ALL devices that are both paired and non-paired.
            m_watcher =
                    DeviceInformation.CreateWatcher(
                        "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")",
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            m_watcher.Added += DeviceWatcher_Added;
            m_watcher.Updated += DeviceWatcher_Updated;
            m_watcher.Removed += DeviceWatcher_Removed;
            m_watcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            m_watcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            m_watcher.Start();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            if (sender == m_watcher)
            {
                Debug.WriteLine(deviceInfo.Name + " " + deviceInfo.Id + " added");
                if (deviceInfo.Name.Contains(kDeviceNamePrefix))
                {
                    m_deviceList.Add(deviceInfo);
                }
            }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            if (sender == m_watcher)
            {
                Debug.WriteLine(deviceInfoUpdate.Id + " updated");
            }
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            if (sender == m_watcher)
            {
                var items = from x in m_deviceList where x.Id == deviceInfoUpdate.Id select x;
                if (items.Any())
                {
                    m_deviceList.Remove(items.First());
                    Debug.WriteLine(deviceInfoUpdate.Id + " removed");
                }
            }
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            if (sender == m_watcher)
            {
                Debug.WriteLine("enumeration complete.");
            }
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            if (sender == m_watcher)
            {
                Debug.Write("No longer watching for devices. " + sender.Status);
            }
        }

    }
}
