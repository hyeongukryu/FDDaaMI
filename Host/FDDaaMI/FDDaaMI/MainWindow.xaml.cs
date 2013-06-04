using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Sanford.Multimedia.Midi;
using System.Threading;

namespace FDDaaMI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MidiFrequency = new double[127];
            int a = 440; // a is 440 hz...
            for (int x = 0; x < 127; ++x)
            {
                MidiFrequency[x] = (a / 32.0) * Math.Pow(2, ((x - 9) / 12.0));
            }


            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        const string FilePath = @"D:\Arduino\Turret Opera (Cara Mia) - SATB - 복사본.dat";
        //        Sequence _sequence = new Sequence(FilePath);

        public double[] MidiFrequency { get; set; }
        public Music Music { get; set; }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int shift = -12;
            double timeFactor = 3.03;
            bool noff = true;

            Music = new FDDaaMI.Music();
            Music.Start();
            Music.Shift = shift;
            Thread.Sleep(1000);

            List<Note> notes = new List<Note>();

            int prev = int.MinValue;

            var map = new Dictionary<int, List<int>>();

            for (int i = 0; i < 100; i++)
            {
                map[i] = null;
            }
            /*
            var k = new int[] { 3 };

            var rand = new Random();
            while (true)
            {
                var freq = rand.Next(40, 400);
                foreach(var i in k)
                {
                    Music.Hardware.SoundFrequency(i, freq);
                }
                Thread.Sleep(100);
            }
            */

            map[0] = new List<int>(new[] { 0 });
            map[1] = new List<int>(new[] { 1 });
            map[2] = new List<int>(new[] { 2 });
            map[3] = new List<int>(new[] { 3 });

            var lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                try
                {
                    var tokens = line.Split(new[] { ' ', '\t' });

                    var cumTime = int.Parse(tokens[0]);
                    var delTime = int.Parse(tokens[1]);
                    var @event = tokens[2];
                    var pitch = int.Parse(tokens[3]);
                    var vel = int.Parse(tokens[4]);
                    var chan = int.Parse(tokens[5]);
                    var value = tokens[6];

                    if (noff || (@event != "noff" && vel != 0))
                    {
                        if (@event == "noff")
                        {
                            vel = 0;
                        }

                        var channel = map[chan];

                        if (channel != null)
                        {
                            foreach (var c in channel)
                            {
                                notes.Add(new Note
                                {
                                    Time = (int)(cumTime * timeFactor),
                                    Frequency = MidiFrequency[pitch],
                                    Velocity = vel,
                                    Channel = c
                                });
                            }
                        }
                    }

                    prev = cumTime;
                }
                catch
                { }
            }

            Music.SoundAll(notes);

            Music.Stop();
        }
    }
}
