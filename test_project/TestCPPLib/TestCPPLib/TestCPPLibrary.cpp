#include "TestCPPLibrary.h"

extern "C" {

    float TestMultiply(float a, float b)
    {
        return a * b;
    }

    float TestDivide(float a, float b)
    {
        if (b == 0) return 0;
        return a / b;
    }


}