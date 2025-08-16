using Marakas.Options;
using Marakas.Options.utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Marakas
{
    public class AudioInput(ComboBox comboInputDevices) : IDisposable
    {
        private readonly ComboBox _comboInputDevices = comboInputDevices;

        private WaveInEvent? _waveInEvt;
        private BufferedWaveProvider? _bufferWP;
        private BufferedWaveProvider? _bufferWPVirtual;
        private VolumeSampleProvider? _volumeVirtualProvider;
        private VolumeSampleProvider? _volumeProvider;
        private MixingSampleProvider? _mixingVirtualProvider;
        private MixingSampleProvider? _mixingProvider;

        public event EventHandler<EntryIndexEventData>? OnInputChanged;

        public MixingSampleProvider? MixingVirtualProvider { get { return _mixingVirtualProvider; } }
        public MixingSampleProvider? MixingProvider { get { return _mixingProvider; } }

        public void InitializeComboBox()
        {
            List<string> inputs = AudioUtils.GetInputDevices();

            _comboInputDevices.ItemsSource = inputs;

            _comboInputDevices.SelectedIndex = GlobalData.Instance.indexInput;

            _comboInputDevices.SelectionChanged += ComboInputDevices_SelectionChanged;
        }

        public void UpdateVolume(float volume)
        {
            if (_volumeVirtualProvider != null)
                _volumeVirtualProvider.Volume = volume;
            if(_volumeProvider != null)
                _volumeProvider.Volume = volume;
        }

        private void ComboInputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_waveInEvt != null)
            {
                _waveInEvt.DeviceNumber = _comboInputDevices.SelectedIndex;
                OnInputChanged?.Invoke(this, new EntryIndexEventData(_comboInputDevices.SelectedIndex, AudioEntry.Input));
            }
        }

        public void Start(bool earFeedback, int sampleRate = 44100, int channels = 1)
        {
            _waveInEvt = new WaveInEvent
            {
                DeviceNumber = _comboInputDevices.SelectedIndex,
                WaveFormat = new WaveFormat(sampleRate, channels),
            };

            StartVRCableOut(sampleRate, channels);

            // Double buffer for each output
            StartEarFeedback(earFeedback ? 1.0f : 0.0f);

            _waveInEvt.DataAvailable += (s, e) =>
            {
                _bufferWPVirtual?.AddSamples(e.Buffer, 0, e.BytesRecorded);
                _bufferWP?.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };

            _waveInEvt.StartRecording();
        }

        private void StartVRCableOut(int sampleRate = 44100, int channels = 1)
        {
            _bufferWPVirtual = new BufferedWaveProvider(_waveInEvt?.WaveFormat);
            ISampleProvider virtualVolSampleProvider = _bufferWPVirtual.ToSampleProvider();
            _volumeVirtualProvider = new(virtualVolSampleProvider) { Volume = 1.0f };
            _mixingVirtualProvider = new(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels))
            {
                ReadFully = true
            };
            _mixingVirtualProvider.AddMixerInput(_volumeVirtualProvider);
            Audio.GetInstance().MixingVirtualProvider = _mixingVirtualProvider;
        }

        public void StartEarFeedback(float feedbackVolume, int sampleRate = 44100, int channels = 1)
        {
            if (_waveInEvt != null)
            {
                _bufferWP = new BufferedWaveProvider(_waveInEvt.WaveFormat);
                ISampleProvider volSampleProvider = _bufferWP.ToSampleProvider();
                _volumeProvider = new(volSampleProvider) { Volume = feedbackVolume};
                _mixingProvider = new(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels)) { ReadFully = true };
                _mixingProvider.AddMixerInput(_volumeProvider);
                Audio.GetInstance().MixingVirtualProvider = _mixingProvider;
            }
        }

        public void StopEarFeedback()
        {
            _bufferWP?.ClearBuffer();
            _bufferWP = null;
        }

        private void Stop()
        {
            if(_waveInEvt != null)
            {
                _waveInEvt.StopRecording();
                _waveInEvt.Dispose();
                _waveInEvt = null;
            }
        }
        public void Dispose()
        {
            Stop();
        }
    }
}
