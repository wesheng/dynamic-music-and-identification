#include "AudioEngine.h"

#include <windows.h>
#include "Audio.h"
#include <cstdio>
#include <cassert>
#include "AudioMacro.h"
#include "AudioEffect.h"

#include <mmreg.h>
#include <fstream>
#include <mmeapi.h>
#include "samplerate.h"
#include "WaveFormat.h"

namespace ws {

    bool AudioEngine::m_isInitialized = false;

	WAVEOUTCAPS AudioEngine::m_waveCapabilities;
	HWAVEOUT AudioEngine::m_waveOut;
	WAVEFORMATEX AudioEngine::m_waveEx;
	Audio * AudioEngine::m_playing;

	WAVEHDR AudioEngine::m_hdrStart;
	WAVEHDR AudioEngine::m_hdrEnd;
	WAVEHDR AudioEngine::m_hdrNoLoop;

	bool AudioEngine::Initialize()
	{
        if (m_isInitialized) return true;

		waveOutGetDevCaps(WAVE_MAPPER, &m_waveCapabilities, sizeof(WAVEOUTCAPS));

		return true;
	}

	Audio * AudioEngine::Load(char* file, bool enableTransitions, bool loop)
	{
		std::ifstream stream;
		stream.open(file, std::ios_base::binary);
		assert(stream.good() && "Could not read file");
		stream.seekg(0, std::ios::end);
		int length = stream.tellg();
		stream.seekg(0, std::ios::beg);
		char * buffer = new char[length];
		stream.read(buffer, length);
		stream.close();

		Audio * output = ReadWaveData(buffer);

        if (enableTransitions) {
            output->m_dataCopyStart = new char[output->m_size];
            output->m_dataCopyTempStart = new char[output->m_size];


            memcpy_s(output->m_dataCopyStart, output->m_size, buffer, output->m_size);

            output->m_dataCopy = output->m_dataCopyStart;
            output->m_dataCopyTemp = output->m_dataCopyTempStart;
        }


		delete[] buffer;
		return output;
	}


    Audio * AudioEngine::Mix(Audio * a1, Audio * a2)
	{
		short * data1 = reinterpret_cast<short *>(a1->m_data);
		short * data2 = reinterpret_cast<short *>(a2->m_data);

		Audio * output = new Audio();

		output->m_size = (a1->m_size > a2->m_size ? a1->m_size : a2->m_size);

		short * outputData = new short[output->m_size / 2];

		int halfSize = output->m_size / 2;
		for (int i = 0; i < halfSize; ++i)
		{
			short d1, d2;
			if (i > halfSize)
				d1 = 0;
			else
				d1 = data1[i];
			if (i > halfSize)
				d2 = 0;
			else
				d2 = data2[i];

			outputData[i] = AudioEffect::Clamp(d1 + d2);
		}

		output->m_data = reinterpret_cast<char *>(outputData);


		output->m_wavehdr.lpData = output->m_dataCopy;
		output->m_wavehdr.dwBufferLength = output->m_size;
		output->m_wavehdr.dwFlags = WHDR_BEGINLOOP | WHDR_ENDLOOP;
		output->m_wavehdr.dwLoops = -1;

		return output;
	}

	void AudioEngine::ApplyPreEffects(Audio * audio)
	{
		memcpy_s(audio->m_dataCopyStart, audio->m_size, audio->m_data, audio->m_size);
		audio->m_dataCopy = audio->m_dataCopyStart;

		ulong shortsPerSecond = m_waveEx.nAvgBytesPerSec / 2;
		for (AudioEffect * e : audio->m_effects)
		{
			e->PreProcess(reinterpret_cast<short *>(audio->m_dataCopy), audio->m_size / 2, shortsPerSecond);
		}
	}

	void AudioEngine::ApplyStartEffects(Audio * audio, short* data, int size, ulong shortsPerSecond)
	{
		for (AudioEffect * e : audio->m_effects)
		{
			audio->m_dataCopy = reinterpret_cast<char *>(e->StartProcess(reinterpret_cast<short *>(data), size / 2, shortsPerSecond));
		}
	}

	void AudioEngine::ApplyEndEffects(Audio * audio, short* data, int size, ulong shortsPerSecond)
	{
		for (AudioEffect * e : audio->m_effects)
		{
			e->EndProcess(reinterpret_cast<short *>(data), size / 2, shortsPerSecond);
		}
	}

	void AudioEngine::Play(Audio* audio)
	{
		//MMRESULT result;
		if (m_playing) Stop();

		ApplyPreEffects(audio);

		MMRESULT result = waveOutOpen(&m_waveOut, WAVE_MAPPER, &m_waveEx, NULL, NULL, CALLBACK_NULL);
		AssertMMResult(result, "WaveOutOpen failed");

		result = waveOutPrepareHeader(m_waveOut, &audio->m_wavehdr, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutPrepareHeader failed");

		result = waveOutWrite(m_waveOut, &audio->m_wavehdr, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		m_playing = audio;
		audio->m_isPlaying = true;
	}

	void AudioEngine::Play(Audio* audio, AudioTime timeToStart)
	{
		if (m_playing) Stop();

		ApplyPreEffects(audio);

		ulong duration = AudioTimeToBytes(audio, timeToStart);

		memcpy_s(audio->m_dataCopyTemp, duration, audio->m_dataCopy, duration);

		short* data = reinterpret_cast<short *>(audio->m_dataCopyTemp);
		ApplyStartEffects(audio, data, duration / 2, m_waveEx.nAvgBytesPerSec / 2);

		MMRESULT result = waveOutOpen(&m_waveOut, WAVE_MAPPER, &m_waveEx, NULL, NULL, CALLBACK_NULL);
		AssertMMResult(result, "WaveOutOpen failed");

		PrepareWaveHDR(m_hdrNoLoop, duration, audio->m_dataCopyTemp);

		char * startData = audio->m_dataCopyStart;
		startData += duration;

		PrepareWaveHDR(m_hdrStart, audio->m_size - duration, startData);
		PrepareWaveHDR(m_hdrEnd, duration, audio->m_dataCopyStart);

		m_hdrStart.dwFlags |= WHDR_BEGINLOOP;
		m_hdrEnd.dwFlags |= WHDR_ENDLOOP;

		result = waveOutWrite(m_waveOut, &m_hdrNoLoop, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		result = waveOutWrite(m_waveOut, &m_hdrStart, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		result = waveOutWrite(m_waveOut, &m_hdrEnd, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		m_playing = audio;
		audio->m_isPlaying = true;
	}

	void AudioEngine::PrepareWaveHDR(WAVEHDR & hdr, ulong length, char * data)
	{
		hdr = WAVEHDR{};
		hdr.dwBufferLength = length;
		hdr.lpData = data;

		MMRESULT result = waveOutPrepareHeader(m_waveOut, &hdr, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutPrepareHeader failed");
	}

	void AudioEngine::UnPrepareWaveHDR(WAVEHDR& hdr)
	{
		if (hdr.lpData)
		{
			waveOutUnprepareHeader(m_waveOut, &hdr, sizeof(WAVEHDR));
			hdr.lpData = nullptr;
		}
	}

	void AudioEngine::Play(Audio* audio, AudioTime position, AudioTime timeToStart)
	{
		if (m_playing) Stop();

		ApplyPreEffects(audio);

		ulong duration = AudioTimeToBytes(audio, timeToStart);
		ulong pos = AudioTimeToBytes(audio, position);

		audio->m_dataCopy = audio->m_dataCopyStart + pos;
		audio->m_dataCopyTemp = audio->m_dataCopyTempStart + pos;

		memcpy_s(audio->m_dataCopyTemp, duration, audio->m_dataCopy, duration);

		//short* data = reinterpret_cast<short *>(audio->m_dataCopyTemp);
		//ApplyStartEffects(audio, data, duration / 2, audio->m_waveEx.nAvgBytesPerSec / 2);

		MMRESULT result = waveOutOpen(&m_waveOut, WAVE_MAPPER, &m_waveEx, NULL, NULL, CALLBACK_NULL);
		AssertMMResult(result, "WaveOutOpen failed");

		char * clipData = audio->m_dataCopyTemp;

		ApplyStartEffects(audio, reinterpret_cast<short*>(clipData), duration / 2, m_waveEx.nAvgBytesPerSec / 2);

		//m_hdrNoLoop = WAVEHDR{};
		//m_hdrNoLoop.dwBufferLength = duration;
		//m_hdrNoLoop.dwFlags = 0;
		//m_hdrNoLoop.lpData = clipData;

		//result = waveOutPrepareHeader(m_waveOut, &m_hdrNoLoop, sizeof(WAVEHDR));
		//AssertMMResult(result, "waveOutPrepareHeader failed");

		PrepareWaveHDR(m_hdrNoLoop, duration, clipData);

		char * startData = audio->m_dataCopyStart;
		startData += duration + pos;

		//m_hdrStart = WAVEHDR{};
		//m_hdrStart.dwBufferLength = audio->m_size - duration - pos;
		//m_hdrStart.lpData = startData;

		//result = waveOutPrepareHeader(m_waveOut, &m_hdrStart, sizeof(WAVEHDR));
		//AssertMMResult(result, "waveOutPrepareHeader failed");

		PrepareWaveHDR(m_hdrStart, audio->m_size - duration - pos, startData);

		PrepareWaveHDR(m_hdrEnd, duration + pos, audio->m_dataCopy);

		m_hdrStart.dwFlags |= WHDR_BEGINLOOP;
		m_hdrEnd.dwFlags |= WHDR_ENDLOOP;

		result = waveOutWrite(m_waveOut, &m_hdrNoLoop, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		result = waveOutWrite(m_waveOut, &m_hdrStart, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		result = waveOutWrite(m_waveOut, &m_hdrEnd, sizeof(WAVEHDR));
		AssertMMResult(result, "waveOutWrite failed");

		m_playing = audio;
		audio->m_isPlaying = true;
	}

	void AudioEngine::Pause(Audio * audio)
	{
		MMRESULT result = waveOutPause(m_waveOut);
		AssertMMResult(result, "waveOutWrite failed");

	}

	void AudioEngine::SetVolume(float volume)
	{
		// FF - max, 00 - min
		SetVolume(volume, volume);
	}

	void AudioEngine::SetVolume(float left, float right)
	{
		DWORD l = 0x0000FFFF * left;
		DWORD r = 0xFFFF0000 * right;
		MMRESULT result = waveOutSetVolume(m_waveOut, l | r);
		AssertMMResult(result, "waveOutSetVolume failed");
	}

	bool AudioEngine::SetPitch(float pitch)
	{
		if (m_waveCapabilities.dwSupport & WAVECAPS_PITCH == 0) return false;
		DWORD p = pitch * 0x00010000;
		MMRESULT result = waveOutSetPitch(m_waveOut, p);
		AssertMMResult(result, "waveOutSetPitch failed");
		return true;
	}

	bool AudioEngine::SetPlaybackSpeed(float speed)
	{
		if (m_waveCapabilities.dwSupport & WAVECAPS_PLAYBACKRATE == 0) return false;
		DWORD rate = speed * 0x00010000;
		MMRESULT result = waveOutSetPlaybackRate(m_waveOut, rate);
		AssertMMResult(result, "waveOutSetPitch failed");

		return true;
	}

	ulong AudioEngine::AudioTimeToBytes(Audio* audio, AudioTime time)
	{
		return time.GetTotalMilliseconds() * (m_waveEx.nAvgBytesPerSec / 1000);
	}

	AudioTime AudioEngine::BytesToAudioTime(Audio * audio, ulong bytes)
	{
		AudioTime output;
		ulong milliseconds = bytes / (m_waveEx.nAvgBytesPerSec / 1000);
		output.SetTotalMilliseconds(milliseconds);
		return output;
	}

	void AudioEngine::SetPosition(Audio * audio, AudioTime time)
	{
		if (m_playing == audio)
		{
			Stop();
			m_playing = audio;
		}

		ulong byteOffset = AudioTimeToBytes(audio, time) % audio->m_size;

		WAVEHDR hdr = audio->m_wavehdr;

		hdr.lpData += byteOffset;
		hdr.dwBufferLength -= byteOffset;

		waveOutWrite(m_waveOut, &hdr, sizeof(WAVEHDR));
		m_playing->m_position = BytesToAudioTime(m_playing, byteOffset);
	}

	AudioTime AudioEngine::GetPosition(Audio* audio)
	{
		MMTIME time;
		time.wType = TIME_BYTES;
		MMRESULT result = waveOutGetPosition(m_waveOut, &time, sizeof(MMTIME));
		AssertMMResult(result, "waveOutGetPosition failed");

		ulong bytes = time.u.cb % audio->m_size;
		AudioTime output = BytesToAudioTime(audio, bytes);

		return output;
	}

	void AudioEngine::Stop(AudioTime timeToStop)
	{
		//Pause(audio);

		if (m_playing)
		{

			short * data = reinterpret_cast<short *>(m_playing->m_dataCopyStart);
			int size = m_playing->m_size;

			AudioTime pos = GetPosition(m_playing);
			ulong offset = AudioTimeToBytes(m_playing, pos);
			ulong bytesToStop = AudioTimeToBytes(m_playing, timeToStop);

			data += offset;
			int offsetSize = size - offset;
			offsetSize = bytesToStop < offsetSize ? bytesToStop : offsetSize;
			size = offsetSize;

			memcpy_s(m_playing->m_dataCopyTemp, size, data, size);
			ApplyEndEffects(m_playing, reinterpret_cast<short *>(m_playing->m_dataCopyTemp), size, m_waveEx.nAvgBytesPerSec / 2);
			m_playing->m_dataCopy = reinterpret_cast<char *>(data);
			waveOutReset(m_waveOut);


			PrepareWaveHDR(m_hdrNoLoop, size, m_playing->m_dataCopyTemp);

			MMRESULT result = waveOutWrite(m_waveOut, &m_hdrNoLoop, sizeof(WAVEHDR));
			AssertMMResult(result, "waveOutWrite failed");

			m_playing->m_position = pos;
			m_playing->m_isPlaying = false;
		}

		m_playing = nullptr;
	}

	void AudioEngine::Stop()
	{
		waveOutReset(m_waveOut);
		waveOutClose(m_waveOut);

		UnPrepareWaveHDR(m_hdrStart);
		UnPrepareWaveHDR(m_hdrEnd);
		UnPrepareWaveHDR(m_hdrNoLoop);


		m_playing = nullptr;
	}

	void AudioEngine::Swap(Audio* to, AudioTime time)
	{
		int shortPerSecond = m_waveEx.nAvgBytesPerSec / 2;
		if (m_playing)
		{
			short * data = reinterpret_cast<short *>(m_playing->m_data);
			int size = m_playing->m_size / 2;

			// offset data & size

			short * copyData = new short[size];
			memcpy_s(copyData, size, data, size);
			ApplyEndEffects(m_playing, data, size, shortPerSecond);
		}

		short * data2 = reinterpret_cast<short *>(to->m_data);
		int size2 = to->m_size / 2;

		memcpy_s(to->m_dataCopy, size2, data2, size2);

		ApplyStartEffects(to, reinterpret_cast<short *>(to->m_dataCopy), size2, shortPerSecond);

		// combine the two

	}

	void AudioEngine::Unload(Audio* audio)
	{
		delete audio;
	}

	bool AudioEngine::Shutdown()
	{
        if (m_isInitialized)
        {
            Stop();
            return true;
        }

        return true;
	
	}

	void AudioEngine::AssertMMResult(MMRESULT result, const char * msg)
	{
		if (result == MMSYSERR_NOERROR) return;

		char buffer[MAXERRORLENGTH];
		waveOutGetErrorText(result, buffer, MAXERRORLENGTH);

		a_printf(", %s [%s]\n", msg, buffer);
		assert(false);
	}

#include "intrin.h" 
	Audio * AudioEngine::ReadWaveData(char * buffer)
	{
		// wave file format, credit to http://soundfile.sapp.org/doc/WaveFormat/
		/*
		 *  Endian | Offset |     Name      |     Size
		 * --------+--------+---------------+--------------
		 *     big |      0 |    ChunkId    |             4   }
		 *  little |      4 |   ChunkSize   |             4    ---- RIFF chunk descriptor
		 *     big |      8 |    Format     |             4   }
		 * --------+--------+---------------+--------------
		 *     big |     12 |  Subchunk1ID  |             4   }
		 *  little |     16 | Subchunk1Size |             4    |
		 *  little |     20 |  AudioFormat  | 	          2    |
		 *  little |     22 |  NumChannels  | 	          2    |
		 *  little |     24 |   SampleRate  | 	          4    ---- fmt sub-chunk
		 *  little |     28 |    ByteRate   | 	          4    |
		 *  little |     32 |   BlockAlign  | 	          2    |
		 *  little |     34 | BitsPerSample | 	          2   }
		 *  -------+--------+---------------+--------------
		 *     big |     36 |  Subchunk2ID  |             4  }
		 *  little |     40 | Subchunk2Size |             4   ---- data sub-chunk
		 *  little |     44 |     data      | Subchunk2Size  }
		 */


		WAVE_HEADER * header = reinterpret_cast<WAVE_HEADER*>(buffer);
		buffer += sizeof(WAVE_HEADER);
		WAVE_DATA * data = reinterpret_cast<WAVE_DATA*>(buffer);
		buffer += sizeof(WAVE_DATA);

		//WAVE_HEADER h = *header;
		//WAVE_DATA d = *data;

		Audio * output = new Audio;

		//data->size *= 48000 / header->sampleRate;
		output->m_size = data->size;
		output->m_size *= 48000 / header->sampleRate;

		output->m_data = new char[output->m_size];
        memcpy_s(output->m_data, output->m_size, buffer, data->size);

		m_waveEx.wFormatTag = WAVE_FORMAT_PCM;
		m_waveEx.nChannels = header->channels;
		m_waveEx.nSamplesPerSec = header->sampleRate;

		// a_printf("Sample Rate: %d\n", m_waveEx.nSamplesPerSec);
		// a_printf("Block Align: %d\n", header->blockAlign);
		// a_printf("bitsPerSample: %d\n", header->bitsPerSample);
		// a_printf("Size: %d\n", output->m_size);

		m_waveEx.nAvgBytesPerSec = header->sampleRate * header->blockAlign;
		m_waveEx.nBlockAlign = header->blockAlign;
		m_waveEx.wBitsPerSample = header->bitsPerSample;

		// ResampleAudio(output, m_waveEx.nSamplesPerSec);

		// m_waveEx.nSamplesPerSec = 48000;

		return output;
	}

	void AudioEngine::ResampleAudio(Audio* output, int sampleRate, int desiredSampleRate)
	{
		if (m_waveEx.nSamplesPerSec == 48000) return;

		int shortSize = output->m_size / 2;
		int fSize = shortSize;
		float * tempIn = new float[fSize];
		float * tempOut = new float[fSize * (48000 / sampleRate)];

		PCMtoFloat(reinterpret_cast<short*>(output->m_data), tempIn, shortSize);

		SRC_DATA srcData;
		srcData.data_in = tempIn;
		srcData.input_frames = fSize / 2;
		srcData.data_out = tempOut;
		srcData.output_frames = fSize / 2;
		srcData.src_ratio = 48000.0f / sampleRate;

		if (srcData.data_in + srcData.input_frames * m_waveEx.nChannels > srcData.data_out)
		{
			a_printf("oh no! %p\n", srcData.data_in);
		}

		int srcResult = src_simple(&srcData, SRC_SINC_FASTEST, m_waveEx.nChannels);

		if (srcResult != 0)
		{
			a_printf("%d: %s\n", srcResult, src_strerror(srcResult));
			assert(false);
		}

		FloattoPCM(tempOut, reinterpret_cast<short*>(output->m_data), srcData.output_frames_gen);

        if (output->m_dataCopyStart)
		    memcpy_s(output->m_dataCopyStart, output->m_size, output->m_data, output->m_size);

		delete[] tempIn;
		delete[] tempOut;
	}


	void AudioEngine::PCMtoFloat(short* pcm, float* out, int pcmLen)
	{
		// 16 bit range = 32768
		for (int i = 0; i < pcmLen; i++)
		{
			float * p = out + i;
			*p = *(pcm + i) / 32768.0f;
			if (*p > 1) *p = 1;
			if (*p < -1) *p = -1;
		}
	}

	void AudioEngine::FloattoPCM(float* f, short* out, int fLen)
	{
		for (int i = 0; i < fLen; i++)
		{
			short * p = out + i;
			*p = *(f + i) * 32768;
			if (*p > 32767) *p = 32767;
			if (*p < -32768) *p = -32768;
		}
	}

}
