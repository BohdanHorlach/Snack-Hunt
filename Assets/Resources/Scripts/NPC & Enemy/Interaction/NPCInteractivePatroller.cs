using System;
using UnityEngine;


public class NPCInteractivePatroller : MonoBehaviour
{
    [SerializeField] private NPCMovement _movement;
    [SerializeField] private NPCInteraction _interaction;
    [SerializeField] private Patroller _patrollerForInteract;


    private void OnEnable()
    {
        _interaction.OnApplyInteract += StopMove;
        _interaction.OnInteractFinished += KeepMove;
    }


    private void OnDisable()
    {
        _interaction.OnApplyInteract -= StopMove;
        _interaction.OnInteractFinished -= KeepMove;
    }


    private void StopMove()
    {
        _movement.Pause();
    }


    private void KeepMove()
    {
        _movement.Resume();
    }


    public void StartPatrolling()
    {
        _patrollerForInteract.StartSearch();
    }


    public void StopPatrolling(Action callback)
    {
        _patrollerForInteract.StopSearch();

        _interaction.AbortInteract(
            () =>
            {
                _movement.Resume();
                callback?.Invoke();
            });
    }
}