using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marakas.Options.utils
{
    internal class AudioUtils
    {
        public static List<string> GetInputDevices()
        {
            List<string> devices = [];
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities capabilities = WaveIn.GetCapabilities(i);
                devices.Add(capabilities.ProductName);
            }
            return devices;
        }

        public static List<string> GetOutputDevices()
        {
            List<string> devices = [];
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capabilities = WaveOut.GetCapabilities(i);
                devices.Add(capabilities.ProductName);
            }
            return devices;
        }
    }
}
