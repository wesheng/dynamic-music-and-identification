#pragma once

#include "AudioMacro.h"
#include "AudioEngine.h"

extern "C" {
	AUDIO_ENGINE bool Initialize()
	{
		return AudioEngine::Initialize();
	}

	AUDIO_ENGINE bool Shutdown()
	{
		return AudioEngine::Shutdown();
	}
}