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
    public class MainPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand NoteOnCommand { get; set; }
        public RelayCommand NoteOffCommand { get; set; }

        public string DeviceName
        {
            get { return IoPokeMikuClientModel.Instance.PokeMiku.DeviceName; }
        }

        public string Note
        {
            get { return m_noteStr; }
            private set
            {
                if(m_noteStr != value)
                {
                    m_noteStr = value;
                    RaisePropertyChanged("Note");
                }
            }
        }

        private string m_noteStr = String.Empty;
        private readonly INavigationService m_navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            m_navigationService = navigationService;

            NoteOnCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.PokeMiku.NoteOn(64);
            });
            NoteOffCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.PokeMiku.NoteOff();
            });
            IoPokeMikuClientModel.Instance.PokeMiku.PropertyChanged += PokeMiku_PropertyChanged;
        }

        private void PokeMiku_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Note")
            {
                if(!DispatcherHelper.UIDispatcher.HasThreadAccess)
                {
                    PokeMiku_PropertyChanged(sender, e);
                    return;
                }

                var pokemiku = sender as PokeMiku;
                Note = pokemiku.Note.ToString();
            }
        }
    }
}
