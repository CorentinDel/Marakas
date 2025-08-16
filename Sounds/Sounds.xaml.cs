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
using Marakas.Sounds;

namespace Marakas.Tabs
{
    /// <summary>
    /// Logique d'interaction pour Sounds.xaml
    /// </summary>
    public partial class Sounds : UserControl
    {
        public Sounds()
        {
            InitializeComponent();

            LoadSounds();
        }

        private void LoadSounds(){
            foreach (SoundData soundData in GlobalData.Instance.Sounds) {
                SoundsButton soundButton = new();
                soundButton.Init(soundData.fileName, soundData.name, soundData.volume);

                SoundsPanel.Children.Add(soundButton);
            }
        }

    }
}
