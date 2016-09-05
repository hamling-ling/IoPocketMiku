﻿using GalaSoft.MvvmLight.Command;
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
    public class MidiDeviceSelectionPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand StartSearchingCommand { get; private set; }
        public RelayCommand StopSearchingCommand { get; private set; }
        public RelayCommand SelectDeviceCommand { get; private set; }

        private ObservableCollection<DeviceInformation> m_collection = new ObservableCollection<DeviceInformation>();
        public ObservableCollection<DeviceInformation> PortList
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

        public MidiDeviceSelectionPageViewModel()
        {
            IoPokeMikuClientModel.Instance.MidiDeviceWatcher.DeviceList.CollectionChanged += PortList_CollectionChanged;
            StartSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StartSearchingMidiDevice();
            });
            StopSearchingCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.StopSearchingMidiDevice();
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
                selectResult = await IoPokeMikuClientModel.Instance.SelectMidiDevice(SelectedDevice);
                if(!selectResult)
                {
                    StatusMessage = "device selection failed";
                    return;
                }
                if (IoPokeMikuClientModel.Instance.SourceKind == SourceKind.CloudSource)
                {
                    ViewModelLocator.Instance.NavigationService.NavigateTo("CloudConnectionPage");
                }
                else
                {
                    ViewModelLocator.Instance.NavigationService.NavigateTo("BleSelectionPage");
                }
            });
        }

        private async void PortList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var col = sender as ObservableCollection<DeviceInformation>;
            await DispatcherHelper.RunAsync(() => {
                PortList = col;
            });
        }
    }
}
