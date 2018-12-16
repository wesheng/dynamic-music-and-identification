#pragma once

#include "AudioEffect.h"
#include "AudioMacro.h"

namespace ws {

	/**
	 * \ingroup AudioEffect
	 * \brief Audio effect that applies a fade out effect to audio.
	 */
	class AUDIO_ENGINE FadeInEffect : public AudioEffect
	{
	public:
		FadeInEffect() = default;
		~FadeInEffect() = default;
	private:
		short * StartProcess(short * data, int size, unsigned long shortsPerSecond) override;
	};
}