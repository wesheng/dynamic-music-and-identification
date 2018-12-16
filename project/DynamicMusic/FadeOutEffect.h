#pragma once
#include "AudioEffect.h"
#include "AudioMacro.h"

namespace ws {
	class AUDIO_ENGINE FadeOutEffect : public AudioEffect
	{
	public:
		FadeOutEffect() = default;
		~FadeOutEffect() = default;
	private:
		short * EndProcess(short * data, int size, unsigned long shortsPerSecond) override;
	};

}