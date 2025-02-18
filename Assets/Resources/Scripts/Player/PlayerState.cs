using UnityEngine;


public class PlayerState : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private ClimbHandler _climbHandler;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;


    public bool IsOnGround => _characterController.isGrounded;
    public bool IsHoldingObject => _objectGrabber.IsHoldingObject;
    public bool IsReadyToThrow => _objectThrower.IsReadyToThrow;
    public bool IsThrowing => _objectThrower.IsThrowing;
    public bool IsClimbing => _climbHandler.IsClimbing;
    public bool IsBusy => IsThrowing || IsReadyToThrow || IsClimbing;
}