using CannedBytes.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDDaaMI
{
    class Processor
    {
        public MidiInPort InPort { get; set; }

        public void Start()
        {
            InPort = new MidiInPort();
            InPort.Successor = new FloppyReceiver();
            InPort.Open(0);
            InPort.Start();
        }

        public void Stop()
        {
            InPort.Stop();
            InPort.Close();
        }
    }
}
