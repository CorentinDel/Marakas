using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marakas.Sounds
{
    public class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader _audioFileReader;
        private readonly Action onComplete;
        public WaveFormat WaveFormat => _audioFileReader.WaveFormat;

        public AutoDisposeFileReader(string filePath, Action onComplete) 
        {
            _audioFileReader = new AudioFileReader(filePath);
            this.onComplete = onComplete;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _audioFileReader.Read(buffer, offset, count);
            if (samplesRead < count)
            {
                _audioFileReader.Dispose();
                onComplete?.Invoke();
            }
            return samplesRead;
        }

        public void Dispose()
        {
            _audioFileReader.Dispose();
        }
    }
}
