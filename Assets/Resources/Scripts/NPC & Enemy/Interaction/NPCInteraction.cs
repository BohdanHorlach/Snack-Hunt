using DG.Tweening;
using System;
using UnityEngine;


public class NPCInteraction : InteractActivator
{
    [SerializeField] private NPCMovement _movement;


    protected override void OnEnable()
    {
        base.OnEnable();
        OnInteractFinded += OnFindInteract;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        OnInteractFinded -= OnFindInteract;
    }


    private void OnFindInteract()
    {
        base.ApplyInteract(() =>
        {
            MoveToInteractPosition(_interaction.InteractPosition);
        });
    }


    private void RotateOnReachedDestination()
    {
        _movement.transform
            .DORotate(_interaction.InteractRotate, 1f)
            .OnComplete(base.PlayIntearct);

        _movement.OnTargetReached -= RotateOnReachedDestination;
    }


    private void MoveToInteractPosition(Vector3 position)
    {
        _movement.SetTarget(position);
        _movement.OnTargetReached += RotateOnReachedDestination;
    }
}