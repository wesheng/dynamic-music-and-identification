#include "AudioEngine2.h"
#include "WaveFormat.h"
#include "Audio.h"

#include "FadeInEffect.h"
#include "FadeOutEffect.h"

#include <windows.h>
#include <fstream>


namespace {
    bool l_isInitialized = false;
    WAVEOUTCAPS l_waveCapabilities;
    HWAVEOUT l_waveOut;
    WAVEFORMATEX l_waveEx;

    WAVEHDR l_hdrStart;
    WAVEHDR l_hdrEnd;
    WAVEHDR l_hdrNoLoop;

    Audio * l_playing;

    // WAVEHDR tempHdr;
}

ulong MillisecondsToBytes(Audio* audio, ulong time);
ulong BytesToMilliseconds(Audio* audio, ulong bytes);

void ApplyPreEffects(Audio * audio);
void ApplyStartEffects(Audio * audio, short * data, int size, ulong shortsPerSecond);
void ApplyEndEffects(Audio * audio, short * data, int size, ulong shortsPerSecond);

void PrepareWaveHDR(WAVEHDR & hdr, ulong length, char * data);
void UnPrepareWaveHDR(WAVEHDR & hdr);

void AssertMMResult(MMRESULT result, const char * msg);
Audio * ReadWaveData(char * buffer);

extern "C" {

    /// <summary>
    /// Initializes the Audio Engine.
    /// </summary>
    /// <returns>Whether the engine was initialized successfully or not</returns>
    bool AudioEngineInitialize()
    {
        if (l_isInitialized) return true;
        waveOutGetDevCaps(WAVE_MAPPER, &l_waveCapabilities, sizeof(WAVEOUTCAPS));
        l_playing = nullptr;
        return l_isInitialized = true;
    }

    /// <summary>
    /// Returns the size of the audio object.
    /// </summary>
    /// <returns>The size of the audio object.</returns>
    int AudioEngineAudioSize()
    {
        return sizeof(Audio);
    }

    /// <summary>
    /// Loads an audio file.
    /// </summary>
    /// <param name="file">The file location</param>
    /// <param name="enableTransitions">if set to <c>true</c> [enable transitions].</param>
    /// <param name="loop">if set to <c>true</c> [loop].</param>
    /// <param name="outHandle">The handle of the audio object.</param>
    void AudioEngineLoad(char * file, bool enableTransitions, bool loop, AE_HANDLE & outHandle)
    {
        std::ifstream stream;
        stream.open(file, std::ios_base::binary);
        
        ae_assert(stream.good(), "Could not read passed file");

        stream.seekg(0, std::ios::end);
        int length = (int) stream.tellg();
        stream.seekg(0, std::ios::beg);
        char * buffer = new char[length];
        stream.read(buffer, length);
        stream.close();

        Audio * output = ReadWaveData(buffer);

        output->hasTransitions = enableTransitions;
        // WAVEHDR * hdr = new WAVEHDR;
        //hdr->lpData = output->data;
        //hdr->dwBufferLength = output->size;
        //hdr->dwLoops = loop ? -1 : 0;
        //hdr->dwUser = 0;
        //hdr->dwBytesRecorded = 0;
        //
        //loop ? hdr->dwFlags = WHDR_BEGINLOOP | WHDR_ENDLOOP : 0;
        // output.hdr = hdr;
        // output->wavOut = malloc(sizeof(HWAVEOUT));

        if (enableTransitions) {
            output->dataCopyStart = (char*) malloc(output->size);
            output->dataCopyTempStart = (char*) malloc(output->size);
            // output->dataCopyStart = new char[output->size];
            // output->dataCopyTempStart = new char[output->size];
            


            memcpy_s(output->dataCopyStart, output->size, buffer, output->size);

            output->dataCopy = output->dataCopyStart;
            output->dataCopyTemp = output->dataCopyTempStart;
        }

        free(buffer);
        outHandle = (void *) output;
    }

    /// <summary>
    /// Loads an audio file and adds fade in & fade out transitions
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="outHandle">The out handle.</param>
    void AudioEngineLoadWithBasicTransitions(char * file, AE_HANDLE & outHandle)
    {
        AudioEngineLoad(file, true, true, outHandle);

        FadeInEffect * fadeIn = new FadeInEffect;
        FadeOutEffect * fadeOut = new FadeOutEffect;
        
        AudioEngineAddEffect(outHandle, fadeIn);
        AudioEngineAddEffect(outHandle, fadeOut);
    }

    /// <summary>
    /// Plays an audio object.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    void AudioEnginePlay(AE_HANDLE handle)
    {
        if (l_playing) AudioEngineStop();

        Audio * audio = (Audio*) handle;

        if (audio->hasTransitions)
            ApplyPreEffects(audio);

        MMRESULT result = waveOutOpen(&l_waveOut, WAVE_MAPPER, &l_waveEx, NULL, NULL, CALLBACK_NULL);
        AssertMMResult(result, "WaveOutOpen failed");

        //WAVEHDR * hdr = (WAVEHDR *) audio->hdr;

        //WAVEHDR tempHdr;
        PrepareWaveHDR(audio->hdr, audio->size, audio->data);


        result = waveOutPrepareHeader(l_waveOut, &audio->hdr, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutPrepareHeader failed");

        result = waveOutWrite(l_waveOut, &audio->hdr, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        l_playing = audio;
        audio->isPlaying = true;
    }

    /// <summary>
    /// Plays an audio file, with transition effects.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    /// <param name="timeToStart">The time to start.</param>
    void AudioEnginePlayTime(AE_HANDLE handle, ulong timeToStart)
    {
        if (l_playing) AudioEngineStop();

        Audio * audio = (Audio *) handle;

        ApplyPreEffects(audio);

        ulong duration = MillisecondsToBytes(audio, timeToStart);

        memcpy_s(audio->dataCopyTemp, duration, audio->dataCopy, duration);

        short* data = reinterpret_cast<short *>(audio->dataCopyTemp);
        ApplyStartEffects(audio, data, duration / 2, l_waveEx.nAvgBytesPerSec / 2);

        MMRESULT result = waveOutOpen(&l_waveOut, WAVE_MAPPER, &l_waveEx, NULL, NULL, CALLBACK_NULL);
        AssertMMResult(result, "WaveOutOpen failed");

        PrepareWaveHDR(l_hdrNoLoop, duration, audio->dataCopyTemp);

        char * startData = audio->dataCopyStart;
        startData += duration;

        PrepareWaveHDR(l_hdrStart, audio->size - duration, startData);
        PrepareWaveHDR(l_hdrEnd, duration, audio->dataCopyStart);

        l_hdrStart.dwFlags |= WHDR_BEGINLOOP;
        l_hdrEnd.dwFlags |= WHDR_ENDLOOP;

        result = waveOutWrite(l_waveOut, &l_hdrNoLoop, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        result = waveOutWrite(l_waveOut, &l_hdrStart, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        result = waveOutWrite(l_waveOut, &l_hdrEnd, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        l_playing = audio;
        audio->isPlaying = true;
    }

    /// <summary>
    /// Plays an audio file at the specified position, with transition effects.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="position">The position.</param>
    /// <param name="timeToStart">The time to start.</param>
    void AudioEnginePlayPos(AE_HANDLE handle, ulong position, ulong timeToStart)
    {
        if (l_playing) AudioEngineStop();

        Audio * audio = (Audio *)handle;

        ulong pos = MillisecondsToBytes(audio, position);
        ulong duration = 0;
        char * clipData = nullptr;
        char * startData = nullptr;

        if (audio->hasTransitions) {

            ApplyPreEffects(audio);
            duration = MillisecondsToBytes(audio, timeToStart);


            audio->dataCopy = audio->dataCopyStart + pos;
            audio->dataCopyTemp = audio->dataCopyTempStart + pos;

            memcpy_s(audio->dataCopyTemp, duration, audio->dataCopy, duration);

            clipData = audio->dataCopyTemp;

            ApplyStartEffects(audio, reinterpret_cast<short*>(clipData), duration / 2, l_waveEx.nAvgBytesPerSec / 2);



            startData = audio->dataCopyStart;
            startData += duration + pos;

            // audio->position += duration;
            // audio->position %= audio->size;

        }
        else
        {

        }

        l_hdrStart.dwFlags |= WHDR_BEGINLOOP;
        l_hdrEnd.dwFlags |= WHDR_ENDLOOP;



        //short* data = reinterpret_cast<short *>(audio->dataCopyTemp);
        //ApplyStartEffects(audio, data, duration / 2, audio->waveEx.nAvgBytesPerSec / 2);

        MMRESULT result = waveOutOpen(&l_waveOut, WAVE_MAPPER, &l_waveEx, NULL, NULL, CALLBACK_NULL);
        AssertMMResult(result, "WaveOutOpen failed");

        
        if (audio->hasTransitions)
        {
            PrepareWaveHDR(l_hdrNoLoop, duration, clipData);
            PrepareWaveHDR(l_hdrStart, audio->size - duration - pos, startData);
            PrepareWaveHDR(l_hdrEnd, duration + pos, audio->dataCopy);
        }
        else
        {
            PrepareWaveHDR(l_hdrStart, audio->size - pos, audio->data + pos);
            PrepareWaveHDR(l_hdrEnd, pos, audio->data);
        }



        result = waveOutWrite(l_waveOut, &l_hdrNoLoop, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        result = waveOutWrite(l_waveOut, &l_hdrStart, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");

        result = waveOutWrite(l_waveOut, &l_hdrEnd, sizeof(WAVEHDR));
        AssertMMResult(result, "waveOutWrite failed");
        //l_hdrNoLoop = WAVEHDR{};
        //l_hdrNoLoop.dwBufferLength = duration;
        //l_hdrNoLoop.dwFlags = 0;
        //l_hdrNoLoop.lpData = clipData;

        //result = waveOutPrepareHeader(l_waveOut, &l_hdrNoLoop, sizeof(WAVEHDR));
        //AssertMMResult(result, "waveOutPrepareHeader failed");



        l_playing = audio;
        audio->isPlaying = true;
    }

    void AudioEngineAddEffect(AE_HANDLE handle, AudioEffect * effect)
    {
        Audio * audio = (Audio *)handle;
        audio->effects.push_back(effect);
    }

    /// <summary>
    /// Retrieve the audio position of an audio object.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    /// <returns>The audio object position</returns>
    ulong AudioEngineGetPosition(AE_HANDLE handle)
    {
        Audio * audio = (Audio *)handle;
        MMTIME time;
        time.wType = TIME_BYTES;
        MMRESULT result = waveOutGetPosition(l_waveOut, &time, sizeof(MMTIME));
        AssertMMResult(result, "waveOutGetPosition failed");

        ulong bytes = (time.u.cb + audio->position) % audio->size;
        return BytesToMilliseconds(audio, bytes);

    }

    ulong AudioEngineGetPositionBytes(AE_HANDLE handle)
    {
        Audio * audio = (Audio *)handle;
        MMTIME time;
        time.wType = TIME_BYTES;
        MMRESULT result = waveOutGetPosition(l_waveOut, &time, sizeof(MMTIME));
        AssertMMResult(result, "waveOutGetPosition failed");

        ulong bytes = (time.u.cb + audio->position) % audio->size;
        return bytes;

    }


    void AudioEngineSetPosition(AE_HANDLE handle, ulong position)
    {
        Audio * audio = (Audio *) handle;

        if (l_playing == audio)
        {
            AudioEngineStop();
            l_playing = audio;
        }

        ulong byteOffset = MillisecondsToBytes(audio, position) % audio->size;

        WAVEHDR hdr = audio->hdr;

        hdr.lpData += byteOffset;
        hdr.dwBufferLength -= byteOffset;

        waveOutWrite(l_waveOut, &hdr, sizeof(WAVEHDR));
        l_playing->position = BytesToMilliseconds(l_playing, byteOffset);
    }

    /// <summary>
    /// Retrieves the size of the audio buffer loaded into an audio object.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    /// <returns>The size of the audio buffer</returns>
    int AudioEngineAudioDataSize(AE_HANDLE handle) {
        return ((Audio*)handle)->size;
    }

    /// <summary>
    /// Retrieves the audio buffer loaded into an audio object.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    /// <returns>The audio buffer</returns>
    char * AudioEngineRetrieveData(AE_HANDLE handle)
    {
        return ((Audio*)handle)->data;
    }

    /// <summary>
    /// Stops a currently playing audio object, with transition effects
    /// </summary>
    /// <param name="timeToStop">The time to stop.</param>
    void AudioEngineStopTime(ulong timeToStop)
    {
        if (l_playing)
        {

            short * data = reinterpret_cast<short *>(l_playing->dataCopyStart);
            int size = l_playing->size;

            ulong pos = AudioEngineGetPositionBytes((void*) l_playing);
            ulong offset = pos;
            ulong bytesToStop = MillisecondsToBytes(l_playing, timeToStop);

            data += offset;
            int offsetSize = size - offset;
            offsetSize = (int) bytesToStop < offsetSize ? bytesToStop : offsetSize;
            size = offsetSize;

            memcpy_s(l_playing->dataCopyTemp, size, data, size);
            ApplyEndEffects(l_playing, reinterpret_cast<short *>(l_playing->dataCopyTemp), size, l_waveEx.nAvgBytesPerSec / 2);
            l_playing->dataCopy = reinterpret_cast<char *>(data);
            waveOutReset(l_waveOut);


            PrepareWaveHDR(l_hdrNoLoop, size, l_playing->dataCopyTemp);

            MMRESULT result = waveOutWrite(l_waveOut, &l_hdrNoLoop, sizeof(WAVEHDR));
            AssertMMResult(result, "waveOutWrite failed");

            l_playing->position = pos;
            l_playing->isPlaying = false;
        }

        l_playing = nullptr;
    }

    /// <summary>
    /// Stops a currently playing audio object.
    /// </summary>
    void AudioEngineStop()
    {
        waveOutReset(l_waveOut);
        waveOutClose(l_waveOut);

        UnPrepareWaveHDR(l_hdrStart);
        UnPrepareWaveHDR(l_hdrEnd);
        UnPrepareWaveHDR(l_hdrNoLoop);


        l_playing = nullptr;
    }

    /// <summary>
    /// Disposes an audio object.
    /// </summary>
    /// <param name="handle">The audio handle.</param>
    void AudioEngineUnload(AE_HANDLE handle)
    {
        Audio * audio = (Audio*) handle;
        if (audio->dataCopyStart)
            free(audio->dataCopyStart);

        //if (audio->dataCopyStart)
        //    free(audio->dataCopyStart);
        if (audio->dataCopyTempStart)
            free(audio->dataCopyTempStart);
        free(audio->data);
        free(audio);


        
        // if (audio->hdr)
        //     free(audio->hdr);
        // if (audio->wavOut)
        //     free(audio->wavOut);
        while (!audio->effects.empty())
        {
            AudioEffect * e = audio->effects.back();
            audio->effects.pop_back();
            if (e)
                delete e;
        }
        // return void();
    }

    /// <summary>
    /// Shutdowns the audio engine.
    /// </summary>
    /// <returns></returns>
    bool AudioEngineShutdown()
    {
        if (l_isInitialized)
        {
            AudioEngineStop();
            return true;
        }

        return true;
    }

    ulong SecondsToBytes(AE_HANDLE handle, float seconds)
    {
        return MillisecondsToBytes((Audio *) handle, seconds * 1000);
    }

    float BytesToSeconds(AE_HANDLE handle, ulong bytes)
    {
        return BytesToMilliseconds((Audio *) handle, bytes) / 1000.0f;
    }

}

ulong MillisecondsToBytes(Audio* audio, ulong time)
{
    return time * (l_waveEx.nAvgBytesPerSec / 1000);
}

ulong BytesToMilliseconds(Audio* audio, ulong bytes)
{
    return bytes / (l_waveEx.nAvgBytesPerSec / 1000);
}

void ApplyPreEffects(Audio * audio)
{
     memcpy_s(audio->dataCopyStart, audio->size, audio->data, audio->size);
     audio->dataCopy = audio->dataCopyStart;
     
     ulong shortsPerSecond = l_waveEx.nAvgBytesPerSec / 2;
     for (AudioEffect * e : audio->effects)
     {
         e->PreProcess(reinterpret_cast<short *>(audio->dataCopy), audio->size / 2, shortsPerSecond);
     }
}

void ApplyStartEffects(Audio * audio, short * data, int size, ulong shortsPerSecond)
{
    for (AudioEffect * e : audio->effects)
    {
        audio->dataCopy = reinterpret_cast<char *>(e->StartProcess(reinterpret_cast<short *>(data), size / 2, shortsPerSecond));
    }
}

void ApplyEndEffects(Audio * audio, short * data, int size, ulong shortsPerSecond)
{
     for (AudioEffect * e : audio->effects)
     {
         e->EndProcess(reinterpret_cast<short *>(data), size / 2, shortsPerSecond);
     }
}

void PrepareWaveHDR(WAVEHDR & hdr, ulong length, char * data)
{
    hdr = WAVEHDR{};
    hdr.dwBufferLength = length;
    hdr.lpData = data;

    MMRESULT result = waveOutPrepareHeader(l_waveOut, &hdr, sizeof(WAVEHDR));
    AssertMMResult(result, "waveOutPrepareHeader failed");
}

void UnPrepareWaveHDR(WAVEHDR & hdr)
{
    if (hdr.lpData)
    {
        waveOutUnprepareHeader(l_waveOut, &hdr, sizeof(WAVEHDR));
        hdr.lpData = nullptr;
    }
}



void AssertMMResult(MMRESULT result, const char * msg) {
    if (result == MMSYSERR_NOERROR) return;

    WCHAR buffer[MAXERRORLENGTH];
    waveOutGetErrorText(result, buffer, MAXERRORLENGTH);

    
    // const int size = MAXERRORLENGTH + 250;
    // char fullMsg[size];
    // sprintf_s(fullMsg, size, "%s [%s]\n", msg, buffer);
    a_printf("%s [%s]\n", msg, buffer);
    assert(false && "Something broke!");
}

Audio * ReadWaveData(char * buffer) {
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
    output->size = data->size;
    output->size *= 48000 / header->sampleRate;

    output->data = new char[output->size];
    memcpy_s(output->data, output->size, buffer, data->size);

    l_waveEx.wFormatTag = WAVE_FORMAT_PCM;
    l_waveEx.nChannels = header->channels;
    l_waveEx.nSamplesPerSec = header->sampleRate;

    // a_printf("Sample Rate: %d\n", audio->waveEx.nSamplesPerSec);
    // a_printf("Block Align: %d\n", header->blockAlign);
    // a_printf("bitsPerSample: %d\n", header->bitsPerSample);
    // a_printf("Size: %d\n", output->audio->size);

    l_waveEx.nAvgBytesPerSec = header->sampleRate * header->blockAlign;
    l_waveEx.nBlockAlign = header->blockAlign;
    l_waveEx.wBitsPerSample = header->bitsPerSample;

    // ResampleAudio(output, audio->waveEx.nSamplesPerSec);

    // audio->waveEx.nSamplesPerSec = 48000;

    return output;
}