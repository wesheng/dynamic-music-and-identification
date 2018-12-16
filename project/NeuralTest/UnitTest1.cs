using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ws.Neural;

namespace NeuralTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestRunnable()
        {
            int[] layers = new[]
            {
                9, 12, 4
            };
            ws.Neural.Network network = new Network(layers, 1.0);

            double[] inputs = new[]
            {
                1.0, 0.0, 1.0,
                1.0, 1.0, 1.0,
                1.0, 0.0, 1.0,
            };
            double[] output = network.Parse(inputs);
            Console.WriteLine(output);
        }

        [TestMethod]
        public void TestXorNoTrain()
        {
            int[] layers = new[]
            {
                2, 1
            };
            Network network = new Network(layers, 1.0);
            Random rand = new Random();
            // generate xors
            for (int i = 0; i < 10; i++)
            {
                double[] input = new[]
                {
                    (double) rand.Next(2),
                    (double) rand.Next(2)
                };
                double[] output = network.Parse(input);
                Console.WriteLine("{0} ^ {1} = {2}", (int) input[0], (int) input[1], output[0]);
            }
        }

        [TestMethod]
        public void TestLearnOneNeuron()
        {
            int[] layers = new[]
            {
                1
            };
            ws.Neural.Network network = new Network(layers, 1.0);

            double[][] trainingData = new[]
            {
                new []{ 1.0, 0.0 }
            };
            //network.Stochastic(trainingData, 50, 2);
            //// double[] output = network.Parse(inputs);
            //Console.WriteLine(output[0]);
        }
    }
}
