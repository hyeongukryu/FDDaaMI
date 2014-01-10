using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace FDDaaMI
{
    public class Hardware : IDisposable
    {
        public SerialPort Port { get; private set; }

        public double Factor { get; set; }
        public bool Software { get; set; }
        
        public Hardware()
        {
            Factor = 1;

            Port = new SerialPort
            {
                PortName = "COM4",
                BaudRate = 38400,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            Port.Open();
            Port.ReadExisting();
            Port.Write(new[] { (byte)42 }, 0, 1);
        }

        public void SoundFrequency(int channel, double frequency)
        {
            if (frequency == 0)
            {
                SoundDelayMicrosecond(channel, 0);
            }
            else
            {
                frequency *= Factor;

                var delay = 1000 * 1000 / frequency;
                SoundDelayMicrosecond(channel, (int)delay);
            }
        }

        public void SoundFrequency(int channel, double frequency, int shift)
        {
            SoundFrequency(channel, frequency * Math.Pow(2, shift / 12.0));
        }

        public void SoundDelayMicrosecond(int channel, int delay)
        {
#if DEBUG
            Console.WriteLine(channel + " " + delay);
#endif
   
            if (Software)
            {
                return;
            }

            int high = (delay >> 8) & 0xFF;
            int low = delay & 0xFF;
            var data = new[] { (byte)channel, (byte)high, (byte)low };

            Port.Write(data, 0, 3);
            var check = Port.ReadByte();
            
            if (check != 42)
            {
                throw new Exception("42");
            }
        }

        public void Dispose()
        {
            Port.Dispose();
        }
    }
}
