#pragma once

struct RIFF_HEADER
{
	char id[4];
	int size;
	char format[4];
};

struct WAVE_HEADER
{
	RIFF_HEADER riff;
	char id[4];
	int size;
	short format;
	short channels;
	int sampleRate;
	int byteRate;
	short blockAlign;
	short bitsPerSample;
};

struct WAVE_DATA
{
	char id[4];
	int size;
};