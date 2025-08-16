using Marakas.Options;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Marakas.Tabs
{
    /// <summary>
    /// Logique d'interaction pour Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        private readonly AudioInput _audioInput;
        private readonly AudioOutput _audioVirtualOutput;
        private readonly AudioOutput _audioOutput;

        public Options()
        {
            InitializeComponent();

            // Entries definitions
            _audioInput = new AudioInput(comboInputDevices);
            _audioInput.InitializeComboBox();
            _audioVirtualOutput = new AudioOutput(comboOutputDevices, true);
            _audioVirtualOutput.InitializeComboBox();
            _audioOutput = new AudioOutput(comboOutputDevicesPersonal, false);
            _audioOutput.InitializeComboBox();

            // StartRecording
            StartRecording();

            // EntriesChanged during recording
            AddEntriesListener();

            //Populate Folder path if it exists
            PathSoundsTextBox.Text = GlobalData.Instance?.PathFolderSounds;
        }

        private void AddEntriesListener()
        {
            _audioInput.OnInputChanged += AudioEntries_OnInputChanged;
            _audioVirtualOutput.OnOutputChanged += AudioEntries_OnInputChanged;
            _audioOutput.OnOutputChanged += AudioEntries_OnInputChanged;
        }

        private void StartRecording()
        {
            _audioInput.Start(earFeedback.IsChecked == true);
            _audioVirtualOutput.Start(_audioInput.MixingVirtualProvider);
            _audioOutput.Start(_audioInput.MixingProvider);
        }

        private void AudioEntries_OnInputChanged(object? sender, EntryIndexEventData data)
        {
            GlobalData.Instance.SetEntryIndex(data.entry, data.selectedIndex);

            // Clear all recording states
            _audioInput.Dispose();
            _audioOutput.Dispose();
            _audioVirtualOutput.Dispose();
            StartRecording();
        }

        private void EaringFeedback_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _audioInput.StopEarFeedback();
            _audioOutput.Dispose();
            _audioInput.StartEarFeedback(earFeedback.IsChecked == true ? 1.0f : 0.0f);
            _audioOutput.Start(_audioInput.MixingProvider);
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float volume = (float)e.NewValue / 100.0f;
            _audioInput?.UpdateVolume(volume);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PathSoundsButton_OpenDialog(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog()
            {
                Title = "Select Folder",
                InitialDirectory = Directory.GetCurrentDirectory(),
            };

            if(folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                PathSoundsTextBox.Text = folderName;
                //Save the path in a json file
                GlobalData.Instance.SetSoundsFolderPath(folderName);
            }
        }
    }
}
