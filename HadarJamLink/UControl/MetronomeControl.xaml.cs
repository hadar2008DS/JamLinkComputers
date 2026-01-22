using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using NAudio.Wave;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for MetronomeControl.xaml
    /// </summary>
    public partial class MetronomeControl : UserControl
    {
        private CancellationTokenSource cts;
        private OutputDevice outputDevice;
        private int currentBpm;
        private int beatCount = 0;
        private int beatsPerMeasure = 4; 
        private int currentBeat = 0;
        public MetronomeControl()
        {
            InitializeComponent();

            // אתחול OutputDevice ראשון
            var devices = OutputDevice.GetAll();
            if (devices.Count > 0)
                outputDevice = OutputDevice.GetAll().FirstOrDefault();

            currentBpm = (int)BpmSlider.Value;

            BpmSlider.ValueChanged += (s, e) =>
            {
                currentBpm = (int)BpmSlider.Value;
                BpmLabel.Text = currentBpm.ToString();
            };

            TimeSignatureCombo.SelectionChanged += (s, e) =>
            {
                var selected = (TimeSignatureCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
                beatsPerMeasure = selected switch
                {
                    "3/4" => 3,
                    "6/8" => 6,
                    _ => 4
                };
                currentBeat = 0;
            };

            StartButton.Click += StartButton_Click;
            StopButton.Click += StopButton_Click;
        }
        private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (cts != null) return;
            cts = new CancellationTokenSource();
            Task.Run(() => RunMetronome(cts.Token));
        }

        private void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            cts?.Cancel();
            cts = null;
            Dispatcher.Invoke(() => BeatEllipse.Fill = Brushes.LightGray);
        }

        private void RunMetronome(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int interval = 60000 / currentBpm;

                PlayClick();
                FlashEllipse();

                Thread.Sleep(interval);
            }
        }

        private void PlayClick()
        {
            Task.Run(() =>
            {
                int sampleRate = 44100;
                short amplitude = 1000;
                double frequency = (currentBeat == 0) ? 1000 : 800;
                int durationMs = 50;

                var buffer = new byte[sampleRate * durationMs / 1000 * 2];
                for (int i = 0; i < buffer.Length / 2; i++)
                {
                    short sample = (short)(amplitude * Math.Sin(2 * Math.PI * frequency * i / sampleRate));
                    buffer[i * 2] = (byte)(sample & 0xff);
                    buffer[i * 2 + 1] = (byte)((sample >> 8) & 0xff);
                }

                using var ms = new System.IO.MemoryStream(buffer);
                using var rdr = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, 16, 1));
                using var wo = new WaveOutEvent();
                wo.Init(rdr);
                wo.Play();
                while (wo.PlaybackState == PlaybackState.Playing) Thread.Sleep(5);
            });

            currentBeat++;
            if (currentBeat >= beatsPerMeasure)
                currentBeat = 0;
        }

        private void FlashEllipse()
        {
            Dispatcher.Invoke(() =>
            {
                // צבע לפי Beat
                BeatEllipse.Fill = (currentBeat == 0) ? Brushes.Green : Brushes.Gray;
            });

            Task.Delay(100).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => BeatEllipse.Fill = Brushes.LightGray);
            });
        }

    }
}
