using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis;
using AudioEngineWrapper;
// using MusicAnalysis;

namespace AnalyzerConsole
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            Analysis.AnalysisHelper h = new AnalysisHelper(@"F:\Documents\Q9\Capstone\Data\", "TrainingData.xlsx", "Music");

            AudioEngine.Initialize();

            h.PassFiles();

            var file = h.TestFile(@"F:\Documents\Q9\Capstone\Data\Metroid Prime\", "Magmoor Caverns");

            Console.WriteLine("Context");
            foreach (var p in file[0])
            {
                Console.WriteLine("{0}: {1}", p.Key, p.Value);
            }

            ////Console.WriteLine("Theme");
            ////foreach (var p in file[1])
            ////{
            ////    Console.WriteLine("{0}: {1}", p.Key, p.Value);
            ////}


            AudioEngine.Shutdown();

            // AudioEngine.Initialize();
            // AudioEngine.Shutdown();

            // MusicAnalysis.MusicAnalyzer t = new MusicAnalyzer();
            // Console.WriteLine(Directory.GetCurrentDirectory());


            //AudioEngine.Initialize();
            //Audio a = AudioEngine.Load("Data\\battle.wav", true);
            //Audio a2 = AudioEngine.Load("Data\\song.wav", true);
            //a.AddEffect(new FadeInEffect());
            //a.AddEffect(new FadeOutEffect());

            //a.Play(3.0f);
            //a2.AddEffect(new FadeInEffect());
            //a2.AddEffect(new FadeOutEffect());


            //Console.ReadKey();

            //a.Stop(1.0f);

            //Console.ReadKey();

            //a2.Play(3.0f);

            //Console.ReadKey();

            //a2.Stop(1.0f);

            Console.ReadKey();

            //a.Dispose();
            //a2.Dispose();
            //AudioEngine.Shutdown();

        }
    }
}
