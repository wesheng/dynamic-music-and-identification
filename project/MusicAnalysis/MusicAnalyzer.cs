using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;
using Accord.Statistics.Kernels;
using System.IO.Compression;
using System.Diagnostics;

using Exocortex.DSP;

namespace MusicAnalysis
{
    [Serializable]
    public class MusicAnalyzer
    {
        //private const int CLEARLY_OPTIMIZATION = 4;
        //private const int InputsSize = 8750000;
        private static Version Ver = new Version()
        {
            Major = 1,
            Minor = 0,
            SubMinor = 0
        };

        private const int CLEARLY_OPTIMIZATION = 8;
        private const int InputsSize = 10000;
        // private const int CutoffPoint = 
        // private const int InputsSize = 35000000;
        // private const int InputsSize = 17500000;

        private const double Modifier = 1;
        // private const int InputsSize = 2 << 23;
        // private const int MaxSongsAtOnce = 6;
        // 
        // private double[][] _storedSongs;
        // private int[][] _storedTypes;

        private List<string> _allContexts;

        // private int _currentIndex;

        // private MultilabelSupportVectorLearning<Linear> _teacher;
        
        private ActivationNetwork _network;
        private ISupervisedLearning _learning;
        private Standardizer _standard;

        public List<string> SupportedContexts => _allContexts.Select(i => (string) i.Clone()).ToList();

        public MusicAnalyzer(string loadLocation)
        {
            using (Stream loader = new FileStream(loadLocation, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                // todo deseserialize standarizer
                Version v = (Version)binaryFormatter.Deserialize(loader);
                if (v.Major == Ver.Major && v.Minor == Ver.Minor)
                {
                    _allContexts = (List<string>)binaryFormatter.Deserialize(loader);
                    _standard = (Standardizer)binaryFormatter.Deserialize(loader);
                    _network = (ActivationNetwork)Network.Load(loader);
                }
                else
                {
                    throw new Exception(string.Format("{0}.{1}.{2} version does not match current {3}.{4}.{5}", v.Major, v.Minor, v.SubMinor, Ver.Major, Ver.Minor, Ver.SubMinor));
                }


            }
        }

        public MusicAnalyzer(List<string> contexts)
        {
            // _storedSongs = new double[MaxSongsAtOnce][];
            // _storedTypes = new int[MaxSongsAtOnce][];
            _allContexts = contexts;
            // _currentIndex = 0;

            // _network = new ActivationNetwork(new SigmoidFunction(), InputsSize, (int) ((InputsSize * (2.0/3.0)) + contexts.Count), (int)((InputsSize * (1.0 / 3.0)) + contexts.Count), contexts.Count);
            int sum = (InputsSize + contexts.Count);
            _network = new ActivationNetwork(new SigmoidFunction(), InputsSize, contexts.Count, contexts.Count);
            // _network = new ActivationNetwork(new SigmoidFunction(), InputsSize, contexts.Count * 3, contexts.Count, contexts.Count);
            _network.Randomize();

            _learning = new BackPropagationLearning(_network)
            {
                LearningRate = 0.2,
                Momentum = 0.5
            };


            _standard = new Standardizer();
            
            // _learning = new ParallelResilientBackpropagationLearning(_network);

            // learning.RunEpoch()

            //_teacher = new MultilabelSupportVectorLearning<Linear>()
            //{
            //    Learner = (p) => new SequentialMinimalOptimization<Linear>()
            //    {
            //        Complexity = 100.0
            //    }
            //};
        }

        //public double AddData(byte[] data, string context)
        //{
        //    if (!_allContexts.Contains(context))
        //        throw new Exception("Context does not exist");

        //    var arr = ByteArrToDoubleArr(data);

        //    double[] types = TypesValues(context);
        //    return _learning.Run(arr, types);
        //}

        

        public unsafe void PreprocessAddData(char * data, int size)
        {

            var arr = ByteArrToDoubleArr(data, size);

            _standard.PreprocessAdd(arr);

        }

        public void PreDeviation()
        {
            _standard.PreprocessFinishAdd();
        }

        public unsafe void ProcessDeviation(char * data, int size)
        {
            var arr = ByteArrToDoubleArr(data, size);

            _standard.PreprocessCalculateDeviation(arr);
        }

        public void PreProcessData()
        {
            _standard.PreprocessFinishDeviation();
        }

        public unsafe double ProcessData(char* data, int size, string context)
        {
            if (!_allContexts.Contains(context))
                throw new Exception(context + " context does not exist");

            var arr = ByteArrToDoubleArr(data, size);
            Standardizer.ProcessCalculate(arr);

            double avg = 0;
            for (int i = 0; i < arr.Length; i++)
                avg += arr[i];
            avg /= arr.Length;
            Debug.WriteLine("Average: " + avg);

            double[] types = TypesValues(context);


            // Fourier.FFT(arr, FourierDirection.Forward);

            //double[] input = new double[arr.Length];
            //for(int i = 0; i < arr.Length; i++)
            //{
            //    input[i] = (double) arr[i].Re;
            //}

            double error;
            // int t;
            // do
            // {
            error = _learning.Run(arr, types);

            // t = TestData(data, size).Select(a => a).Where(a => a.Value >= 1 && a.Key.Equals(context)).Count();
            // } while (t < 1 && confirmGood);

            Debug.WriteLine("Data Error: " + error);
            return error;

        }

        private double[] TypesValues(string context)
        {
            double[] types = new double[_allContexts.Count];
            for (int i = 0; i < types.Length; i++)
            {
                if (_allContexts[i] == context)
                {
                    types[i] = 1.0;
                }
                else
                {
                    types[i] = -1.0;
                }
            }

            Standardizer.ProcessCalculate(types);

            // types = NormalizeDouble(types);

            return types;
        }

        private static double[] ByteArrToDoubleArr(byte[] data)
        {
            const int MAX_SIZE = InputsSize;

            MemoryStream stream = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(stream, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }

            byte[] compressedData = stream.ToArray();

            double[] arr = new double[MAX_SIZE];

            for (int i = 0; i < compressedData.Length / CLEARLY_OPTIMIZATION; i++)
            {
                if (i % CLEARLY_OPTIMIZATION != 0) continue;
                short s = BitConverter.ToInt16(compressedData, i * sizeof(short));
                arr[i / CLEARLY_OPTIMIZATION] = s / 32768.0;
                arr[i / CLEARLY_OPTIMIZATION] = Math.Min(arr[i / CLEARLY_OPTIMIZATION], 1);
                arr[i / CLEARLY_OPTIMIZATION] = Math.Max(arr[i / CLEARLY_OPTIMIZATION], -1);
            }
            
            return arr;
        }

        private static unsafe double[] ByteArrToDoubleArr(char * data, int size)
        {
            const int MAX_SIZE = InputsSize;
            // const int MAX_SIZE = 24000000;

            // SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            //MemoryStream stream = new MemoryStream();
            //using (DeflateStream dstream = new DeflateStream(stream, CompressionLevel.Optimal))
            //{
            //    dstream.Write(data, 0, size);
            //}

            // byte[] compressedData = stream.ToArray();

            short* shortData = (short*)data;


            //double[] arr = new double[MAX_SIZE];

            //for (int i = 0; i < size / CLEARLY_OPTIMIZATION && i / CLEARLY_OPTIMIZATION < MAX_SIZE; i++)
            //{
            //    if (i % CLEARLY_OPTIMIZATION != 0) continue;
            //    arr[i / CLEARLY_OPTIMIZATION] = shortData[i] / 32768.0;
            //    arr[i / CLEARLY_OPTIMIZATION] = Math.Min(arr[i / CLEARLY_OPTIMIZATION], 1);
            //    arr[i / CLEARLY_OPTIMIZATION] = Math.Max(arr[i / CLEARLY_OPTIMIZATION], -1);
            //}

            int shortSize = size / (sizeof(short) / sizeof(byte));
            double[] arr = new double[MAX_SIZE];

            int offset = 0;
            while (offset < shortSize && *(shortData + offset) == 0)
                ++offset;

            //Random rand = new Random();
            //int start = rand.Next(offset, (shortSize / CLEARLY_OPTIMIZATION) - MAX_SIZE);


            for (int i = 0; i < shortSize / CLEARLY_OPTIMIZATION && i / CLEARLY_OPTIMIZATION < MAX_SIZE; i += CLEARLY_OPTIMIZATION)
            {

                double sum = 0;
                for (int j = 0; j < CLEARLY_OPTIMIZATION; j++)
                {
                    short v = *(shortData + i + j + offset);

                    double pp = v / 32768.0;
                    pp = Math.Min(pp, 1.0);
                    pp = Math.Max(pp, -1.0);
                    //double pp = v;
                    sum += pp;
                }
                sum /= CLEARLY_OPTIMIZATION;
                arr[i / CLEARLY_OPTIMIZATION] = sum * Modifier;
            }

            return arr;
        }

        private static double[] NormalizeDouble(double[] input)
        {
            double avg = 0;
            for (int i = 0; i < input.Length; ++i)
            {
                avg += input[i];
            }
            avg /= input.Length;

            double deviation = 0;
            for (int i = 0; i < input.Length; ++i)
            {
                double x = input[i] - avg;
                x *= x;
                deviation += x;
            }

            deviation = Math.Sqrt(deviation);

            double newAvg = 0;
            for (int i = 0; i < input.Length; ++i)
            {
                input[i] = (input[i] - avg) / deviation;
                newAvg += input[i];
            }
            newAvg /= input.Length;
            Debug.WriteLine("New Average: " + newAvg);
            return input;
        }

        private static unsafe ComplexF[] ByteArrToComplexArr(char* data, int size)
        {
            const int MAX_SIZE = InputsSize;
            // const int MAX_SIZE = 24000000;

            // SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            //MemoryStream stream = new MemoryStream();
            //using (DeflateStream dstream = new DeflateStream(stream, CompressionLevel.Optimal))
            //{
            //    dstream.Write(data, 0, size);
            //}

            // byte[] compressedData = stream.ToArray();

            short* shortData = (short*)data;

            int shortSize = size / (sizeof(short) / sizeof(byte));
            ComplexF[] arr = new ComplexF[MAX_SIZE];
            for (int i = 0; i < shortSize / CLEARLY_OPTIMIZATION; i++)
            {
                //for (int i = 0; i < pcmLen; i++)
                //{
                //    float* p = out +i;
                //    *p = *(pcm + i) / 32768.0f;
                //    if (*p > 1) *p = 1;
                //    if (*p < -1) *p = -1;
                //}
                if (i % CLEARLY_OPTIMIZATION != 0) continue;
                // Debug.WriteLine(*(shortData + i));


                short val = *(shortData + i);
                arr[i / CLEARLY_OPTIMIZATION].Re = val / 32768.0f;
                arr[i / CLEARLY_OPTIMIZATION].Re = Math.Min(arr[i / CLEARLY_OPTIMIZATION].Re, 1.0f);
                arr[i / CLEARLY_OPTIMIZATION].Re = Math.Max(arr[i / CLEARLY_OPTIMIZATION].Re, -1.0f);
                // arr[i / CLEARLY_OPTIMIZATION] = p;
                // Debug.WriteLine(p);
            }

            return arr;
        }

        public void Save(string location)
        {
            using (FileStream stream = new FileStream(location, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Ver);
                formatter.Serialize(stream, _allContexts);
                formatter.Serialize(stream, _standard);
                _network.Save(stream);
                
            }
        }


        //public void TrainData()
        //{
        //    double[][] songsToTrain;
        //    int[][] types;
        //    if (_storedSongs[MaxSongsAtOnce - 1] == null)
        //    {
        //        songsToTrain = new double[_currentIndex][];
        //        types = new int[_currentIndex][];
        //        for (int i = 0; i < songsToTrain.Length; i++)
        //        {
        //            songsToTrain[i] = _storedSongs[i];
        //            types[i] = _storedTypes[i];
        //        }
        //    }
        //    else
        //    {
        //        songsToTrain = _storedSongs;
        //        types = _storedTypes;
        //    }
        //    _teacher.Learn(songsToTrain, types);

        //    _storedSongs = new double[MaxSongsAtOnce][];
        //    _currentIndex = 0;
        //}
        public Dictionary<string, double> TestData(byte[] data)
        {
            // var model = _teacher.Model;
            double[] arr = ByteArrToDoubleArr(data);
            // double[] probabilities = model.Probabilities(arr);
            double[] probabilities = _network.Compute(arr);
            double total = 0;
            foreach (double probability in probabilities)
            {
                total += probability;
            }
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] = probabilities[i] / total;
            }

            return _allContexts.Zip(probabilities, (k, v) => new {s = k, d = v})
                .OrderBy((k) => k.d).Reverse().ToDictionary(key => key.s, value => value.d);
        }

        public unsafe Dictionary<string, double> TestData(char * data, int size)
        {
            // var model = _teacher.Model;
            // var arr = ByteArrToComplexArr(data, size);
            // Fourier.FFT(arr, FourierDirection.Forward);

            //double[] input = new double[arr.Length];
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    input[i] = (double)arr[i].Re;
            //}

            var input = ByteArrToDoubleArr(data, size);
            Standardizer.ProcessCalculate(input);

            double[] probabilities = _network.Compute(input);
            

            double total = 0;
            foreach (double probability in probabilities)
            {
                total += probability;
            }

            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] = probabilities[i] / total;
            }



            return _allContexts.Zip(probabilities, (k, v) => new { s = k, d = v })
                .OrderBy((k) => k.d).Reverse().ToDictionary(key => key.s, value => value.d);
        }
    }
}
