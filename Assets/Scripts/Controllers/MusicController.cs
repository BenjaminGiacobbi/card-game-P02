using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : GenericSingleton<MusicController>
{
    private float _defaultVolume = 1;
    private AudioSource _musicPlayer = null;

    public override void Awake()
    {
        base.Awake();
        _musicPlayer = GetComponent<AudioSource>();
        _musicPlayer.loop = true;
        _defaultVolume = _musicPlayer.volume;
    }

    public void PlayMusic(AudioClip music, float volume)
    {
        /*
        if(_musicPlayer.isPlaying)
        {
            LeanTween.value(_musicPlayer.volume, 0, 0.5f).setOnUpdate(
                (float val) => { _musicPlayer.volume = val; }).setOnComplete(
                    () => { StartNewSong(music, volume); });
        }
        else
        {
            StartNewSong(music, volume);
        }
        */
    }

    private void StartNewSong(AudioClip music, float volume)
    {
        /*
        _musicPlayer.volume = volume;
        _musicPlayer.clip = music;
        _musicPlayer.Play();
        */
    }

    public void StopMusic()
    {
        /*
        if(_musicPlayer.isPlaying)
        {
            LeanTween.value(_musicPlayer.volume, 0, 0.5f).setOnUpdate(
                (float val) => { _musicPlayer.volume = val; });
        }
        */
    }
}
