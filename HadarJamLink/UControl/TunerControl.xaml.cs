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
using System.Windows.Threading;

namespace JamLinkComputers.UControl
{
    /// <summary>
    /// Interaction logic for TunerControl.xaml
    /// </summary>
    /// 

    public partial class TunerControl : UserControl
    {
        private WaveInEvent waveIn;
        private DispatcherTimer greenLockTimer;
        private bool isLockedOnGreen = false;

        private Dictionary<string, float> currentNotes;

        // הגדרות התדרים מהטבלה שלך
        private readonly Dictionary<string, float> guitar = new Dictionary<string, float> {
        {"E2", 82f}, {"A2", 110f}, {"D3", 146f}, {"G3", 196f}, {"B3", 246f}, {"E4", 329f} };
        private readonly Dictionary<string, float> bass = new Dictionary<string, float> {
        {"E1", 41f}, {"A1", 55f}, {"D2", 73f}, {"G2", 98f} };
        private readonly Dictionary<string, float> uke = new Dictionary<string, float> {
        {"G4", 392f}, {"C4", 261f}, {"E4", 329f}, {"A4", 440f}};

        private void InstrumentSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InstrumentSelector == null) return;

            var selected = (ComboBoxItem)InstrumentSelector.SelectedItem;
            switch (selected.Content.ToString())
            {
                case "Guitar": currentNotes = guitar; break;
                case "Bass": currentNotes = bass; break;
                case "Ukelele": currentNotes = uke; break;
            }
        }


        public TunerControl()
        {
            InitializeComponent();
            StartMicrophone();
            // הגדרת הטיימר למשך של שנייה אחת
            greenLockTimer = new DispatcherTimer();
            greenLockTimer.Interval = TimeSpan.FromSeconds(1);
            greenLockTimer.Tick += (s, e) => {
                isLockedOnGreen = false; // שחרור הנעילה
                greenLockTimer.Stop(); };
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
                // הגנה למקרה שעדיין לא נבחר כלי בתיבת הבחירה
                if (currentNotes == null) return;
                try
                {
                    // 1. חישוב התדר הנוכחי מהמיקרופון
                    float freq = GetFrequency(e.Buffer);
                    // סינון רעשים קיצוניים (מתחת ל-30Hz בדרך כלל אינו רלוונטי לכלים הללו)
                    if (freq < 30) return;
                    // 2. מציאת התו הקרוב ביותר מתוך ה-Dictionary של הכלי שנבחר
                    string closestNote = GetClosestNote(freq);
                    float idealFreq = currentNotes[closestNote];
                    // 3. עדכון הממשק (UI) ב-Thread הראשי
                    Dispatcher.Invoke(() =>
                    {
                        // הצגת שם התו (למשל "E2")
                        NoteText.Text = closestNote;
                        // חישוב הסטייה היחסית
                        float delta = (freq - idealFreq) / idealFreq;
                        // בדיקה אם המיתר מכוון (סטייה קטנה מ-1%)
                        if (Math.Abs(delta) < 0.01f)
                        {
                            // הפעלת מצב "נעילה" על ירוק
                            isLockedOnGreen = true;

                            greenLockTimer.Stop(); // איפוס הטיימר אם כבר רץ
                            greenLockTimer.Start(); // התחלת הספירה לאחור (שנייה אחת)
                            TuningArrow.Fill = Brushes.Green;
                            TuningArrow.RenderTransform = new RotateTransform(0); // יישור החץ למרכז
                        }
                        // אם אנחנו לא בתוך זמן ה"נעילה" הירוק, נעדכן את החץ לאדום לפי הסטייה
                        else if (!isLockedOnGreen)
                        {
                            TuningArrow.Fill = Brushes.Red;
                            // חישוב זווית הסיבוב (delta * 500 נותן תנועה מורגשת)
                            // הגבלנו את הזווית לטווח של 60 מעלות לכל צד
                            float angle = Math.Max(-60, Math.Min(60, delta * 500));
                            TuningArrow.RenderTransform = new RotateTransform(angle);
                        }
                    });
                }
                catch (Exception ex)
                {
                    // רישום שגיאה במידה וקרסה הלוגיקה (למשל בעיית הרשאות מיקרופון)
                    System.Diagnostics.Debug.WriteLine("Error in Tuning: " + ex.Message);
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
            string closest = "??";
            float minDiff = float.MaxValue;

            foreach (var note in currentNotes)
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


