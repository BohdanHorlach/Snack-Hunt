using System;
using System.Collections.Generic;
using UnityEngine;


public class FootstepsInvoker : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private PlayerState _state;
    [SerializeField] private AnimateHandler _animateHandler;
    [SerializeField] private AudioInvoker _audioInvoker;
    [SerializeField] private AudioInvokerInfo _runningAudio;
    [SerializeField] private AudioInvokerInfo _sprintAudio;
    [SerializeField] private AudioInvokerInfo _silentWalkAudio;

    private List<KeyValuePair<Func<bool>, AudioInvokerInfo>> _footstepSources;


    private void Awake()
    {
        _footstepSources = new List<KeyValuePair<Func<bool>, AudioInvokerInfo>>()
        {
            new (() => _player.IsSilentWalking, _silentWalkAudio ),
            new (() => _player.IsSptinting, _sprintAudio),
            new (() => !_player.IsSilentWalking && !_player.IsSptinting, _runningAudio )
        };
    }


    private void OnEnable()
    {
        _animateHandler.OnMakeStep += PlayFootstep;
    }


    private void OnDisable()
    {
        _animateHandler.OnMakeStep -= PlayFootstep;
    }


    private AudioInvokerInfo GetFootstepsSource()
    {
        foreach (var footstepSources in _footstepSources)
        {
            if (footstepSources.Key.Invoke())
                return footstepSources.Value;
        }

        return _footstepSources[_footstepSources.Count - 1].Value;
    }


    private void PlayFootstep()
    {
        if (_state.IsOnGround == false)
            return;

        AudioInvokerInfo audioSource = GetFootstepsSource();

        _audioInvoker.SetAudioSource(
                audioSource.Source, 
                audioSource.Volume, 
                audioSource.AudibilityDistance);

        _audioInvoker.PlayAudio();
    }
}