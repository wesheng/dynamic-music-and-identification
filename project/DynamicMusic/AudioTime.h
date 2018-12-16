#pragma once
#include "Audio.h"
#include "AudioMacro.h"
#include <string>
#include <sstream>

namespace ws {
	class AUDIO_ENGINE AudioTime
	{
		unsigned long m_totalHours = 0;
		unsigned long m_totalMinutes = 0;
		unsigned long m_totalSeconds = 0;
		unsigned long m_totalMilliseconds = 0;

		unsigned long m_hours = 0;
		unsigned long m_minutes = 0;
		unsigned long m_seconds = 0;
		unsigned long m_milliseconds = 0;

	public:
		AudioTime() = default;

		AudioTime(float seconds)
		{
			SetTotalMilliseconds(static_cast<ulong>(seconds * 1000.0f));
		}

		unsigned long GetTotalHours() const
		{
			return m_totalHours;
		}

		void SetTotalHours(unsigned long total_hours)
		{
			SetTotalMinutes(total_hours * 60);
			//m_totalHours = total_hours;

			//m_totalMinutes += m_totalHours * 60;

			//m_hours = m_totalHours;
		}

		unsigned long GetTotalMinutes() const
		{
			return m_totalMinutes;
		}

		void SetTotalMinutes(unsigned long total_minutes)
		{
			SetTotalSeconds(total_minutes * 60);
			//m_totalMinutes = total_minutes;

			//SetTotalHours(m_totalMinutes / 60);
			//m_minutes = m_totalMinutes % 60;
		}

		unsigned long GetTotalSeconds() const
		{
			return m_totalSeconds;
		}

		void SetTotalSeconds(unsigned long total_seconds)
		{
			SetTotalMilliseconds(total_seconds * 1000);
			//m_totalSeconds = total_seconds;

			//SetTotalMinutes(m_totalSeconds / 60);
			//m_seconds = m_totalSeconds % 60;
		}

		unsigned long GetTotalMilliseconds() const
		{
			return m_totalMilliseconds;
		}

		void SetTotalMilliseconds(unsigned long total_milliseconds)
		{
			m_totalMilliseconds = total_milliseconds;

			m_totalSeconds = (m_totalMilliseconds / 1000);
			m_totalMinutes = m_totalMinutes / 60;
			m_totalHours = m_totalMinutes / 60;

			m_milliseconds = m_totalMilliseconds % 1000;
			m_seconds = m_totalSeconds % 60;
			m_minutes = m_totalMinutes % 60;
			m_hours = m_totalHours;
		}

		unsigned long GetHours() const
		{
			return m_hours;
		}

		unsigned long GetMinutes() const
		{
			return m_minutes;
		}

		unsigned long GetSeconds() const
		{
			return m_seconds;
		}

		unsigned long GetMilliseconds() const
		{
			return m_milliseconds;
		}

		operator std::string() const
		{
			std::ostringstream stream{};
			stream << m_hours << "h" << m_minutes << "m" << m_seconds << "s" << m_milliseconds << "mm";
			return stream.str();
		}

		AudioTime operator+(AudioTime & other) const {
			return AudioTime{ (m_totalMilliseconds + other.m_totalMilliseconds) / 1000.0f };
		}

		AudioTime operator-(AudioTime & other) const {
			return AudioTime{ (m_totalMilliseconds - other.m_totalMilliseconds) / 1000.0f };
		}
	};
}