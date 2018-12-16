using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SongRegionPlayer : MonoBehaviour {

    // [SerializeField] private Bounds _bounds;
    [SerializeField] private bool _usFileName = false;
    [SerializeField] private int _indexToPlay;

    [SerializeField] private MusicPlayer _player;

    [Header("Testing")]
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audio;
    [SerializeField] private string[] _objectTags;

    private bool _playing;
    private BoxCollider _collider;
    

	// Use this for initialization
	void Start () {
        _collider = GetComponent<BoxCollider>();
        _playing = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in _objectTags)
        {
            if (other.CompareTag(tag))
            {
                if (!_playing)
                {
                    _player.PlayIndex(_indexToPlay);
                    _playing = true;
                }
                break;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var tag in _objectTags)
        {
            if (other.CompareTag(tag))
            {
                if (_playing)
                {
                    _playing = false;
                }
                break;
            }
        }

    }



    //private void OnDrawGizmosSelected()
    //{
    //    //Gizmos.color = GetComponent<Renderer>().material.color;
    //    //Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    //}
}
