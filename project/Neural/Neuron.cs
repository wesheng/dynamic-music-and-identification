using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ws.Neural
{
    public class Neuron
    {
        private double[] Weights;

        public double Bias { get; set; } = 1.0;

        public double[] Inputs
        {
            get
            {
                double[] copy = new double[_inputs.Length];
                _inputs.CopyTo(copy, _inputs.Length);
                return copy;
            }
            private set => _inputs = value;
        }

        private double[] _inputs;
        public double Output { get; private set; } = 0.0;
        public double Error { get; set; } = 0.0;

        private readonly Random _random;

        public Neuron(int numInputs, double bias)
        {
            _random = new Random();
            Weights = new double[numInputs];
            _inputs = new double[numInputs];
            Bias = bias;
        }

        public Neuron(int numInputs)
        {
            _random = new Random();
            Weights = new double[numInputs];
            _inputs = new double[numInputs];
            Bias = _random.NextDouble();
        }

        public double Parse(double[] inputs)
        {
            // todo copy to _inputs
            double sum = 0;
            inputs.CopyTo(_inputs, 0);
            for (int i = 0; i < Weights.Length; i++)
            {
                if (i >= inputs.Length)
                    break;
                sum += inputs[i] * Weights[i];
            }
            sum += Bias;
            Output = Network.Sigmoid(sum);
            return Output;
        }

        public double Parse(List<double> inputs)
        {
            double sum = 0;
            inputs.CopyTo(_inputs, 0);
            for (int i = 0; i < Weights.Length; i++)
            {
                if (i >= inputs.Count)
                    break;
                sum += inputs.Count * Weights[i];
            }
            sum += Bias;
            Output = Network.Sigmoid(sum);
            return Output;
        }

        public double Parse(double input)
        {
            double sum = input * Weights[0];
            Inputs[0] = input;
            sum += Bias;
            Output = Network.Sigmoid(sum);
            return Output;
        }

        public void UpdateWeights(double learnRate)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] += Error * _inputs[i] * learnRate;
                Bias += Error * learnRate;
            }
        }

    }
}
