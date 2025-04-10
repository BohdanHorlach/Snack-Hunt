using UnityEngine;


public class PlayerState : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Climber _climber;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;


    public bool IsOnGround => _characterController.isGrounded;
    public bool IsHoldingObject => _objectGrabber.IsHoldingObject;
    public bool IsReadyToThrow => _objectThrower.IsReadyToThrow;
    public bool IsThrowing => _objectThrower.IsThrowing;
    public bool IsClimbing => _climber.IsClimbing;
    public bool IsBusy => IsThrowing || IsReadyToThrow || IsClimbing;
}