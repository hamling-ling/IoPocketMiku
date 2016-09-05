using GalaSoft.MvvmLight.Threading;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace IoPokeMikuClient.Model
{
    public class BleClient : TunerSource
    {
        readonly string kUuidA001Str = "0000a001";
        readonly string kUuidA002Str = "0000a002";

        private DeviceInformation m_deviceInfo;
        private BluetoothLEDevice m_bleDevice;

        public BleClient()
        {
        }

        public override async Task<bool> Connect(DeviceInformation device)
        {
            DevicePairingResult result = null;
            await DispatcherHelper.RunAsync(async () =>
            {
                m_deviceInfo = device;
                if (!device.Pairing.IsPaired)
                {
                    result = await device.Pairing.PairAsync();
                    Debug.WriteLine("Pairing result = " + result.Status);

                    if (result == null)
                    {
                        Debug.WriteLine("not paired");
                        return;
                    }
                    if (result.Status != DevicePairingResultStatus.Paired && result.Status != DevicePairingResultStatus.AlreadyPaired)
                    {
                        Debug.WriteLine("not paired " + result.Status);
                        return;
                    }
                }

                BluetoothLEDevice bleDevice = null;
                try
                {
                    bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
                }
                catch (Exception ex) when ((uint)ex.HResult == 0x800710df)
                {
                    Debug.WriteLine("get ble device failed");
                    return;
                }

                if (bleDevice == null)
                {
                    Debug.WriteLine("ble device failed");
                    return;
                }

                Debug.WriteLine("bleDevice assigned " + bleDevice.DeviceId ?? "null");
                m_bleDevice = bleDevice;
            });

            return true;
        }
    }
}
