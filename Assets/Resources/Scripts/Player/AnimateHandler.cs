using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private Climber _climber;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;

    private const float JUMP_COOLDOWN = 0.5f;
    private bool _isRunning;
    private bool _isSprint;
    private bool _isSilentWalking;

    public Action OnMakeStep;


    private void OnEnable()
    {
        _objectThrower.OnThrow += PlayThrow;
        _climber.OnClimb += PlayClimb;
    }


    private void OnDisable()
    {
        _objectThrower.OnThrow -= PlayThrow;
        _climber.OnClimb -= PlayClimb;
    }


    private void Update()
    {
        if (_climber.IsClimbing)
            return;

        _animator.SetBool("IsOnGround", _playerState.IsOnGround);
        _animator.SetBool("IsRunning", _isRunning && _playerState.IsOnGround && _playerState.IsReadyToThrow == false);
        _animator.SetBool("IsSprint", _isSprint);
        _animator.SetBool("IsSilentWalking", _isSilentWalking);
    }


    private void DropObject()
    {
        _objectGrabber.DropObject();
        _animator.SetBool("IsHoldingObject", false);
    }


    private void PlayThrow()
    {
        _animator.SetTrigger("ThrowObject");
        _animator.SetBool("IsHoldingObject", false);
    }


    private void PlayClimb()
    {
        if(_objectGrabber.IsHoldingObject == false)
            _animator.SetTrigger("MakeClimb");
    }


    //Calls from animate
    private void MakeStep()
    {
        OnMakeStep?.Invoke();
    }


    public void TakeObject()
    {
        if (_objectGrabber.TryTakeObject() == true)
            _animator.SetBool("IsHoldingObject", true);
    }


    public void InputMove(InputAction.CallbackContext context)
    {
        _isRunning = context.performed;
    }


    public void InputSprint(InputAction.CallbackContext context)
    {
        _isSprint = context.performed;
    }

    
    public void InputSilentWalk(InputAction.CallbackContext context)
    {
        _isSilentWalking = context.performed;
    }


    private void ResetIsJumping()
    {
        _animator.SetBool("IsJumping", false);
    }


    public void PlayJump(InputAction.CallbackContext context)
    {
        if (context.started && _playerState.IsOnGround && !_playerState.IsBusy)
        {
            _animator.SetTrigger("MakeJump");
            _animator.SetBool("IsJumping", true);
            _animator.SetBool("IsOnGround", false);
            Invoke("ResetIsJumping", JUMP_COOLDOWN);
        }
    }


    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started == false)
            return;

        if (_playerState.IsHoldingObject)
            DropObject();
        else
            TakeObject();
    }
}