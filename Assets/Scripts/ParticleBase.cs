using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class ParticleBase : MonoBehaviour
{
    [SerializeField] ParticleSystem _objectParticles = null;
    [SerializeField] AudioSource _objectAudio = null;

    // caching
    private void Awake()
    {
        _objectParticles = GetComponent<ParticleSystem>();
        _objectAudio = GetComponent<AudioSource>();
    }

    // method to call particles across whatever's using it
    public void PlayComponents()
    {
        Debug.Log("Playing 2");
        Debug.Log(_objectParticles);
        Debug.Log(_objectAudio);
        if (_objectParticles != null)
        {
            _objectParticles.Play();
            Debug.Log("Playing 3");
        }
            
        if(_objectAudio != null && _objectAudio.clip != null)
        {
            AudioHelper.PlayClip2D(_objectAudio.clip, 0.3f);
            Debug.Log("Playing 4");
        }
            
    }
}
