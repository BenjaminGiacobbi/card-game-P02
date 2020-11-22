using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class ParticleBase : MonoBehaviour
{
    [SerializeField] float _audioVolume = 0.5f;
    ParticleSystem _objectParticles = null;
    AudioSource _objectAudio = null;

    // caching
    private void Awake()
    {
        _objectParticles = GetComponent<ParticleSystem>();
        _objectAudio = GetComponent<AudioSource>();
    }

    // method to call particles across whatever's using it
    public void PlayComponents()
    {
        if (_objectParticles != null)
            _objectParticles.Play();
            
        if(_objectAudio != null && _objectAudio.clip != null)
            AudioHelper.PlayClip2D(_objectAudio.clip, _audioVolume);
    }

    public void ChangeColor(Color color)
    {
        var main = _objectParticles.main;
        main.startColor = color;
    }
}
