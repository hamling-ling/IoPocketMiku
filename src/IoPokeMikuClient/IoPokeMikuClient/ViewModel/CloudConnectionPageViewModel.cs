using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using IoPokeMikuClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.ViewModel
{
    public class CloudConnectionPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand LoadInfoCommand { get; set; }
        public RelayCommand ConnectCommand { get; set; }

        public string AppId
        {
            get
            {
                return m_appId;
            }
            private set
            {
                if (m_appId != value)
                {
                    m_appId = value;
                    RaisePropertyChanged("AppId");
                }
            }
        }

        public string DataStore
        {
            get
            {
                return m_dataStore;
            }
            private set
            {
                if (m_dataStore != value)
                {
                    m_dataStore = value;
                    RaisePropertyChanged("DataStore");
                }
            }
        }

        private readonly INavigationService m_navigationService;
        private string m_appId = string.Empty;
        private string m_dataStore = string.Empty;

        public CloudConnectionPageViewModel(INavigationService navigationService)
        {
            m_navigationService = navigationService;

            LoadInfoCommand = new RelayCommand(async () =>
            {
                var cloud = IoPokeMikuClientModel.Instance.Source as CloudClient;
                if(cloud == null)
                {
                    return;
                }

                var info = await cloud.LoadSetting();
                if(info.IsEmpty)
                {
                    AppId = info.AppId;
                    DataStore = info.DataStore;
                }
                AppId = "catipxwt08x";
                DataStore = "test1";
            });

            ConnectCommand = new RelayCommand(async () =>
            {
                if(string.IsNullOrEmpty(AppId) || string.IsNullOrEmpty(DataStore))
                {
                    return;
                }

                var cloud = IoPokeMikuClientModel.Instance.Source as CloudClient;
                if (cloud == null)
                {
                    return;
                }

                bool result = false;
                result = await cloud.SaveSetting(new ConnectionInfo(AppId, DataStore));
                if(!result)
                {
                    return;
                }

                result = await cloud.Connect(null);
                if(!result)
                {
                    return;
                }
                ViewModelLocator.Instance.NavigationService.NavigateTo("MainPage");
            });
        }
    }
}
