#pragma once
#include "AudioMacro.h"

    struct AudioEffect {

    public:

        AudioEffect() {};
        ~AudioEffect() {};

        /**
        * \brief Preprocesses the passed audio data.
        * \brief Called by AudioEngine::Play().
        * \brief This method will only be called once by the AudioEngine.
        * \param data The entire raw audio data
        * \param size The size of the data
        * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
        */
        virtual short * PreProcess(short * data, int size, unsigned long shortsPerSecond) { return data; }

        /**
        * \brief Processes the passed audio data.
        * \brief Called by AudioEngine::Play() and AudioEngine::Swap().
        * \brief Used when the AudioEngine plays a song, starting from a specific position.
        * \param data The raw audio data, starting from the current song position
        * \param size The size of the data; from the current song position to the song position where the full song takes over
        * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
        */
        virtual short * StartProcess(short * data, int size, unsigned long shortsPerSecond) { return data; }

        /**
        * \brief Processes the passed audio data.
        * \brief Called by AudioEngine::Swap().
        * \param data The raw audio data, starting from current song position
        * \param size The size of the data; from the current song position to the song position where the song is stopped
        * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
        */
        virtual short * EndProcess(short * data, int size, unsigned long shortsPerSecond) { return data; }

        /**
        * \brief Clamps the passed short to be within audio range.
        * \param in The short
        * \return The clamped short, within the range -0x7fff and 0x7fff
        */
        static short Clamp(short in)
        {
            if (in > 0x7fff)
                return 0x7fff;
            if (in < -0x7fff)
                return -0x7fff;
            return in;
        }
    private:
        bool m_hasPreprocessed = false;
    };
