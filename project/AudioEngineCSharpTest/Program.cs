using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioEngineCSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"C:\Users\wsheng\Documents\Neumont\Q9\capstone\repo\unity\project\Assets\Audio\VN1.7.wav";

            AudioEngineCSharp.AudioEngine.Initialize();

            AudioEngineCSharp.AudioEngine.Audio audio = AudioEngineCSharp.AudioEngine.LoadPreset(file);

            // ulong time = AudioEngineCSharp.AudioEngine.SecondsToBytes(audio, 1);
            AudioEngineCSharp.AudioEngine.Play(audio, 0, 1000);

            string command;

            ulong lastPos = 0;
            do
            {
                command = Console.ReadLine();
                command = command.ToLower();

                switch (command)
                {
                    case "stop":
                        lastPos = audio.Pos + 1000;
                        AudioEngineCSharp.AudioEngine.Stop(audio, 1000);
                        break;
                    case "play":
                        
                        AudioEngineCSharp.AudioEngine.Play(audio, lastPos, 1000);
                        break;
                }

            } while (!command.Equals("exit"));

            AudioEngineCSharp.AudioEngine.Stop(audio, 0);

            Console.ReadKey();


            audio.Dispose();
            // AudioEngineCSharp.AudioEngine.Unload(audio._audio);
            // AudioEngineCSharp.AudioEngine.Unload(audio);

            AudioEngineCSharp.AudioEngine.Shutdown();
        }
    }
}
