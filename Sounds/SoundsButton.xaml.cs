using Marakas.Options;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Marakas.Sounds
{
    /// <summary>
    /// Logique d'interaction pour SoundsButton.xaml
    /// </summary>
    public partial class SoundsButton : UserControl, IDisposable
    {
        private string _soundFileLocation = "";
        private string _name = "";
        private float _soundVolume = 0.2f;
        private bool _soundPlaying = false;

        private AudioFileReader? soundReader;
        private ISampleProvider? soundProvider;
        //private ISampleProvider? soundVirtualProvider;

        public SoundsButton()
        {
            InitializeComponent();
        }

        public void Init(string soundFileLocation, string name,  float soundVolume)
        {
            _soundFileLocation = soundFileLocation;
            _name = name;
            _soundVolume = soundVolume;
            SoundName.Text = name;
        }

        private void SoundBtn_PlaySound(object sender, RoutedEventArgs e)
        {
            Audio audioInstance = Audio.GetInstance();

            audioInstance.currentSounds?.Dispose();

            soundReader = new AudioFileReader($@"{GlobalData.Instance.PathFolderSounds}/{_soundFileLocation}");
            soundProvider = new VolumeSampleProvider(soundReader.ToSampleProvider()) { Volume = _soundVolume }.ToMono();
            //soundVirtualProvider = new VolumeSampleProvider(soundReader.ToSampleProvider()) { Volume = _soundVolume }.ToMono();

            if (audioInstance.MixingVirtualProvider != null && soundProvider.WaveFormat.SampleRate != audioInstance?.MixingVirtualProvider?.WaveFormat.SampleRate)
            {
                soundProvider = new WdlResamplingSampleProvider(soundProvider, audioInstance.MixingVirtualProvider.WaveFormat.SampleRate);
                //soundVirtualProvider = new WdlResamplingSampleProvider(soundVirtualProvider, audioInstance.MixingVirtualProvider.WaveFormat.SampleRate);
            }

            //audioInstance.MixingVirtualProvider?.AddMixerInput(soundVirtualProvider);
            audioInstance.MixingProvider?.AddMixerInput(soundProvider);

            // Update UI
            // Change play image to stop image
            return;
        }

        public void Stop()
        {
            Audio audioInstance = Audio.GetInstance();

            if (soundReader != null)
            {
                soundReader.Dispose();
                soundReader = null;
            }

            if(soundProvider != null)
            {
                audioInstance.MixingVirtualProvider?.RemoveMixerInput(soundProvider);
                audioInstance.MixingProvider?.RemoveMixerInput(soundProvider);
                soundProvider = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private void SoundBtn_Expand(object sender, RoutedEventArgs e)
        {
            // Update UI
            return;
        }
    }
}
