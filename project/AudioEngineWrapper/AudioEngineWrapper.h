// AudioEngineWrapper.h

#pragma once


#include "AudioEngine.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace AudioEngineWrapper
{
	// TODO finish wrapper

	public ref class AudioEffect
	{
	protected:
		ws::AudioEffect * m_effect;
	public:
		~AudioEffect()
		{
			if (m_effect)
				delete m_effect;
		}
		ws::AudioEffect * GetEffect() { return m_effect; }
	};

	public ref class FadeInEffect : public AudioEffect
	{
	public:
		FadeInEffect();
	};

	public ref class FadeOutEffect : public AudioEffect
	{
	public:
		FadeOutEffect();
	};

	public ref class Audio
	{
	private:
		ws::Audio * m_audio;

		~Audio();

		Collections::Generic::List<AudioEffect^> m_effects;
	public:
		Audio(ws::Audio * audio)
		{
			m_audio = audio;
		}
		bool AddEffect(AudioEffect^ effect);

		void Play(float timeToPlay);
		void Stop(float timeToStop);

		property float Position
		{
			float get();
			void set(float value);
		}

        property array<byte>^ Data {
            array<byte>^ get();
        }
	};

	public ref class AudioEngine
	{
	public:
		static bool Initialize();

		// return Audio type
		static Audio^ Load(String^ file, bool enableTransitions, bool loop);

		static bool Shutdown();

        static property bool Initialized
        {
            bool get();
        }
	};



}

