// This is the main DLL file.

#include "stdafx.h"


#include "AudioEngineWrapper.h"


#include "FadeInEffect.h"
#include "FadeOutEffect.h"
#include <vcclr.h>
#include <cstdio>

namespace AudioEngineWrapper
{

	FadeInEffect::FadeInEffect()
	{
		m_effect = new ws::FadeInEffect();
	}

	FadeOutEffect::FadeOutEffect()
	{
		m_effect = new ws::FadeOutEffect();
	}

	Audio::~Audio()
	{
		ws::AudioEngine::Unload(m_audio);
	}

	bool Audio::AddEffect(AudioEffect^ effect)
	{
		// convert to ws::AudioEffect


		bool result = m_audio->AddEffect(effect->GetEffect());
		if (result)
		{
			m_effects.Add(effect);
		}

		return result;
	}

	void Audio::Play(float time)
	{
		ws::AudioTime timeToStart {time};
		ws::AudioEngine::Play(m_audio, timeToStart);
	}

	void Audio::Stop(float timeToStop)
	{
		ws::AudioTime toStop{ timeToStop };
		if (m_audio->IsPlaying())
			ws::AudioEngine::Stop(toStop);
	}

	float Audio::Position::get()
	{
		return m_audio->GetPosition().GetTotalSeconds();
	}

	void Audio::Position::set(float value)
	{
		m_audio->SetPosition(ws::AudioTime{ value });
	}

    array<byte>^ Audio::Data::get()
    {
        char * data = m_audio->GetData();
        int size = m_audio->GetSize();
        array<byte> ^ output = gcnew array<byte>(size);
        for (int i = 0; i < size; i++)
        {
            output[i] = data[i];
        }
        return output;
    }
	
	bool AudioEngine::Initialize()
	{
		return ws::AudioEngine::Initialize();
	}

	Audio^ AudioEngine::Load(String ^ file, bool enableTransitions, bool loop)
	{
		IntPtr filePtr = Marshal::StringToHGlobalAnsi(file);
		char * str = static_cast<char*>(filePtr.ToPointer());
		ws::Audio * a = ws::AudioEngine::Load(str, enableTransitions, loop);
		Marshal::FreeHGlobal(filePtr);
		return gcnew Audio{ a };
	}


	bool AudioEngine::Shutdown()
	{
		return ws::AudioEngine::Shutdown();
	}

    bool AudioEngine::Initialized::get()
	{
        return ws::AudioEngine::IsInitialized();
	}
}