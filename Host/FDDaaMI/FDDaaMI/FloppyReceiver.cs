using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CannedBytes.Midi;
using CannedBytes.Midi.Message;

namespace FDDaaMI
{
    class FloppyReceiver : IMidiDataReceiver
    {
        private MidiMessageFactory _factory = new MidiMessageFactory();
        private double[] _midiFrequency = null;
        private Hardware _hardware = new Hardware();
        private List<HashSet<int>> _status = new List<HashSet<int>>();
        private List<double> _lastStatus = new List<double>();
        public bool Polyphony { get; set; }

        public FloppyReceiver()
        {
            _hardware.Software = false;
            _hardware.Factor = 0.5;
            Polyphony = true;

            _midiFrequency = new double[128];
            int a = 440;
            for (int x = 0; x < 128; ++x)
            {
                _midiFrequency[x] = (a / 32.0) * Math.Pow(2, ((x - 9) / 12.0));
            }

            for (int i = 0; i < 4; i++)
            {
                _status.Add(new HashSet<int>());
                _lastStatus.Add(0);
            }
        }

        private void Status(int chan, double frequency)
        {
            if (_lastStatus[chan] != frequency)
            {
                _lastStatus[chan] = frequency;
                _hardware.SoundFrequency(chan, frequency);
            }
        }

        private void Update(int chan)
        {
            var q = (from n in _status[chan]
                     orderby n descending
                     select n).ToArray();

            if (q.Length == 0)
            {
                Status(chan, 0);
            }
            else
            {
                Status(chan, _midiFrequency[q.First()]);
            }
        }


        private void NoteOn(int chan, int note)
        {
            _status[chan].Add(note);
            Update(chan);
        }

        private void NoteOff(int chan, int note)
        {
            _status[chan].Remove(note);
            Update(chan);
        }

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
        }

        public void ShortData(int data, long timestamp)
        {
            MidiShortMessage message = _factory.CreateShortMessage(data);
            if (128 <= message.Status && message.Status <= 159)
            {
                bool on = message.Status >= 144;
                int fddChan = ((message.Status >= 144) ? (message.Status - 144) : (message.Status - 128)) % 4;

                int noteNumber = message.Parameter1;
                int velocity = message.Parameter2;

                if (on && velocity > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        NoteOn((fddChan + i) % 4, noteNumber);
                        if (Polyphony)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        NoteOff((fddChan + i) % 4, noteNumber);
                        if (Polyphony)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
