using GalaSoft.MvvmLight.Threading;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace IoPokeMikuClient.Model
{
    public class BleClient : TunerSource
    {
        readonly string kUuidTunerSvc = "0000a000";
        readonly string kUuidFreqChar = "0000a001";
        readonly string kUuidGainChar = "0000a002";

        private DeviceInformation m_deviceInfo;
        private BluetoothLEDevice m_bleDevice;

        public BleClient()
        {
        }

        public override async Task<bool> Connect(DeviceInformation device)
        {
            var ret = false;
            var bleDevice = await ConnectInternal(device);
            if(bleDevice != null)
            {

                if(await Subscribe(bleDevice))
                {
                    m_bleDevice = bleDevice;
                    ret = true;
                    Debug.WriteLine("Connect succeed");
                }
            }
            return ret;
        }

        /// <summary>
        /// Connect to Ble device
        /// </summary>
        /// <remarks>call from UI thread</remarks>
        /// <param name="device"></param>
        /// <returns></returns>
        private async Task<BluetoothLEDevice> ConnectInternal(DeviceInformation device)
        {
            BluetoothLEDevice bleDevice = null;

            DevicePairingResult result = null;
            m_deviceInfo = device;
            if (!device.Pairing.IsPaired)
            {
                result = await device.Pairing.PairAsync();
                Debug.WriteLine("Pairing result = " + result.Status);

                if (result == null)
                {
                    Debug.WriteLine("not paired");
                    return null;
                }
                if (result.Status != DevicePairingResultStatus.Paired && result.Status != DevicePairingResultStatus.AlreadyPaired)
                {
                    Debug.WriteLine("not paired " + result.Status);
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("Already Paired");
            }

            try
            {
                bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x800710df)
            {
                Debug.WriteLine("get ble device failed");
            }

            if (bleDevice == null)
            {
                Debug.WriteLine("ble device failed");
            }

            Debug.WriteLine("bleDevice assigned " + bleDevice.DeviceId ?? "null");

            return bleDevice;
        }

        private async Task<bool> Subscribe(BluetoothLEDevice bleDevice)
        {
            var charact = GetFreqChar(bleDevice);
            if(charact == null)
            {
                return false;
            }

            var status = GattCommunicationStatus.Unreachable;
            try
            {
                status = await charact.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("failed to subscribe");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to subscribe");
            }

            if (status != GattCommunicationStatus.Success)
            {
                Debug.WriteLine("Error registering for notifications: " + status);
                return false;
            }

            bool ret = false;
            try
            {
                charact.ValueChanged += Charact_ValueChanged;
                Debug.WriteLine("subscribe succeed " + charact.Uuid);
                ret = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to subsctive " + charact.Uuid);
            }
            return ret;
        }

        private GattCharacteristic GetFreqChar(BluetoothLEDevice bleDevice)
        {
            if(bleDevice == null)
            {
                return null;
            }

            var tunerServices = from x in bleDevice.GattServices where x.Uuid.ToString().StartsWith(kUuidTunerSvc) select x;
            if (!tunerServices.Any())
            {
                return null;
            }

            var tuner = bleDevice.GattServices.FirstOrDefault(w => w.Uuid.StartsWith(kUuidTunerSvc));
            if (tuner == null)
            {
                return null;
            }
            var freqChars = tuner.GetAllCharacteristics().FirstOrDefault(w => w.Uuid.StartsWith(kUuidFreqChar));
            return freqChars;
        }

        private void Charact_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

            Debug.WriteLine(sender.Uuid + " value changed :");

            var byteArray = args.CharacteristicValue.ToArray();
            UInt16 freq = 0;
            if (byteArray.Length >= 2)
            {
                freq = BitConverter.ToUInt16(byteArray, 0);
                Debug.WriteLine(freq + "Hz");
            }

            var pitch = new PitchInfo() { f = freq, midi = 0 };
            var clientargs = new CloudClientEventArgs(pitch);
            RaiseDataReceived(this, clientargs);
        }

        private void GattEentArgs(GattValueChangedEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            var byteArray = args.CharacteristicValue.ToArray();
            foreach (var byteVal in byteArray)
            {
                builder.Append(String.Format("{0:X2} ", byteVal));
            }
            Debug.WriteLine(builder.ToString());
        }
    }
}
