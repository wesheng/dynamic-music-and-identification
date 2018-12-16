using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ws.Neural
{
    // Helper math functions
    public class NeuralMath
    {
        public static double Sigmoid(double value)
        {
            return 1.0 / (1.0 + Math.Exp(-value));
        }

        public static double SigmoidPrime(double value)
        {
            return Sigmoid(value) * (1.0 - Sigmoid(value));
        }
    }
}
