using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ws.Neural
{
    public class Network
    {
        private NeuronLayer[] _layers;
        private readonly Random _random;
        public double LearningRate { get; set; }

        public Network(int[] layers, double learningRate)
        {
            LearningRate = learningRate;
            _random = new Random();
            _layers = new NeuronLayer[layers.Length];
            _layers[0] = new NeuronLayer(layers[0], 1);
            for (var i = 1; i < _layers.Length; i++)
            {
                var numInputs = i <= 0 ? 0 : layers[i - 1];
                _layers[i] = new NeuronLayer(layers[i], numInputs);
            }
        }

        private void CalcCost()
        {
            // C(w,b) = (1/(2n)) summation->x ((y(x) - learningRate).length ^ 2) 
            // w = weight
            // b = bias
            // n = # of training inputs
            // learningRate = vec. of outputs when x inputted
            // summation = all training inputs
        }

        // Stochastic: Update for each result
        // Mini-Batch: Update for every b result
        // Batch: Update after all results done

        public void Stochastic(double[][] trainingData, double[][] results, int epochs, double learningRate)
        {
            // Gradient Descent
            // 

            // backpropagation algorithm
            // use to calculate the error

            // network output vs target
            // this comes from gradient descent
            // https://mattmazur.com/2015/03/17/a-step-by-step-backpropagation-example/

            for (int i = 0; i < epochs; i++)
            {
                // parse then 
                for (int bIndex = 0; bIndex < trainingData.Length; bIndex++)
                {
                    double[] output = Parse(trainingData[bIndex]);
                    double[] target = results[bIndex];

                    double totalError = 0;
                    for (int j = 0; j < output.Length; j++)
                    {
                        double error = (1.0 / 2.0) * Math.Pow(target[i] - output[i], 2.0);
                        totalError += error;
                    }
                    //double[] error = new double[output.Length];
                    //for (int j = 0; j < output.Length; j++)
                    //{
                    //    error[j] = (1.0 / 2.0) * Math.Pow(target[j] - output[j], 2);
                    //}
                    // find weights using backpropagation

                    /*
                     * neuron.error = sigmoidprime(neuron.output) * (result - output)
                     * neuron.adjust()
                     * 
                     * hidden1.error = sigmoidprime(hidden1.output) * neuron.error * neuron.weight[0]
                     * hidden2.error = sigmoidprime(hidden2.output) * neuron.error * neuron.weight[1]
                     * hidden1.adjust()
                     * hidden2.adjust()
                     */

                    //for (int k = 0; k < error.Length; k++)
                    //{
                    //    _layers[_layers.Length - 1].Neurons[k].Error = error[k];
                    //    _layers[_layers.Length - 1].Neurons[k].UpdateWeights(learningRate);

                    //    for (int l = _layers.Length - 2; l >= 0; l++)
                    //    {
                    //        for (int m = 0; m < _layers[l].Neurons.Length; m++)
                    //        {

                    //        }

                    //    }
                    //}

                }

            }

            // newWeight = weight - gradient
            // newBias = bias - 
        }
        
        public void Mini_Batch(double[][] trainingData, int epochs, int batchSizes, float eta)
        {
            // Mini-batch Gradient Descent is the most common method for deep learning
            // Stochastic Gradient Descent aka Online machine learning
            // used to update as new data becomes available
            for (int i = 0; i < epochs; i++)
            {
                Console.WriteLine("---------------");
                Console.WriteLine("Epoch {0}!", i);
                double[][] shuffledTraining = ShuffleData(trainingData);



            }
        }

        private double[][] ShuffleData(double[][] data)
        {
            // copy data
            double[][] output = new double[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] = new double[data[i].Length];
                for (int j = 0; j < data[i].Length; j++)
                {
                    output[i][j] = data[i][j];
                }
            }

            for (int i = 0; i < output.Length; i++)
            {
                int swapIndex = _random.Next(i, output.Length);
                double[] temp = output[swapIndex];
                output[swapIndex] = output[i];
                output[i] = temp;
            }
            return output;

        }


        public double[] Parse(double[] inputs)
        {
            double[] l = new double[inputs.Length];
            inputs.CopyTo(l, 0);
            for (int i = 0; i < _layers.Length; i++)
            {
                l = _layers[i].Parse(l);
            }
            return l;
        }


        //public double[] Parse(double[] inputs)
        //{
        //    List<double> input = new List<double>(inputs);
        //    List<double> output = new List<double>();
        //    for (int i = 0; i < Neurons.Length; i++)
        //    {
        //        for (int j = 0; j < Neurons[i].Length; j++)
        //        {
        //            output.Add(Neurons[i][j].Parse(input));
        //        }
        //        input = new List<double>(output);
        //        output = new List<double>();
        //    }
        //    return input.ToArray();
        //}

        public static double Sigmoid(double value)
        {
            
            return 1.0 / (1.0 + Math.Exp(value));
        }

        
    }
}
