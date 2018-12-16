using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Controls;
using Accord.IO;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Kernels;
using Gaussian = Accord.Statistics.Kernels.Gaussian;

namespace AccordTest
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            double[][] inputs =
            {
                new double[] {0},
                new double[] {3},
                new double[] {1},
                new double[] {2},
            };

            int[][] outputs =
            {
                new[] {-1, 1, -1},
                new[] {-1, -1, 1},
                new[] {1, 1, -1},
                new[] {-1, -1, -1}
            };

            var teacher = new MultilabelSupportVectorLearning<Linear>()
            {
                Learner = (p) => new SequentialMinimalOptimization<Linear>()
                {
                    Complexity = 10000.0
                }
            };

            var svm = teacher.Learn(inputs, outputs);
            
            double[][] results = svm.Probabilities(inputs);
            //double[][] results = new double[4][];
            //for (int i = 0; i < results.Length; i++)
            //    results[i] = new double[4];

            //double[][] results2 = svm.Decide(inputs, results);

            int[] maxAnswers = svm.ToMulticlass().Decide(inputs);

            

            Console.ReadKey();
        }

        private static void LoadExcel()
        {
            ExcelReader reader = new ExcelReader(@"C:\Users\wsheng\Documents\Neumont\Q9\capstone\repo\project\AccordTest\examples.xlsx");

            // DataTable table = reader.GetWorksheet("Classification - Yin Yang");
            DataTable table = reader.GetWorksheet("Classification - Yin Yang");

            double[][] inputs = table.ToJagged<double>("X", "Y");
            int[] outputs = table.Columns["G"].ToArray<int>();
            ScatterplotBox.Show("Yin-Yang", inputs, outputs).Hold();
        }

        private static void XorLearn()
        {
            double[][] inputs =
            {
                new double[] {0, 0},
                new double[] {1, 0},
                new double[] {0, 1},
                new double[] {1, 1}
            };

            int[] outputs =
            {
                0, 1, 1, 0
            };

            var smo = new SequentialMinimalOptimization<Gaussian>()
            {
                Complexity = 100
            };

            var svm = smo.Learn(inputs, outputs);

            bool[] prediction = svm.Decide(inputs);

            double error = new AccuracyLoss(outputs).Loss(prediction);
            Console.WriteLine("Error: " + error);

            ScatterplotBox.Show("Training data", inputs, outputs);
            ScatterplotBox.Show("SVM results", inputs, prediction.ToZeroOne());
        }
    }
}
