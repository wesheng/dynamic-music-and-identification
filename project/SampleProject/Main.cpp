#include <windows.h>
#include <cstdio>
////#include "AudioEngine.h"
//#include "AudioEngine.h"
//#include <chrono>
//#include "FadeInEffect.h"
//#include "FadeOutEffect.h"
//#include <Windows.h>
////#include <winuser.h>
//#include <cstdio>
#include "AudioEngine2.h"
#include <iostream>

HANDLE handle;

void ShowCursor(bool show = true)
{
	CONSOLE_CURSOR_INFO info;
	GetConsoleCursorInfo(handle, &info);
	info.bVisible = show;
	SetConsoleCursorInfo(handle, &info);
}

void GoToPos(int x, int y)
{
	COORD pos = {x, y};
	SetConsoleCursorPosition(handle,pos);
}

int main()
{
	//BOOL bRtn;
	//LPVOID lpRes;
	//HANDLE hResInfo, hRes;

	//hResInfo = FindResource(NULL, TEXT("..\\..\\data\\ribi-azure.wav"), "WAVE");

	//if (hResInfo == NULL)
	//{
	//	printf("Oh no!\n");
	//	return -1;
	//}

	//hRes = LoadResource(NULL, static_cast<HRSRC>(hResInfo));
	//if (hRes == NULL)
	//{
	//	printf("Oh no! 2\n");
	//	return -1;
	//}

	//lpRes = LockResource(hRes);
	//if (lpRes != NULL)
	//{
	//	bRtn = sndPlaySound(static_cast<LPSTR>(lpRes), SND_MEMORY | SND_SYNC | SND_NODEFAULT);
	//	UnlockResource(hRes);
	//}

	//FreeResource(hRes);



	//PlaySound(TEXT("..\\..\\data\\ribi-azure.wav"), NULL, SND_FILENAME);
	//MAKEINTRESOURCE()
	
	//handle = GetStdHandle(STD_OUTPUT_HANDLE);
	//HWND console = GetConsoleWindow();
	//ShowCursor(false);

	//ws::AudioEngine::Initialize();

	//ws::Audio * a = ws::AudioEngine::Load("..\\..\\data\\battle.wav", true);
	//ws::Audio * a2 = ws::AudioEngine::Load("..\\..\\data\\song.wav", true);

	//a->AddEffect(new ws::FadeInEffect());
	//a->AddEffect(new ws::FadeOutEffect());

	////Audio * a3 = AudioEngine::Mix(a, a2);

	////AudioEngine::ApplyPreEffects(a);


	////AudioEngine::SetPlaybackSpeed(2);
	////AudioEngine::SetVolume(a3, 1);
	//// AudioEngine::SetVolume(a2, 0.2f);
	////float v = 0;
	////bool isGoingLeft = false;

	//bool isPPressed = false;
	//bool wasPPressed = false;

	//bool wasOPressed = false;

	//ws::AudioTime timeToStart{ 2 };
	//ws::AudioTime timeToStop {2};
	//ws::AudioTime aPosition;
	//ws::AudioTime a2Position;

	//ws::AudioEngine::Play(a, timeToStart);


	//while (!(GetAsyncKeyState('K') && console == GetForegroundWindow()))
	//{

	//	if (GetAsyncKeyState('P'))
	//	{
	//		isPPressed = true;
	//		if (!wasPPressed)
	//		{
	//			if (a->IsPlaying())
	//			{
	//				printf("Stopping...\n");
	//				aPosition = ws::AudioEngine::GetPosition(a) + timeToStop;
	//				ws::AudioEngine::Stop(timeToStop);
	//			} else
	//			{
	//				printf("Playing...\n");
	//				ws::AudioEngine::Play(a, aPosition, timeToStart);

	//			}
	//		}
	//		wasPPressed = true;
	//	}
	//	else if (GetAsyncKeyState('O'))
	//	{
	//		if (!wasOPressed)
	//		{
	//			if (a2->IsPlaying())
	//			{
	//				a2Position = ws::AudioEngine::GetPosition(a2) + timeToStop;
	//				ws::AudioEngine::Stop(timeToStop);
	//			}
	//			else
	//			{
	//				ws::AudioEngine::Play(a2, a2Position, timeToStart);
	//			}
	//		}
	//		wasOPressed = true;
	//	}
	//	else
	//	{
	//		isPPressed = false;
	//		wasPPressed = false;
	//		wasOPressed = false;
	//	}
	//	//AudioTime t = AudioEngine::GetPosition(a3);
	//	//GoToPos(0, 3);
	//	//printf("Time: %03lu:%02lu:%02lu:%03lu - %030lu", t.m_hours, t.m_minutes, t.m_seconds, t.m_milliseconds, t.Bytes);
	//	//GoToPos(0, 4);
	//	//printf("Time: %03lu:%04lu:%06lu:%08lu", t.m_totalHours, t.m_totalMinutes, t.m_totalSeconds, t.m_totalMilliseconds);

	//	//if (GetAsyncKeyState('P'))
	//	//{
	//	//	if (!wasPPressed)
	//	//	{
	//	//		isPlayingSecond = !isPlayingSecond;
	//	//		if (isPlayingSecond)
	//	//		{
	//	//			aTime = AudioEngine::GetPosition(a);
	//	//		}
	//	//		else
	//	//		{
	//	//			a2Time = AudioEngine::GetPosition(a2);
	//	//		}
	//	//		AudioEngine::Stop();
	//	//		AudioEngine::Play(isPlayingSecond ? a2 : a);
	//	//		if (isPlayingSecond)
	//	//			AudioEngine::SetPosition(a2, a2Time);
	//	//		else
	//	//			AudioEngine::SetPosition(a, aTime);
	//	//	}
	//	//	wasPPressed = true;

	//	//} else
	//	//{
	//	//	wasPPressed = false;
	//	//}
	//}

 //   ws::AudioEngine::Stop();

	//ws::AudioEngine::Unload(a);
	//ws::AudioEngine::Unload(a2);
	//ws::AudioEngine::Shutdown();

    AudioEngineInitialize();

    AE_HANDLE a1 = AudioEngineLoad("C:\\Users\\wsheng\\Documents\\Neumont\\Q9\\capstone\\repo\\unity\\project\\Assets\\Audio\\VN1.7.wav", true, false);
    AudioEnginePlay(a1);

    int something;
    std::cin >> something;

    AudioEngineStop();
    AudioEngineUnload(a1);

    AudioEngineShutdown();

    printf("done!\n");
}
