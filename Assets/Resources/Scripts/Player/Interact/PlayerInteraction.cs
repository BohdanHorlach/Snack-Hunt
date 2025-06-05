using DG.Tweening;
using UnityEngine;



public class PlayerInteraction : InteractActivator
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private ObjectGrabber _grabber;
    [SerializeField] private VisibilityByOther _visibilityByEnemy;

    public bool IsCanInteract => _visibilityByEnemy.IsVisible == false
                                && _grabber.IsHoldingObject == false;


    private void MoveToInteractPosition(Vector3 position)
    {
        _movement.MoveTo(position, RotateOnReachedDestination);
    }


    private void RotateOnReachedDestination()
    {
        _movement.MainTransform
            .DORotate(_interaction.InteractRotate, 1f)
            .OnComplete(base.PlayIntearct);
    }


    public void Input()
    {
        if (IsCanInteract == false)
            return;

        if (_interaction.IsActive)
            base.AbortInteract(null);
        else
            base.ApplyInteract(() => MoveToInteractPosition(_interaction.InteractPosition));
    } 
}