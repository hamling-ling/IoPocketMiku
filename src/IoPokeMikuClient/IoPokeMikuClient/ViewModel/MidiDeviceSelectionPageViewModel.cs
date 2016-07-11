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

namespace IoPokeMikuClient.ViewModel
{
    public class MidiDeviceSelectionPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand StartSearchingCommand { get; private set; }
        public RelayCommand StopSearchingCommand { get; private set; }
        public RelayCommand SelectDeviceCommand { get; private set; }

        private ObservableCollection<string> m_collection = new ObservableCollection<string>();
        public ObservableCollection<string> PortList
        {
            get
            {
                //return ;
                return m_collection;
            }
            set
            {
                m_collection = value;
                RaisePropertyChanged("PortList");
            }
        }

        public string SelectedDeviceName
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

        public MidiDeviceSelectionPageViewModel()
        {
            IoPokeMikuClientModel.Instance.MidiDeviceWatcher.PortList.CollectionChanged += PortList_CollectionChanged;
            StartSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StartSearchingDevice();
            });
            StopSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StopSearchingDevice();
            });
            SelectDeviceCommand = new RelayCommand(async () =>
            {
                Debug.WriteLine(SelectedIndex);
                if(SelectedDeviceName == null)
                {
                    StatusMessage = "device not selected";
                    return;
                }
                bool selectResult = false;
                selectResult = await IoPokeMikuClientModel.Instance.SelectMidiDevice(SelectedDeviceName);
                if(!selectResult)
                {
                    StatusMessage = "device selection failed";
                    return;
                }
                ViewModelLocator.Instance.NavigationService.NavigateTo("CloudConnectionPage");
            });
        }

        private async void PortList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var col = sender as ObservableCollection<string>;
            await DispatcherHelper.RunAsync(() => {
                PortList = new ObservableCollection<string>(col);
            });
        }
    }
}
