using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicAnalysis
{
    [Serializable]
    public class Standardizer
    {
        private double sum;
        private double avg;
        private int count;
        private int count2;
        private double deviation;
        private double deviation2;
        private static double Modifier = 1;

        public Standardizer()
        {
            sum = 0.0;
            count = 0;
            count2 = 0;
            deviation = 0;
            deviation2 = 0;
        }

        public void PreprocessAdd(double[] arr)
        {
            count += arr.Length;
            for (int i = 0; i < arr.Length; ++i)
            {
                sum += arr[i];
            }
        }

        public void PreprocessFinishAdd()
        {
 
            avg = sum / count;
        }

        public double[] PreprocessCalculateDeviation(double[] arr)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                double x = arr[i] - avg;
                x *= x;
                deviation += x;
                count2++;
            }
            count2 -= 1;
            return arr;
        }

        public void PreprocessFinishDeviation()
        {
            deviation = Math.Sqrt(deviation / count2);
        }

        public static double[] ProcessCalculate(double[] arr)
        {

            int c = arr.Length;
            double a = 0, a2 = 0, d = 0, d2 = 0;
            for (int i = 0; i < arr.Length; ++i)
            {
                a += arr[i];
            }
            a /= c;
            for (int i = 0; i < arr.Length; ++i)
            {
                double x = arr[i] - a;
                x *= x;
                d += x;
            }
            d = Math.Sqrt(d / (c - 1));

            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = (arr[i] - a) / d;
                arr[i] *= Modifier;
                a2 += arr[i];
            }
            a2 /= c;
            for (int i = 0; i < arr.Length; ++i)
            {
                double x = arr[i] - a2;
                x *= x;
                d2 += x;
            }
            d2 = Math.Sqrt(d2 / (c - 1));
            Debug.WriteLine("d2: {0}", d2);

            //double arrAvg = 0, arrAvg2 = 0;
            ////for (int i = 0; i < arr.Length; ++i)
            ////{
            ////    arrAvg += arr[i];
            ////}
            ////arrAvg /= arr.Length;




            //for (int i = 0; i < arr.Length; ++i)
            //{
            //    arr[i] = (arr[i] - avg);
            //    arrAvg += arr[i];
            //}

            //arrAvg /= arr.Length;
            //Debug.WriteLine("ArrAvg: {0}. Deviation: {1}", arrAvg, deviation);

            //for (int i = 0; i < arr.Length; ++i)
            //{
            //    arr[i] = arr[i] / deviation;
            //    arr[i] *= Modifier;
            //    arrAvg2 += arr[i];
            //}

            //arrAvg2 /= arr.Length;
            //Debug.WriteLine("ArrAvg2: {0}", arrAvg2);

            //for (int i = 0; i < arr.Length; ++i)
            //{
            //    double x = arr[i] - arrAvg2;
            //    x *= x;
            //    deviation2 += x;
            //}

            return arr;
        }

        public double TestDeviation2
        {
            get
            {
                return Math.Sqrt(deviation2 / count2);
            }
        }

        public void ResetDeviation2()
        {
            deviation2 = 0;
        }
    }
}
