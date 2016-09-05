using GalaSoft.MvvmLight;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace IoPokeMikuClient.Model
{
    public class TunerSource : ObservableObject
    {
        public delegate void TunerEventHandler(object sender, CloudClientEventArgs args);

        public virtual event TunerEventHandler DataReceived;

        public TunerSource()
        {
        }

        public virtual async Task<bool> Connect(DeviceInformation device)
        {
            return true;
        }

        protected virtual void Connect(ConnectionInfo info)
        {
        }

        protected virtual void Publish(string topic, string msg)
        {
        }

        protected virtual void Subscribe(string topic)
        {
        }

        protected virtual void RaiseDataReceived(object sender, CloudClientEventArgs args)
        {
            var handler = DataReceived;
            if(handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
