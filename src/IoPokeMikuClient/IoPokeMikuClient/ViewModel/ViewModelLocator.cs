using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoPokeMikuClient.View;
using IoPokeMikuClient.ViewModel;

namespace IoPokeMikuClient.ViewModel
{
    public class ViewModelLocator
    {
        private static ViewModelLocator s_instance = new ViewModelLocator();

        public static ViewModelLocator Instance {  get { return s_instance; } }

        private ViewModelLocator()
        {
            Initialize();
        }

        private void Initialize()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<MidiDeviceSelectionPageViewModel>();
            SimpleIoc.Default.Register<CloudConnectionPageViewModel>();
            SimpleIoc.Default.Register<BleDeviceSelectionPageViewModel>();
        }

        private static INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("MainPage", typeof(MainPage));
            navigationService.Configure("MidiDeviceSelectionPage", typeof(MidiDeviceSelectionPage));
            navigationService.Configure("CloudConnectionPage", typeof(CloudConnectionPage));
            navigationService.Configure("BleSelectionPage", typeof(BleSelectionPage));
            return navigationService;
        }

        public MainPageViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainPageViewModel>(); }
        }

        public MidiDeviceSelectionPageViewModel MidiDeviceSelection
        {
            get { return ServiceLocator.Current.GetInstance<MidiDeviceSelectionPageViewModel>(); }
        }

        public CloudConnectionPageViewModel CloudConnection
        {
            get { return ServiceLocator.Current.GetInstance<CloudConnectionPageViewModel>(); }
        }

        public BleDeviceSelectionPageViewModel BleSelection
        {
            get { return ServiceLocator.Current.GetInstance<BleDeviceSelectionPageViewModel>(); }
        }

        public INavigationService NavigationService
        {
            get { return ServiceLocator.Current.GetInstance<INavigationService>(); }
        }
    }
}
