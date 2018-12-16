//
//#include <Mmdeviceapi.h>
//#include <Audioclient.h>
//#include <functional>
//
//
//int main()
//{
//	IMMDeviceEnumerator * enumerator;
//
//	CoCreateInstance(CLSID_MMDeviceEnumerator, NULL, CLSCTX_ALL, IID_IMMDeviceEnumerator, (void**)&enumerator);
//
//	//IMMDeviceCollection * collection;
//
//	//enumerato
//
//	IMMDevice * device;
//
//	enumerator->GetDefaultAudioEndpoint(eRender, eMultimedia, &device);
//
//	//IMMDeviceCollection::Item(0, &device);
//
//	IAudioClient * client;
//	
//	HRESULT result = device->Activate(IID_IAudioClient, CLSCTX_ALL, NULL, (void **) &client);
//	if (result == S_OK)
//	{
//		WAVEFORMATEX * pwfx = NULL;
//		result = client->Initialize(AUDCLNT_SHAREMODE_SHARED, 0, 10000000, 10000000, pwfx, NULL);
//		
//		
//		CoTaskMemFree(pwfx);
//	}
//
//
//}


#include <windows.h>
#include <cstdio>
#include "AudioEngine.h"

int main()
{
	AudioEngine::Initialize();
	Audio * a = AudioEngine::Load("..\\..\\data\\ribi-azure.wav");
	a->Play();
	while (!( GetAsyncKeyState('K') && GetConsoleWindow() == GetForegroundWindow()))
	{
		
	}
	AudioEngine::Unload(a);
}
