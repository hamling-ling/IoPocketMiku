using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using IoPokeMikuClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace IoPokeMikuClient.ViewModel
{
    public class BleDeviceSelectionPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand StartSearchingCommand { get; private set; }
        public RelayCommand StopSearchingCommand { get; private set; }
        public RelayCommand SelectDeviceCommand { get; private set; }

        private ObservableCollection<DeviceInformation> m_collection = new ObservableCollection<DeviceInformation>();
        public ObservableCollection<DeviceInformation> DeviceList
        {
            get
            {
                //return ;
                return m_collection;
            }
            set
            {
                m_collection = value;
                RaisePropertyChanged("DeviceList");
            }
        }

        public DeviceInformation SelectedDevice
        {
            get; set;
        }

        public int SelectedIndex
        {
            get; set;
        }

        public string StatusMessage
        {
            get { return m_statusMessage; }
            set
            {
                if (m_statusMessage != value)
                {
                    m_statusMessage = value;
                    RaisePropertyChanged("StatusMessage");
                }
            }
        }

        private string m_statusMessage = string.Empty;

        public BleDeviceSelectionPageViewModel()
        {
            IoPokeMikuClientModel.Instance.BleDeviceWatcher.DeviceList.CollectionChanged += PortList_CollectionChanged;
            StartSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StartSearchingSourceDevice();
            });
            StopSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StopSearchingSourceDevice();
            });
            SelectDeviceCommand = new RelayCommand(async () =>
            {
                Debug.WriteLine(SelectedIndex);
                if(SelectedDevice == null)
                {
                    StatusMessage = "device not selected";
                    return;
                }
                bool selectResult = false;
                selectResult = await IoPokeMikuClientModel.Instance.SelectSourceDevice(SelectedDevice);
                if(!selectResult)
                {
                    StatusMessage = "device selection failed";

                    try
                    {
                        IoPokeMikuClientModel.Instance.StopSearchingSourceDevice();
                        IoPokeMikuClientModel.Instance.StartSearchingSourceDevice();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    return;
                }
                ViewModelLocator.Instance.NavigationService.NavigateTo("MainPage");
            });
        }

        private void PortList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var col = sender as ObservableCollection<DeviceInformation>;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                DeviceList = col;
            });
        }
    }
}
