#include "Audio.h"
#include "AudioEngine.h"
#include "AudioEffect.h"

namespace ws {

	Audio::Audio()
	{

	}


	Audio::~Audio()
	{
		delete[] m_data;
        
        if (m_dataCopyStart)
            delete[] m_dataCopyStart;
        if (m_dataCopyTempStart)
            delete[] m_dataCopyTempStart;

		while (!m_effects.empty())
		{
			AudioEffect * e = m_effects.back();
			m_effects.pop_back();
			if (e)
				delete e;
		}
	}

	AudioTime Audio::GetPosition()
	{
		if (m_isPlaying)
		{
			m_position = AudioEngine::GetPosition(this);
			return m_position;
		}
		else
			return m_position;
	}

	void Audio::SetPosition(AudioTime time)
	{
		if (m_isPlaying)
		{
			AudioEngine::SetPosition(this, time);
		} else
		{
			m_position = time;
		}
	}

	bool Audio::IsPlaying() const
	{
		return m_isPlaying;
	}

	//

	//
	//void Audio::Play()
	//{
	//	AudioEngine::Play(this);
	//}
}