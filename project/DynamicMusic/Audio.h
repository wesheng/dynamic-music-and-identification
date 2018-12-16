#pragma once

#include <windows.h>
#include "AudioMacro.h"
#include <vector>
#include <typeinfo>
#include "AudioTime.h"

namespace ws {
	/**
	* \brief Audio class that holds music information.
	* Automatically removes all effects that is still held.
	* \param You may only obtain an instance of this class through the AudioEngine::Load() method.
	*/
	class AUDIO_ENGINE Audio
	{
		friend class AudioEngine;
		friend class AudioEffect;

		Audio();
		~Audio();

		// Data in its purest form
		char * m_data;
		int m_size;

		// Processed data
		char * m_dataCopy;
		char * m_dataCopyStart;

		// Temporary modified data
		char * m_dataCopyTemp;
		char * m_dataCopyTempStart;

		WAVEHDR m_wavehdr;
		HWAVEOUT m_waveOut;

		bool m_isPlaying;

		std::vector<AudioEffect *> m_effects;

		AudioTime m_position;
	public:
		/**
			* \brief Adds an audio effect.
			* \param effect The audio effect to add.
			* \return Whether the effect already exists
			*/
		template<typename T>
		bool AddEffect(T * effect);

		/**
			* \brief Removes an audio effect.
			* \tparam T The AudioEffect type to remove.
			* \return The specified AudioEffect.
			*/
		template<typename T>
		AudioEffect * RemoveEffect();
		//void Play();

		AudioTime GetPosition();

	    /**
		 * \brief Sets the audio position to the specified position
		 * \param time 
		 */
		void SetPosition(AudioTime time);
		// void SetPosition(AudioTime position);
		bool IsPlaying() const;

        char * GetData() const
        {
            return m_data;
        }
        int GetSize() const
        {
            return m_size;
        }

	};

	template <typename T>
	bool Audio::AddEffect(T* effect)
	{
		for (AudioEffect * e : m_effects)
		{
			if (typeid(T) == typeid(e))
			{
				return false;
			}
		}
		m_effects.push_back(effect);
		return true;
	}


	template <typename T>
	AudioEffect* Audio::RemoveEffect()
	{
		for (int i = 0; i < m_effects.size(); ++i)
		{
			if (typeid(T) == typeid(m_effects[i]))
			{
				AudioEffect * e = m_effects[i];
				m_effects.erase(m_effects.begin() + i);
				return e;
			}
		}
		return nullptr;
	}
}