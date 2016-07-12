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
        public RelayCommand<string> SelectPlayerCommand { get; set; }

        public string DeviceName
        {
            get { return IoPokeMikuClientModel.Instance.PlayerSelector.Player.DeviceName; }
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
                    IoPokeMikuClientModel.Instance.PlayerSelector.Player.NoteOn((byte)(64 + x));
                }
            });
            NoteOffCommand = new RelayCommand(() =>
            {
                IoPokeMikuClientModel.Instance.PlayerSelector.Player.NoteOff();
            });
            SelectPlayerCommand = new RelayCommand<string>((w) =>
            {
                PlayerKind kind = PlayerKind.MikuSolo;
                if (w == "MikuSolo")
                {
                    kind = PlayerKind.MikuSolo;
                } else if(w == "MikuChorus")
                {
                    kind = PlayerKind.MikuChorus;
                } else if(w == "Orchestra")
                {
                    kind = PlayerKind.Orchestra;
                }
                IoPokeMikuClientModel.Instance.ChangePlayer(kind);
            });
            IoPokeMikuClientModel.Instance.PlayerSelector.Player.PropertyChanged += PokeMiku_PropertyChanged;
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
