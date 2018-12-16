using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ws.Neural
{
    struct NeuronLayer
    {
        public Neuron[] Neurons;

        public NeuronLayer(int num, int numInputs, double bias)
        {
            Neurons = new Neuron[num];
            for (var i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(numInputs, bias);
            }
        }

        public NeuronLayer(int num, int numInputs, double[] biases)
        {
            Neurons = new Neuron[num];
            for (var i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(numInputs, biases[i]);
            }
        }

        public NeuronLayer(int num, int numInputs)
        {
            Neurons = new Neuron[num];
            for (var i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(numInputs);
            }
        }


        public double[] Parse(double[] input)
        {
            double[] output = new double[Neurons.Length];
            for (int i = 0; i < Neurons.Length; i++)
            {
                output[i] = Neurons[i].Parse(input);
            }
            return output;
        }

        public Neuron this[int index]
        {
            get => Neurons[index];
            set => Neurons[index] = value;
        }

    }
}
