using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AudioEngineCSharp
{

    public static class AudioEngine
    {
        public class Audio : IDisposable
        {
            public unsafe IntPtr _audio;

            public int DataSize { private set; get; }
            private ulong pos;

            public byte[] Data
            {
                get
                {
                    unsafe
                    {
                        byte[] output = new byte[DataSize];
                        IntPtr ptr = new IntPtr(RawData);
                        Marshal.Copy(ptr, output, 0, DataSize);
                        return output;
                    }
                }
            }

            public ulong Pos
            {
                set
                {
                    pos = value;
                    AudioEngineSetPosition(_audio, pos);
                }
                get
                {
                    pos = AudioEngineGetPosition(_audio);
                    return pos;
                }
            }

            public unsafe char * RawData
            {
                get
                {
                    return AudioEngineRetrieveData(_audio);
                }
            }

            public unsafe Audio(IntPtr audio)
            {
                DataSize = AudioEngineAudioDataSize(audio);
                _audio = audio;
            }


            public unsafe void Dispose()
            {
                Unload(_audio);
            }
        }

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineInitialize")]
        public static extern bool Initialize();

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineAudioSize")]
        private static extern int EngineAudioSize();

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineLoad", CallingConvention = CallingConvention.Cdecl)]
        private static extern void EngineLoad(string file, bool enableTransitions, bool loop, out IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEnginePlay")]
        private static extern unsafe void AudioEnginePlay(IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEnginePlayTime")]
        private static extern unsafe void AudioEnginePlay(IntPtr handle, ulong time);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineLoadWithBasicTransitions")]
        public static extern void EngineLoadPreset(string file, out IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEnginePlayPos")]
        private static extern unsafe void AudioEnginePlayPos(IntPtr handle, ulong position, ulong timeToStart);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineGetPosition")]
        private static extern unsafe ulong AudioEngineGetPosition(IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineSetPosition")]
        private static extern unsafe void AudioEngineSetPosition(IntPtr handle, ulong position);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineAddEffect")]
        private static extern unsafe void AudioEngineAddEffect(IntPtr handle, IntPtr effect);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineStop")]
        private static extern void EngineStop();

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineStopTime")]
        private static extern void EngineStopTime(ulong timeToStop);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineAudioDataSize")]
        private static extern int AudioEngineAudioDataSize(IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineRetrieveData")]
        private static extern unsafe char* AudioEngineRetrieveData(IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineUnload")]
        private static extern unsafe void EngineUnload(IntPtr handle);

        [DllImport("AudioEngine2", EntryPoint = "AudioEngineShutdown")]
        public static extern bool Shutdown();

        [DllImport("AudioEngine2", EntryPoint = "SecondsToBytes")]
        private static extern ulong EngineSecondsToBytes(IntPtr handle, float seconds);

        [DllImport("AudioEngine2", EntryPoint = "BytesToSeconds")]
        private static extern float EngineBytesToSeconds(IntPtr handle, ulong bytes);

        public static int AudioSize { get { return EngineAudioSize(); } }

        public static Audio Load(string file, bool enableTransitions = false, bool loop = false)
        {
            unsafe
            {
                IntPtr handle;
                // IntPtr handle;
                EngineLoad(file, enableTransitions, loop, out handle);
                return new Audio(handle);
            }
        }

        public static Audio LoadPreset(string file)
        {
            unsafe
            {
                IntPtr handle;
                // IntPtr handle;
                EngineLoadPreset(file, out handle);
                return new Audio(handle);
            }
        }

        public static void Play(Audio audio)
        {
            unsafe
            {
                AudioEnginePlay(audio._audio);
            }
        }

        public static void PlayPos(Audio audio, ulong position)
        {
            Play(audio, position, 0);
        }

        public static void Play(Audio audio, ulong position, ulong timeToStart)
        {
            unsafe
            {
                AudioEnginePlayPos(audio._audio, position, timeToStart);
            }
        }

        public static void Stop(Audio audio)
        {
            unsafe
            {
                
                EngineStop();
            }
        }

        public static void Stop(Audio audio, ulong time)
        {
            EngineStopTime(time);
        }

        public static void Stop(Audio audio, float seconds)
        {
            ulong time = SecondsToBytes(audio, seconds);
            Stop(audio, time);
        }

        public static unsafe void Unload(IntPtr audio)
        {
            if (audio != IntPtr.Zero)
                EngineUnload(audio);
            // Marshal.FreeHGlobal(audio);
        }

        public static float BytesToSeconds(Audio audio, ulong bytes)
        {
            return EngineBytesToSeconds(audio._audio, bytes);
        }

        public static ulong SecondsToBytes(Audio audio, float seconds)
        {
            return EngineSecondsToBytes(audio._audio, seconds);
        }
    }
}
