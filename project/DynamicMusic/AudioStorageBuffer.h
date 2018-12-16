#pragma once

#define MAX_AUDIO_STORAGE_SIZE 1 << 26


class AudioStorageBuffer
{
public:
	AudioStorageBuffer(unsigned long maxStorageSize = MAX_AUDIO_STORAGE_SIZE)
	{
		m_data = new char[maxStorageSize];
	}

	~AudioStorageBuffer()
	{
		delete[] m_data;
	}

	void * GetBuffer() const
	{
		return m_data;
	}

private:
	char * m_data;

};