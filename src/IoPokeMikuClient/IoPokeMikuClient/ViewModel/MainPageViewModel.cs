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
using Windows.UI.Core;

namespace IoPokeMikuClient.ViewModel
{
    public class MainPageViewModel : PokeMikuBaseViewModel
    {
        public RelayCommand<string> NoteOnCommand { get; set; }
        public RelayCommand NoteOffCommand { get; set; }
        public RelayCommand<string> SelectPlayerCommand { get; set; }
        public RelayCommand LoadDefaultValueCommand { get; set; }

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

        public double Freq
        {
            get { return m_freq; }
            private set
            {
                if (m_freq != value)
                {
                    m_freq = value;
                    RaisePropertyChanged("Freq");
                }
            }
        }

        public string SelectedMode
        {
            get { return m_mode;  }
            set
            {
                if(m_mode != value)
                {
                    m_mode = value;
                    RaisePropertyChanged("SelectedMode");
                }
            }
        }

        public ReadOnlyCollection<string> ModeList
        {
            get { return m_modeList; }
            set
            {
                if(m_modeList != value)
                {
                    m_modeList = value;
                    RaisePropertyChanged("ModeList");
                }
            }
        }

        private string m_noteStr = String.Empty;
        private double m_freq = 0.0f;
        private ReadOnlyCollection<string> m_modeList = new ReadOnlyCollection<string>(new []{ "MikuSolo", "Orchestra" });
        private string m_mode = string.Empty;

        private readonly INavigationService m_navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            m_navigationService = navigationService;

            NoteOnCommand = new RelayCommand<string>((w) =>
            {
                int x = 0;
                if (int.TryParse(w, out x))
                {
                    IoPokeMikuClientModel.Instance.PlayerSelector.Player.NoteOn((byte)(60 + x));
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
            LoadDefaultValueCommand = new RelayCommand(() =>
            {
                SelectedMode = m_modeList.FirstOrDefault<string>();
            });

            IoPokeMikuClientModel.Instance.PlayerSelector.Player.PropertyChanged += PokeMiku_PropertyChanged;
            IoPokeMikuClientModel.Instance.Source.DataReceived += Cloud_DataReceived;
        }

        private async void Cloud_DataReceived(object sender, CloudClientEventArgs args)
        {
            if (!DispatcherHelper.UIDispatcher.HasThreadAccess)
            {
                await DispatcherHelper.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Cloud_DataReceived(sender, args);
                });
                return;
            }

            Freq = args.PitchInfo.f;
        }

        private async void PokeMiku_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Note")
            {
                if(!DispatcherHelper.UIDispatcher.HasThreadAccess)
                {
                    await DispatcherHelper.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PokeMiku_PropertyChanged(sender, e);
                    });
                    
                    return;
                }

                var pokemiku = sender as MidiPlayer;
                Note = pokemiku.Note.ToString();
            }
        }
    }
}
