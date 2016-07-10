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
        public RelayCommand ConnectCommand { get; set; }

        private string m_noteStr = String.Empty;
        private readonly INavigationService m_navigationService;

        public CloudConnectionPageViewModel(INavigationService navigationService)
        {
            m_navigationService = navigationService;

            ConnectCommand = new RelayCommand(() =>
            {
                var result = IoPokeMikuClientModel.Instance.Cloud.Connect();
                if(result)
                {
                    ViewModelLocator.Instance.NavigationService.NavigateTo("MainPage");
                }
            });
        }
    }
}
