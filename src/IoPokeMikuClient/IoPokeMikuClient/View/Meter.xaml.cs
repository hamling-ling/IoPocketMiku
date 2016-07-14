using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IoPokeMikuClient.View
{
    public sealed partial class Meter : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                typeof(double),
                                typeof(Meter),
                                new PropertyMetadata(0.0, new PropertyChangedCallback(OnValueChanged)));

        public static readonly DependencyProperty FreqProperty = DependencyProperty.Register("Freq",
                        typeof(double),
                        typeof(Meter),
                        new PropertyMetadata(0.0, new PropertyChangedCallback(OnFreqChanged)));

        /// <summary>
        /// range in 30 to 120
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Freq
        {
            get { return (double)GetValue(FreqProperty); }
            set { SetValue(FreqProperty, value); }
        }
        public Meter()
        {
            this.InitializeComponent();
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Meter self = obj as Meter;
            if (self != null)
            {
                self.MeterValueAnimation.To = e.NewValue as Double?;
                self.myStoryboard.Begin();
            }
        }

        private static void OnFreqChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Meter self = obj as Meter;
            if (self == null)
            {
                return;
            }
            Double? newVal = e.NewValue as Double?;
            if(newVal == null)
            {
                return;
            }


            double angle = 0.0;
            string noteText = string.Empty;
            if(newVal.Value != 0.0)
            {
                var midi = FreqToMidiNote(newVal.Value);
                var rightMidi = Math.Round(midi);
                angle = (midi - rightMidi) * (90.0 - 30.0) + 90.0;
                noteText = GetMidiNoteStr((int)rightMidi);
            } else
            {
                angle = 90.0;
                noteText = "";
            }


            self.MeterValueAnimation.To = angle;
            self.myStoryboard.Begin();
            self.NoteText.Text = noteText;
        }

        private static double FreqToMidiNote(double freq)
        {
            double midi = (12.0 / Math.Log10(2)) * Math.Log10(freq / 440.0) + 69.0;
            return midi;
        }

        private static string GetMidiNoteStr(int note)
        {
            string[] notes = new string[]
            {
                "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
            };
            int idx = note % notes.Count();
            return notes[idx];
        }
    }
}
