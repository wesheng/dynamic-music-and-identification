#pragma once

#include "AudioMacro.h"
#include "AudioEffect.h"

extern "C" {

    AUDIO_ENGINE bool AudioEngineInitialize();

    AUDIO_ENGINE int AudioEngineAudioSize();
    AUDIO_ENGINE void AudioEngineLoad(char * file, bool enableTransitions, bool loop, AE_HANDLE& outHandle);
    AUDIO_ENGINE void AudioEngineLoadWithBasicTransitions(char * file, AE_HANDLE & outHandle);
    
    AUDIO_ENGINE void AudioEnginePlay(AE_HANDLE handle);
    AUDIO_ENGINE void AudioEnginePlayTime(AE_HANDLE handle, ulong timeToStart);
    AUDIO_ENGINE void AudioEnginePlayPos(AE_HANDLE handle, ulong position, ulong timeToStart);

    AUDIO_ENGINE void AudioEngineAddEffect(AE_HANDLE handle, AudioEffect * effect);

    AUDIO_ENGINE ulong AudioEngineGetPosition(AE_HANDLE handle);
    AUDIO_ENGINE ulong AudioEngineGetPositionBytes(AE_HANDLE handle);
    AUDIO_ENGINE void AudioEngineSetPosition(AE_HANDLE handle, ulong position);
    AUDIO_ENGINE int AudioEngineAudioDataSize(AE_HANDLE handle);
    AUDIO_ENGINE char * AudioEngineRetrieveData(AE_HANDLE handle);

    AUDIO_ENGINE void AudioEngineStopTime(ulong timeToStop);
    AUDIO_ENGINE void AudioEngineStop();

    AUDIO_ENGINE void AudioEngineUnload(AE_HANDLE handle);

    AUDIO_ENGINE ulong SecondsToBytes(AE_HANDLE handle, float seconds);
    AUDIO_ENGINE float BytesToSeconds(AE_HANDLE handle, ulong bytes);

    AUDIO_ENGINE bool AudioEngineShutdown();
};


