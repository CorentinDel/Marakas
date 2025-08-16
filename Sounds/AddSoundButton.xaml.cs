using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using UserControl = System.Windows.Controls.UserControl;

namespace Marakas.Sounds
{
    /// <summary>
    /// Logique d'interaction pour AddSoundButton.xaml
    /// </summary>
    public partial class AddSoundButton : UserControl
    {
        public AddSoundButton()
        {
            InitializeComponent();
        }

        private void SoundBtn_AddSound(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Fichiers audio|*.mp3;*.wav",
                Title = "Sélectionner un fichier audio"
            };

            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFile = fileDialog.FileName;
                string destFile = Path.Combine(GlobalData.Instance.PathFolderSounds, Path.GetFileName(sourceFile));

                try
                {
                    File.Copy(sourceFile, destFile, overwrite: true);
                    GlobalData.Instance.AddSound(new SoundData(Path.GetFileName(sourceFile), Path.GetFileName(sourceFile), 0.2f));
                }catch(IOException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
