using System;
using DG.Tweening;
using UnityEngine;



public class PlayerInteraction : InteractActivator
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private GameObject _enemyTarget;
    [SerializeField] private ObjectGrabber _grabber;
    [SerializeField] private VisibilityByOther _visibilityByEnemy;

    public bool IsCanInteract => _visibilityByEnemy.IsVisible == false
                                && _grabber.IsHoldingObject == false;


    private void RotateOnReachedDestination()
    {
        _movement.MainTransform
            .DORotate(_interaction.InteractRotate, 1f)
            .OnComplete(base.PlayIntearct);
    }


    private void ConfigureInteraction(Vector3? targetPosition, Action onReachedCallback, bool isEnemyActive)
    {
        _movement.SetTarget(targetPosition, onReachedCallback);
        _enemyTarget.SetActive(isEnemyActive);
    }


    private void OnAbortInteract()
    {
        ConfigureInteraction(null, null, true);
    }


    private void OnApplyInteraction()
    {
        ConfigureInteraction(
            _interaction.InteractPosition,
            RotateOnReachedDestination,
            false
        );
    }


    public void Input()
    {
        if (IsCanInteract == false)
            return;

        if (_interaction.IsActive)
            base.AbortInteract(OnAbortInteract);
        else
            base.ApplyInteract(OnApplyInteraction);
    } 
}