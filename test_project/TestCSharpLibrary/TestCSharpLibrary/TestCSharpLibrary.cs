using System;
using System.Runtime.InteropServices;

namespace TestCSharpLibrary
{
    public class TestCSharpLibrary
    {
        // From c++ Dll (unmanaged)
        [DllImport("TestCPPLibrary")]
        public static extern float TestMultiply(float a, float b);

        // From c++ Dll (unmanaged)
        [DllImport("TestCPPLibrary")]
        public static extern float TestDivide(float a, float b);


        public static float SharpMultiply(float a, float b)
        {
            return (a * b);
        }

        public static float SharpDivide(float a, float b)
        {
            if (Math.Abs(b) < 0.001)
            {
                return 0;
            }

            return (a / b);
        }
    }
}
