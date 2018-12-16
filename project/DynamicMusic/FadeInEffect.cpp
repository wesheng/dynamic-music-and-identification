#include "FadeInEffect.h"
#include <cstdio>

namespace ws {
	short * FadeInEffect::StartProcess(short * data, int size, unsigned long shortsPerSecond)
	{
		for (unsigned i = 0; i < size; ++i)
		{
			float t = i / static_cast<float>(size);
			data[i] = Clamp(data[i] * t);
		}
		return data;
	}
}