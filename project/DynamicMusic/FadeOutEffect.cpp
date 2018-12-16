#include "FadeOutEffect.h"
#include <cstdio>

namespace ws {
	short* FadeOutEffect::EndProcess(short* data, int size, unsigned long shortsPerSecond)
	{
		for (unsigned i = 0; i < size; ++i)
		{
			float t = ((size - i) / static_cast<float>(size));
			data[i] = Clamp(data[i] * t);
		}
		return data;
	}
}