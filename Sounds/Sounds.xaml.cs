using Marakas.Sounds;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour Sounds.xaml
    /// </summary>
    public partial class Sounds : UserControl
    {
        private List<SoundsButton> soundsBtnList = [];
        private SoundsButton? selectedSoundsBtn;

        public Sounds()
        {
            InitializeComponent();

            LoadSounds();
        }

        private void LoadSounds(){
            foreach (SoundData soundData in GlobalData.Instance.Sounds) {
                SoundsButton soundButton = new();
                soundButton.Init(soundData.fileName, soundData.name, soundData.volumeVC, soundData.volumeHP);
                soundButton.OnSoundBtnSelected += SoundButton_OnSoundBtnSelected;

                SoundsPanel.Children.Add(soundButton);
                soundsBtnList.Add(soundButton);
            }
        }

        private void SoundButton_OnSoundBtnSelected(object? sender, SoundData data)
        {
            GridLength gridColumnWidth = new(1.0f, GridUnitType.Star);
            if (SoundOptPanel.Width != gridColumnWidth)
                SoundOptPanel.Width = gridColumnWidth;

            selectedSoundsBtn = sender as SoundsButton;

            SoundNameTextBox.Text = data.name;
            GainValueVC.Value = data.volumeVC * 100.0f;
            GainValueHP.Value = data.volumeHP * 100.0f;
        }

        private void SoundName_TextChanged(object sender, EventArgs e)
        {
            if(selectedSoundsBtn != null)
            {
                TextBox textBox = sender as TextBox;
                selectedSoundsBtn.SetName(textBox.Text);
            }
        }

        private void VolumeVCSlider_ValueChanged(object sender, EventArgs e)
        {
            if (selectedSoundsBtn != null)
            {
                Slider sliderVolumeVC = sender as Slider;
                selectedSoundsBtn.SetVolumeVC((float)sliderVolumeVC.Value / 100f);
            }
        }

        private void VolumeHPSlider_ValueChanged(Object sender, EventArgs e)
        {
            if (selectedSoundsBtn != null)
            {
                Slider sliderVolumeHP = sender as Slider;
                selectedSoundsBtn.SetVolumeHP((float)sliderVolumeHP.Value / 100f);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SoundOption_CloseBtn(object sender, EventArgs e)
        {
            GridLength gridColumnWidth = new(0.0f);
            SoundOptPanel.Width = gridColumnWidth;

            selectedSoundsBtn = null;
        }

        private void AddButton_SoundAdded(object sender, RoutedEventArgs e)
        {
            ClearUISounds();
            LoadSounds();
        }

        private void ClearUISounds()
        {
            foreach(SoundsButton soundBtn in soundsBtnList)
            {
                SoundsPanel.Children.Remove(soundBtn);
                soundBtn.Dispose();
            }
            soundsBtnList.Clear();
        }

        private void SoundOption_StopSound(object sender, RoutedEventArgs e)
        {
            selectedSoundsBtn?.Stop();
        }

    }
}
