using System;
using UnityEngine;


public class AudioInvoker : MonoBehaviour, IPaused
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Range(0, 1)] private float _volume = 0.5f;
    [SerializeField] private float _audibilityDistance = 15f;

    private bool _isCanPlay = true;

    public Action<float, float, Vector3> OnPlay;


    public void SetAudioSource(AudioSource audioSource, float volume, float audibilityDistance)
    {
        _audioSource = audioSource;
        _volume = volume;
        _audibilityDistance = audibilityDistance;
    }


    public void PlayAudio()
    {
        if (_isCanPlay == false)
            return;

        _audioSource.Play();
        OnPlay?.Invoke(_volume, _audibilityDistance, _audioSource.transform.position);
    }


    public void Pause()
    {
        _isCanPlay = false;
        _audioSource.Stop();
    }


    public void Resume()
    {
        _isCanPlay = true;
    }
}