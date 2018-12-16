#pragma once

#include <Windows.h>
#include <assert.h>
#include <cstdio>

#ifdef AUDIOENGINE2_EXPORT 
#define AUDIO_ENGINE __declspec(dllexport)
#else
#define AUDIO_ENGINE __declspec(dllimport)
#endif

#define ae_assert(condition, msg) assert(condition && msg)

#define AUDIOENGINE_DEBUG

template<typename...args>
void a_printf(char const * const str, args... arguments)
{
#ifdef AUDIOENGINE_DEBUG
    printf(str, arguments...);
#endif
}

extern "C" {
    typedef LPVOID AE_HANDLE;
    typedef unsigned long ulong;
}
