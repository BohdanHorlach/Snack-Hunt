using System;
using UnityEngine;


public class AudioInvoker : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField, Range(0, 1)] private float _audibility;

    public Action<float, Vector3> OnPlay;


    public void PlayAudio()
    {
        _audioSource.Play();
        OnPlay?.Invoke(_audibility, _audioSource.transform.position);
    }
}