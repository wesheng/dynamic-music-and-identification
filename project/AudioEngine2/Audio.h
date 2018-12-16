#pragma once

#include "AudioMacro.h"
#include "AudioTime.h"
#include "AudioEffect.h"

#include <vector>
#include <typeinfo>
#include <windows.h>


struct Audio {
    // Audio();
    // ~Audio();

    // Data in its purest form
    char * data = nullptr;
    int size;

    // Processed data
    char * dataCopy = nullptr;
    char * dataCopyStart = nullptr;

    // Temporary modified data
    char * dataCopyTemp = nullptr;
    char * dataCopyTempStart = nullptr;

    WAVEHDR hdr;
    // void * hdr;
    // HWAVEOUT wavOut;

    bool isPlaying;
    bool hasTransitions;

    // std::vector<AudioEffect *> m_effects;
    std::vector<AudioEffect *> effects;

    ulong position = 0;
};
