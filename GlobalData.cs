using Marakas.Sounds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marakas
{
    public class GlobalData
    {
        [JsonProperty]
        public string PathFolderSounds { get; private set; }
        [JsonProperty]
        public List<SoundData> Sounds { get; private set; }
        [JsonProperty]
        public int indexInput { get; private set; }
        [JsonProperty]
        public int indexVirtualOutput { get; private set; }
        [JsonProperty]
        public int indexOutput { get; private set; }

        public static GlobalData Instance { get; private set; }

        private static readonly string jsonPath = @$"{Directory.GetCurrentDirectory()}/marakas.json";

        public static void Load()
        {
            if (!File.Exists(jsonPath))
            {
                Instance = new()
                {
                    PathFolderSounds = Directory.GetCurrentDirectory(),
                    Sounds = []
                };
                return;
            }

            string jsonContent = File.ReadAllText(jsonPath);

            Instance = JsonConvert.DeserializeObject<GlobalData>(jsonContent);
        }

        public static void Save()
        {
            string jsonContent = JsonConvert.SerializeObject(Instance, Formatting.Indented);
            File.WriteAllText(jsonPath, jsonContent);
        }

        public void AddSound(SoundData sound)
        {
            Sounds.Add(sound);
            Save();
        }

        public void SetSoundsFolderPath(string folderPath)
        {
            PathFolderSounds = folderPath;
            Save();
        }

        public void SetSoundData(string filename,  SoundData data)
        {
            SoundData? listItem = Sounds.Find((sound) => sound.fileName == filename);

            if(listItem == null)
            {
                Debug.WriteLine("No sound found");
                return;
            }

            listItem.name = data.name;
            listItem.volumeVC = data.volumeVC;
            listItem.volumeHP = data.volumeHP;
            Save();
        }

        public void SetEntryIndex(AudioEntry entry, int index)
        {
            switch (entry)
            {
                case AudioEntry.Input:
                    indexInput = index; break;
                case AudioEntry.VirtualOutput:
                    indexVirtualOutput = index; break;
                case AudioEntry.Output:
                    indexOutput = index; break;
            }
            Save();
        }
    }

    public class EntryIndexEventData(int _selectedIndex, AudioEntry _entry)
    {
        public int selectedIndex = _selectedIndex;
        public AudioEntry entry = _entry;
    }

    public class SoundData(string _fileName, string _name, float _volumeVC, float _volumeHP)
    {
        public string fileName = _fileName;
        public float volumeVC = _volumeVC;
        public float volumeHP = _volumeHP;
        public string name = _name;
    }

    public enum AudioEntry
    {
        Input,
        VirtualOutput,
        Output,
    }
}
