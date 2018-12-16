#pragma once

#include "AudioEffect.h"
#include "AudioMacro.h"

struct FadeOutEffect : public AudioEffect
{
public:
    FadeOutEffect() {};
    ~FadeOutEffect() {};
private:
    short * EndProcess(short * data, int size, unsigned long shortsPerSecond) override {
        for (unsigned i = 0; i < size; ++i)
        {
            float t = ((size - i) / static_cast<float>(size));
            data[i] = Clamp(data[i] * t);
        }
        return data;
    }
};
