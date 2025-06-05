using System;
using UnityEngine;


public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private Health _player;
    [SerializeField] private Climber _playerClimber;
    [SerializeField] private ObjectFinderByMask _fenceFinder;
    [SerializeField] private ObjectFinderByMask _exitSpaceFinder;

    public Action OnTriggered;


    private void OnEnable()
    {
        _player.OnDeath += OnDeath;
        _playerClimber.OnClimb += OnClimb;
        _exitSpaceFinder.OnObjectDetect += Exit;
    }


    private void OnDisable()
    {
        _player.OnDeath -= OnDeath;
        _playerClimber.OnClimb -= OnClimb;
        _exitSpaceFinder.OnObjectDetect -= Exit;
    }



    private void Exit(Transform transform)
    {
        OnTriggered?.Invoke();
    }


    private void OnDeath()
    {
        Exit(null);
    }


    private void OnClimb()
    {
        if (_fenceFinder.IsHaveObjectInSpace)
            Exit(null);
    }
}
