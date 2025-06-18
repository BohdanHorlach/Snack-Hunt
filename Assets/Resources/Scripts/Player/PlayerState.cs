using UnityEngine;


public class PlayerState : MonoBehaviour, IPaused
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Climber _climber;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;
    [SerializeField] private ObjectFinderByMask _groundFinder;


    public bool IsOnGround => _characterController.isGrounded || ( _groundFinder.IsHaveObjectInSpace && !_playerMovement.IsJumping);
    public bool IsHoldingObject => _objectGrabber.IsHoldingObject;
    public bool IsReadyToThrow => _objectThrower.IsReadyToThrow;
    public bool IsThrowing => _objectThrower.IsThrowing;
    public bool IsClimbing => _climber.IsClimbing;
    public bool IsActiveInteraction => _interaction.InteractIsFinished == false;
    public bool IsBusy => IsThrowing || IsReadyToThrow || IsClimbing || IsActiveInteraction;
    public bool IsPaused { get => PauseHandler.IsPaused; }
}