using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FDDaaMI
{
    public class Music : IDisposable
    {
        public int Shift { get; set; }
        public Hardware Hardware { get; private set; }
        public double[] Status { get; set; }

        public Music()
        {
            Shift = 0;
            Status = new double[] { 0, 0, 0, 0 };
        }

        public void Start()
        {
            Hardware = new Hardware();

            for (int i = 0; i < 4; i++)
            {
                Hardware.SoundFrequency(i, 0);
            }
        }

        public void Dispose()
        {
            Hardware.Dispose();
        }

        void Process(double[] next)
        {
            for (int i = 0; i < Status.Length; i++)
            {
                if (Status[i] != next[i])
                {
                    Status[i] = next[i];

                    Hardware.SoundFrequency(i, Status[i], Shift);
                }
            }
        }

        int step = 0;

        public void SoundAll(List<Note> notes)
        {
            notes.Sort((a, b) => a.Time.CompareTo(b.Time));

            int time = 0;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach (var note in notes)
            {
                while (watch.Elapsed.TotalMilliseconds < note.Time)
                {
                    Console.WriteLine(watch.Elapsed.TotalMilliseconds);
                }

                if (note.Velocity != 0)
                {
                    Hardware.SoundFrequency(note.Channel, note.Frequency, Shift);
                }
                else
                {
                    Hardware.SoundFrequency(note.Channel, 0);
                }
            }
        }


        public void Sound(IEnumerable<Note> notes)
        {
            var nextStatus = Status.ToArray();
            var off = from note in notes
                      where note.Velocity == 0
                      select note;

            foreach (var note in off)
            {
                var q = from status in nextStatus.Select((freq, i) => new { Frequency = freq, Index = i })
                        where status.Frequency == note.Frequency
                        select status.Index;

                foreach (var n in q)
                {
                    nextStatus[n] = 0;
                }

                if (q.Count() > 0)
                {
                    var index = q.First();
                    nextStatus[index] = 0;
                }
            }


            var on = from note in notes
                     where note.Velocity > 0
                     orderby note.Frequency descending
                     select note;

            foreach (var note in on)
            {
                var q = from status in nextStatus.Select((freq, i) => new { Frequency = freq, Index = i })
                        where status.Frequency == 0
                        orderby status.Index ascending
                        select status.Index;

                if (q.Count() > 0)
                {
                    var index = q.First();
                    nextStatus[index] = note.Frequency;
                }
                else
                {
                    nextStatus[(step++) % 3] = note.Frequency;
                }
            }

            Process(nextStatus);
        }

        public void Stop()
        {
            var next = new double[] { 0, 0, 0, 0 };
            Process(next);
        }
    }
}
