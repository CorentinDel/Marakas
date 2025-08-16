using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Marakas.Options.utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Marakas.Options
{
    public class AudioOutput(ComboBox comboOutputDevices, bool isVirtual) : IDisposable
    {
        private readonly ComboBox _comboOutputDevices = comboOutputDevices;
        private WaveOutEvent? _waveOutEvt;

        public readonly bool IsVirtual = isVirtual;

        public bool Playing { get {  return _waveOutEvt != null; } }

        public event EventHandler<EntryIndexEventData>? OnOutputChanged;

        public void InitializeComboBox()
        {
            List<string> outputs = AudioUtils.GetOutputDevices();

            _comboOutputDevices.ItemsSource = outputs;

            _comboOutputDevices.SelectedIndex = IsVirtual ? GlobalData.Instance.indexVirtualOutput : GlobalData.Instance.indexOutput;

            _comboOutputDevices.SelectionChanged += ComboOutputDevices_SelectionChanged;
        }

        private void ComboOutputDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_waveOutEvt != null)
            {
                _waveOutEvt.DeviceNumber = _comboOutputDevices.SelectedIndex;
                OnOutputChanged?.Invoke(this, new EntryIndexEventData(_comboOutputDevices.SelectedIndex, IsVirtual ? AudioEntry.VirtualOutput : AudioEntry.Output));
            }
        }

        public void Start(MixingSampleProvider? provider)
        {
            if (provider == null)
                return;

            _waveOutEvt = new WaveOutEvent
            {
                DeviceNumber = _comboOutputDevices.SelectedIndex,
            };

            _waveOutEvt.Init(provider);
            _waveOutEvt.Play();
        }

        private void Stop()
        {
            if (_waveOutEvt != null)
            {
                _waveOutEvt.Stop();
                _waveOutEvt.Dispose();
                _waveOutEvt = null;
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
