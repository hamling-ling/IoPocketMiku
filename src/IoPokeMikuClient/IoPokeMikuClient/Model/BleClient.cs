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
        readonly string kUuidTunerSvcEnd = "90FF";
        readonly string kUuidFreqCharEnd = "9033";
        readonly string kUuidGainCharEnd = "9034";

        private readonly object m_deviceInfoLock = new object();
        private DeviceInformation m_deviceInfo;
        private BluetoothLEDevice m_bleDevice;

        private GattCharacteristic m_freqChar;
        private GattDeviceService m_tunerSvc;

        public DeviceInformation DeviceInfo
        {
            get { return m_deviceInfo; }
        }

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
                } else
                {
                    Debug.WriteLine("Connect failed");
                }
            }
            return ret;
        }

        public override void UpdateDeviceInfo(DeviceInformation device)
        {
            lock (m_deviceInfoLock)
            {
                m_deviceInfo = device;
            }
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
            lock (m_deviceInfoLock)
            {
                m_deviceInfo = device;
            }
            if (device.Pairing.IsPaired)
            {
                await  device.Pairing.UnpairAsync();
                Debug.WriteLine("unpaired");
                device = await DeviceInformation.CreateFromIdAsync(device.Id);
                lock (m_deviceInfoLock)
                {
                    m_deviceInfo = device;
                }
                return null;
            }

            if (!device.Pairing.IsPaired)
            {
                var pairintKinds = DevicePairingKinds.ConfirmOnly;
                var protectionLevel = DevicePairingProtectionLevel.None;
                var customPairing = device.Pairing.Custom;

                customPairing.PairingRequested += CustomPairing_PairingRequested;
                result = await customPairing.PairAsync(pairintKinds, protectionLevel);
                customPairing.PairingRequested -= CustomPairing_PairingRequested;

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

            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                try
                {
                    Debug.WriteLine("trying to open service ... (" + i + "/5)");
                    bleDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
                    if (bleDevice != null)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("get ble device failed");
                }

            }
            if (bleDevice == null)
            {
                Debug.WriteLine("ble device failed");
                return null;
            }

            Debug.WriteLine("bleDevice assigned " + bleDevice.DeviceId ?? "null");

            return bleDevice;
        }

        private void CustomPairing_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            switch (args.PairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                    args.Accept();
                    Debug.WriteLine("pairing request Accept");
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> Subscribe(BluetoothLEDevice bleDevice)
        {
            GattCharacteristic charact = null;
            for (int i = 0; i < 10; i++)
            {
                Debug.WriteLine("trying to open characteristic ... (" + i + "/10)");
                charact = GetFreqChar(bleDevice);
                if (charact != null)
                {
                    break;
                }
                await Task.Delay(1000);
                bleDevice = await BluetoothLEDevice.FromIdAsync(bleDevice.DeviceId);
                if(bleDevice == null)
                {
                    return false;
                }
            }
            if (charact == null)
            {
                Debug.WriteLine("could not get characteristic");
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
                Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to subscribe.");
                Debug.WriteLine(ex.ToString());
            }

            if (status != GattCommunicationStatus.Success)
            {
                Debug.WriteLine("Error registering for notifications: " + status);
                return false;
            }

            bool ret = false;
            try
            {
                charact.ValueChanged -= Charact_ValueChanged;
                charact.ValueChanged += Charact_ValueChanged;
                m_freqChar = charact;
                Debug.WriteLine("subscribe succeed " + charact.Uuid);
                ret = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to subscribe " + charact.Uuid);
                Debug.WriteLine(ex.ToString());
            }
            return ret;
        }

        private GattCharacteristic GetFreqChar(BluetoothLEDevice bleDevice)
        {
            if(bleDevice == null)
            {
                return null;
            }
            // debug
            var services = bleDevice.GattServices;
            foreach(var x in services)
            {
                Debug.WriteLine("uuid=" + x.Uuid);
            }
            var tunerServices = from x in bleDevice.GattServices where x.Uuid.EndsWith(kUuidTunerSvcEnd) select x;
            if (!tunerServices.Any())
            {
                Debug.WriteLine("no tuner services");
                return null;
            }

            Debug.WriteLine("services count=" + services.Count());

            GattCharacteristic freqChars = null;

            foreach (var tuner in services)
            {
                var allChars = tuner.GetAllCharacteristics();
                foreach (var x in allChars)
                {
                    Debug.WriteLine("chars " + x.Uuid);
                }

                freqChars = tuner.GetAllCharacteristics().FirstOrDefault(w => w.Uuid.EndsWith(kUuidFreqCharEnd));
                if (freqChars != null)
                {
                    m_tunerSvc = tuner;
                    break;
                }
            }
            return freqChars;
        }

        private void Charact_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {

            Debug.WriteLine(sender.Uuid + " value changed :");
            PrintGattEentArgs(args);

            var byteArray = args.CharacteristicValue.ToArray();
            UInt16 freq = 0;
            UInt16 midiNote = 0;
            if (byteArray.Length >= 4)
            {
                freq = (UInt16)((byteArray[0] << 8) | byteArray[1]);
                midiNote = (UInt16)((byteArray[2] << 8) | byteArray[3]);
                Debug.WriteLine(freq + "Hz, midi=" + midiNote);
            }

            var pitch = new PitchInfo() { f = freq, midi = midiNote };
            var clientargs = new CloudClientEventArgs(pitch);
            RaiseDataReceived(this, clientargs);
        }

        private void PrintGattEentArgs(GattValueChangedEventArgs args)
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
