#pragma once

#include "AudioMacro.h"
#include "AudioEffect.h"

struct FadeInEffect : public AudioEffect
{
public:
    FadeInEffect() {};
    ~FadeInEffect() {};

    short * StartProcess(short * data, int size, unsigned long shortsPerSecond) override {
        for (unsigned i = 0; i < size; ++i)
        {
            float t = i / static_cast<float>(size);
            data[i] = Clamp(data[i] * t);
        }
        return data;
    }
};