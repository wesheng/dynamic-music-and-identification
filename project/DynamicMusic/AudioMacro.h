#pragma once

#ifdef AUDIOENGINE_EXPORTS
#define AUDIO_ENGINE __declspec(dllexport)
#else
#define AUDIO_ENGINE __declspec(dllimport)
#endif

#define AUDIOENGINE_DEBUG

#include <cstdio>
template<typename...args>
void a_printf(char const * const str, args... arguments)
{
#ifdef AUDIOENGINE_DEBUG
	printf(str, arguments...);
#endif
}

using ulong = unsigned long;