using Marakas.Sounds;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marakas.Options
{
    public class Audio
    {
        private static Audio? _instance;

        public MixingSampleProvider? MixingVirtualProvider { get; set; }
        public MixingSampleProvider? MixingProvider { get; set; }

        public SoundsButton currentSounds { get; set; }

        private Audio() { }

        public static Audio GetInstance()
        {
            _instance ??= new Audio();

            return _instance;
        }
    }
}
