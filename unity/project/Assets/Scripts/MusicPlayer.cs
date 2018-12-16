using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using Assets.Scripts.Classes;
using System;

public class MusicPlayer : MonoBehaviour {

    [SerializeField] private string[] _songFile;

    [Serializable]
    struct SongInfo
    {
        public string file;
        public float offset;
        public float bpm;
    }

    [SerializeField] private SongInfo[] _songInfos;

    // AudioEngineCSharp.AudioEngine.Audio[] _audio;
    Song[] _songs;
    Song _playing;
    int _audioIndex;

    //struct Song
    //{
    //    public AudioEngineCSharp.AudioEngine.Audio audio;
    //    public ulong position;
    //}

    // Use this for initialization
    void Start()
    {
        AudioEngineCSharp.AudioEngine.Initialize();
        _songs = new Song[_songInfos.Length];
        for (int i = 0; i < _songs.Length; i++)
        {
            _songs[i] = new Song(_songInfos[i].file, _songInfos[i].offset, _songInfos[i].bpm);
        }

        _audioIndex = 0;
        // PlayNext();
        PlayIndex(0);
    }

    //private void PlayNext()
    //{
    //    if (_playing != null)
    //        AudioEngineCSharp.AudioEngine.Stop(_playing);
    //    AudioEngineCSharp.AudioEngine.Play(_audio[_audioIndex]);

    //    _playing = _audio[_audioIndex++];
    //    if (_audioIndex >= _audio.Length)
    //        _audioIndex = 0;
    //}

    public void PlayIndex(int index)
    {
        _audioIndex = index % _songs.Length;

        StartCoroutine(SwitchSong(_songs[_audioIndex++], 1, 1));

        if (_audioIndex >= _songs.Length)
            _audioIndex = 0;
    }

    private IEnumerator SwitchSong(Song toPlay, float timeToStop, float timeToStart)
    {
        float currentTime = 0;
        if (_playing != null)
        {
            _playing.Stop(timeToStop);
            while (currentTime < timeToStop)
            {
                currentTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        _playing = toPlay;
        toPlay.Play(true, timeToStart);
    }

    private void OnDestroy()
    {
        if (_playing != null)
            _playing.Stop(0);
        // AudioEngineCSharp.AudioEngine.Stop(_playing);
        for (int i = 0; i < _songs.Length; i++)
        {
            _songs[i].Dispose();
        }
        AudioEngineCSharp.AudioEngine.Shutdown();
    }

    // Update is called once per frame
    void Update () {
		//if (Input.GetButtonDown("Fire2"))
  //      {
  //          PlayNext();
  //      }
        //if (Input.GetButtonDown("Jump"))
        //{
        //    if (_playing != null)
        //    {
        //        AudioEngineCSharp.AudioEngine.Stop(_playing);
        //        _playing = null;
        //    }
        //}
	}
}
