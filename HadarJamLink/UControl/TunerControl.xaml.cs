using NAudio.Wave;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for TunerControl.xaml
    /// </summary>
    /// 

    public partial class TunerControl : UserControl
    {
        private WaveInEvent waveIn;

        // תווים בסיסיים A-G עם תדרים לדוגמה (A4 = 440Hz)
        private Dictionary<string, float> notes = new Dictionary<string, float>()
        {
            {"A", 440f},
            {"B", 493.88f},
            {"C", 261.63f},
            {"D", 293.66f},
            {"E", 329.63f},
            {"F", 349.23f},
            {"G", 392.00f}
        };


        public TunerControl()
        {
            InitializeComponent();
            StartMicrophone();

        }
        private void StartMicrophone()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                float freq = GetFrequency(e.Buffer);
                string closestNote = GetClosestNote(freq);
                float idealFreq = notes[closestNote];

                Dispatcher.Invoke(() =>
                {
                    NoteText.Text = closestNote;
                    float delta = (freq - idealFreq) / idealFreq;

                    if (Math.Abs(delta) < 0.02f)
                    {
                        TuningArrow.RenderTransform = new RotateTransform(0);
                        TuningArrow.Fill = Brushes.Green;
                    }
                    else
                    {
                        TuningArrow.RenderTransform = new RotateTransform(delta * 90);
                        TuningArrow.Fill = Brushes.Red;
                    }
                });
            }
            catch (TaskCanceledException)
            {
                // כאן מתעלמים מהמקרה שהטסק בוטל
            }
            catch (Exception ex)
            {
                //  לטפל בשגיאות אחרות
                Console.WriteLine(ex.Message);
            }
        }

        // חישוב תדר בקירוב דרך zero crossing
        private float GetFrequency(byte[] buffer)
        {
            int sampleCount = buffer.Length / 2;
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(buffer, i * 2);
                samples[i] = sample / 32768f;
            }

            int zeroCrossings = 0;
            for (int i = 1; i < samples.Length; i++)
            {
                if ((samples[i - 1] >= 0 && samples[i] < 0) || (samples[i - 1] < 0 && samples[i] >= 0))
                    zeroCrossings++;
            }

            float frequency = (zeroCrossings * waveIn.WaveFormat.SampleRate) / (2f * samples.Length);
            return frequency;
        }

        private string GetClosestNote(float freq)
        {
            string closest = null;
            float minDiff = float.MaxValue;

            foreach (var note in notes)
            {
                float diff = Math.Abs(note.Value - freq);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    closest = note.Key;
                }
            }

            return closest;
        }
    }
}


