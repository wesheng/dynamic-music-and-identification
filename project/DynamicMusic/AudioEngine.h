#pragma once
#include <windows.h>
#include "Audio.h"
#include "AudioTime.h"
#include "AudioMacro.h"
#include <vector>

namespace ws
{
	/**
		* \brief Engine class that manages Audio.
		*/
	class AUDIO_ENGINE AudioEngine
	{
	public:
		AudioEngine() = delete;
		~AudioEngine() = delete;

		/**
			* \brief Initializes the Engine.
			* \return Whether the Engine was initialized successfully.
			*/
		static bool Initialize();

		/**
			* \brief Loads an audio file for playback.
			* \brief Only supports WAV file formats.
			* \param file The file to load
			* \return An Audio instance
			*/
		static Audio * Load(char * file, bool enableTransitions = true, bool loop = false);

		static Audio * Mix(Audio * a1, Audio * a2);


		// static Audio * FadeToTrack(Audio * fadeTo, float time);

		/**
			* \brief Plays an audio instance.
			* \param audio The audio instance to play,
			*/
		static void Play(Audio * audio);
		static void Play(Audio * audio, AudioTime timeToStart);
		static void Play(Audio * audio, AudioTime position, AudioTime timeToStart);

		static void Pause(Audio * audio);
		static void SetVolume(float volume);
		static void SetVolume(float left, float right);
		static bool SetPitch(float pitch);
		static bool SetPlaybackSpeed(float speed);

		static AudioTime GetPosition(Audio * audio);
		static void SetPosition(Audio * audio, AudioTime time);

		/**
			* \brief Stops the currently playing audio instance.
			* \param timeToStop The amount of time until the audio completely stops.
			*/
		static void Stop(AudioTime timeToStop);
		static void Stop();

		/**
			* \brief Swaps the currently playing audio to the passed parameter.
			* \param to The audio to swap to
			* \param time The audio position to swap to
			*/
		static void Swap(Audio * to, AudioTime time);

		/**
			* /brief Unloads an audio file.
			* /param The audio instance to unload
			*/
		static void Unload(Audio * audio);

		/**
			* /brief Shutdowns the Engine. Currently does nothing.
			* /return Whether the Engine was shutdown successfully.
			*/
		static bool Shutdown();

        static bool IsInitialized() { return m_isInitialized; }
	private:
		static ulong AudioTimeToBytes(Audio* audio, AudioTime time);
		static AudioTime BytesToAudioTime(Audio* audio, ulong bytes);
		static void ResampleAudio(Audio* output, int sampleRate, int desiredSampleRate = 48000);

		static void ApplyPreEffects(Audio * audio);
		static void ApplyStartEffects(Audio * audio, short * data, int size, ulong shortsPerSecond);
		static void ApplyEndEffects(Audio * audio, short * data, int size, ulong shortsPerSecond);

		static void PrepareWaveHDR(WAVEHDR & hdr, ulong length, char * data);
		static void UnPrepareWaveHDR(WAVEHDR & hdr);

		static void AssertMMResult(MMRESULT result, const char * msg);
		static Audio *  ReadWaveData(char * buffer);

		static void PCMtoFloat(short * pcm, float * out, int pcmLen);
		static void FloattoPCM(float * f, short * out, int fLen);


        static bool m_isInitialized;

		static WAVEOUTCAPS m_waveCapabilities;
		static HWAVEOUT m_waveOut;
		static WAVEFORMATEX m_waveEx;
		static Audio * m_playing;

		static WAVEHDR m_hdrStart;
		static WAVEHDR m_hdrEnd;
		static WAVEHDR m_hdrNoLoop;
	};
}