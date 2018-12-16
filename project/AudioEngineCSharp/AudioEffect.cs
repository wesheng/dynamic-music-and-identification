using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AudioEngineCSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe class AudioEffect
    {
        /**
		 * \brief Preprocesses the passed audio data.
		 * \brief Called by AudioEngine::Play().
		 * \brief This method will only be called once by the AudioEngine.
		 * \param data The entire raw audio data
		 * \param size The size of the data
		 * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
		 */
        public virtual short* PreProcess(short* data, int size, ulong shortsPerSecond) { return data; }

        /**
		 * \brief Processes the passed audio data.
		 * \brief Called by AudioEngine::Play() and AudioEngine::Swap().
		 * \brief Used when the AudioEngine plays a song, starting from a specific position.
		 * \param data The raw audio data, starting from the current song position
		 * \param size The size of the data; from the current song position to the song position where the full song takes over
		 * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
		 */
        public virtual short* StartProcess(short* data, int size, ulong shortsPerSecond) { return data; }

        /**
		 * \brief Processes the passed audio data.
		 * \brief Called by AudioEngine::Swap().
		 * \param data The raw audio data, starting from current song position
		 * \param size The size of the data; from the current song position to the song position where the song is stopped
		 * \param shortsPerSecond Number of shorts per second (equivalent to bytes per second / 2)
		 */
        public virtual short* EndProcess(short* data, int size, ulong shortsPerSecond) { return data; }

        /**
		 * \brief Clamps the passed short to be within audio range.
		 * \param input The short
		 * \return The clamped short, within the range -0x7fff and 0x7fff
		 */
        protected static short Clamp(short input)
        {
            if (input > 0x7fff)
				return 0x7fff;
            if (input < -0x7fff)
				return -0x7fff;
            return input;
        }

        private bool m_hasPreprocessed = false;

    }
}
