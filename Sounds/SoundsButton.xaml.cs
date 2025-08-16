using Marakas.Options;
using Marakas.Tabs;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;

namespace Marakas.Sounds
{
    /// <summary>
    /// Logique d'interaction pour SoundsButton.xaml
    /// </summary>
    public partial class SoundsButton : UserControl, IDisposable
    {
        private string _soundFileLocation = "";
        private string _name = "";
        private float _soundVolumeVC = 0.2f;
        private float _soundVolumeHP = 0.2f;

        private AudioFileReader? soundReader;
        private AudioFileReader? soundVirtualReader;
        private VolumeSampleProvider? soundVolume;
        private VolumeSampleProvider? soundVirtualVolume;
        private ISampleProvider? soundProvider;
        private ISampleProvider? soundVirtualProvider;

        public event EventHandler<SoundData>? OnSoundBtnSelected;

        public SoundsButton()
        {
            InitializeComponent();
        }

        public void Init(string soundFileLocation, string name,  float volumeVC, float volumeHP)
        {
            _soundFileLocation = soundFileLocation;
            SetData(name, volumeVC, volumeHP);
        }

        public void SetName(string name)
        {
            _name = name;
            GlobalData.Instance.SetSoundData(_soundFileLocation, new SoundData(_soundFileLocation, name, _soundVolumeVC, _soundVolumeHP));
        }

        public void SetVolumeVC(float volumeVC)
        {
            _soundVolumeVC = volumeVC;
            GlobalData.Instance.SetSoundData(_soundFileLocation, new SoundData(_soundFileLocation, _name, _soundVolumeVC, _soundVolumeHP));
        }

        public void SetVolumeHP(float volumeHP)
        {
            _soundVolumeHP = volumeHP;
            GlobalData.Instance.SetSoundData(_soundFileLocation, new SoundData(_soundFileLocation, _name, _soundVolumeVC, _soundVolumeHP));
        }

        private void SetData(string name, float volumeVC, float volumeHP)
        {
            _name = name;
            _soundVolumeVC = volumeVC;
            _soundVolumeHP = volumeHP;
            SoundName.Text = name;
        }

        public void Update(string name, float volumeVC, float volumeHP)
        {
            SetData(name, volumeVC, volumeHP);

            if (soundVirtualVolume != null)
                soundVirtualVolume.Volume = _soundVolumeVC;

            if (soundVolume != null)
                soundVolume.Volume = _soundVolumeHP;

            GlobalData.Instance.SetSoundData(_soundFileLocation, new SoundData(_soundFileLocation, name, volumeVC, volumeHP));
        }

        private void SoundBtn_PlaySound(object sender, RoutedEventArgs e)
        {
            Audio audioInstance = Audio.GetInstance();

            audioInstance.currentSounds?.Dispose();

            // Casque 100%
            soundReader = new AudioFileReader($@"{GlobalData.Instance.PathFolderSounds}/{_soundFileLocation}");
            soundVolume = new VolumeSampleProvider(soundReader.ToSampleProvider()) { Volume = _soundVolumeHP };
            soundProvider = new WdlResamplingSampleProvider(soundVolume.ToMono(), audioInstance.MixingProvider != null ? audioInstance.MixingProvider.WaveFormat.SampleRate : 44100);
            audioInstance.MixingProvider?.AddMixerInput(soundProvider);

            // VB Cable 0%
            soundVirtualReader = new AudioFileReader($@"{GlobalData.Instance.PathFolderSounds}/{_soundFileLocation}");
            soundVirtualVolume = new VolumeSampleProvider(soundVirtualReader.ToSampleProvider()) { Volume = _soundVolumeVC };
            soundVirtualProvider = new WdlResamplingSampleProvider(soundVirtualVolume.ToMono(), audioInstance.MixingVirtualProvider != null ? audioInstance.MixingVirtualProvider.WaveFormat.SampleRate : 44100);
            audioInstance.MixingVirtualProvider?.AddMixerInput(soundVirtualProvider);

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

            if(soundVirtualReader != null)
            {
                soundVirtualReader.Dispose();
                soundVirtualReader = null;
            }

            if(soundProvider != null)
            {
                audioInstance.MixingVirtualProvider?.RemoveMixerInput(soundVirtualProvider);
                audioInstance.MixingProvider?.RemoveMixerInput(soundProvider);
                soundProvider = null;
            }

            // Update UI
            // Change Stop image to Play
        }

        public void Dispose()
        {
            Stop();
        }

        private void SoundBtn_Expand(object sender, RoutedEventArgs e)
        {
            // Update UI
            SoundData soundData = new(_soundFileLocation, _name, _soundVolumeVC, _soundVolumeHP);
            OnSoundBtnSelected?.Invoke(this, soundData);
            return;
        }
    }
}
