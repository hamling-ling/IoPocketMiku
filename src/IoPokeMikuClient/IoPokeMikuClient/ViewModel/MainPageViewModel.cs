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
        public RelayCommand<string> NoteOnCommand { get; set; }
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

            NoteOnCommand = new RelayCommand<string>((w) =>
            {
                int x = 0;
                if (int.TryParse(w, out x))
                {
                    IoPokeMikuClientModel.Instance.PokeMiku.NoteOn((byte)(64 + x));
                }
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

                var pokemiku = sender as MidiPlayer;
                Note = pokemiku.Note.ToString();
            }
        }
    }
}
