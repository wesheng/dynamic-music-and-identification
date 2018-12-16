using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Classes
{
    class Song : IDisposable
    {
        private AudioEngineCSharp.AudioEngine.Audio _audio;
        public float Offset { get; private set; }
        public float BeatPerMinute { get; private set; }
        public float BeatPerSecond { get; private set; }
        public float SecondPerBeat { get; private set; }

        public float Position {
            get
            {
                return AudioEngineCSharp.AudioEngine.BytesToSeconds(_audio, _audio.Pos);
            }
        }

        private float _pos;

        public Song(string file, float offset, float bpm)
        {
            _audio = AudioEngineCSharp.AudioEngine.LoadPreset(file);
            Offset = offset;
            BeatPerMinute = bpm;
            BeatPerSecond = BeatPerMinute / 60.0f;
            SecondPerBeat = 1.0f / BeatPerSecond;
        }

        public void Play(float position)
        {
            AudioEngineCSharp.AudioEngine.PlayPos(_audio, AudioEngineCSharp.AudioEngine.SecondsToBytes(_audio, position));
        }

        public void Play(float position, float time)
        {
            AudioEngineCSharp.AudioEngine.Play(_audio, (ulong) (position * 1000), (ulong)(time * 1000));
        }

        public void Play(bool fromLastPos, float time)
        {
            if (fromLastPos)
            {
                Play(_pos, time);
            }
            else
            {
                AudioEngineCSharp.AudioEngine.Play(_audio, 0, (ulong) (time * 1000));
            }
        }

        public void Stop(float time)
        {
            ulong stopBytes = (ulong)(time * 1000);
            _pos = (_audio.Pos / 1000.0f) + time;
            AudioEngineCSharp.AudioEngine.Stop(_audio, stopBytes);
        }

        public void Dispose()
        {
            _audio.Dispose();
        }
    }
}
