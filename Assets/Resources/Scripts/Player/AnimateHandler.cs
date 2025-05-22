using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHandler : PausedObject, IOnRewind
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private PlayerInteraction _interact;
    [SerializeField] private Climber _climber;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;

    private const float JUMP_COOLDOWN = 0.5f;
    private const float MOVE_THRESHOLD = 0.05f;

    private Vector3 _previousPosition;
    private bool _isSprint;
    private bool _isSilentWalking;

    public Action OnMakeStep;


    private void Start()
    {
        _previousPosition = transform.position;
    }


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
        if (_climber.IsClimbing || _playerState.IsPaused)
            return;

        float yDifference = _previousPosition.y - transform.position.y;
        bool isMoving = Vector3.Distance(_previousPosition, transform.position) > MOVE_THRESHOLD;

        _animator.SetFloat("YDifference", yDifference);
        _animator.SetBool("IsMoving", isMoving && _playerState.IsReadyToThrow == false);
        _animator.SetBool("IsOnGround", _playerState.IsOnGround);
        _animator.SetBool("IsSprint", _isSprint);
        _animator.SetBool("IsSilentWalking", _isSilentWalking);

        _previousPosition = transform.position;
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
        if (_playerState.IsPaused || !_playerState.IsOnGround || _playerState.IsBusy)
            return;

        if (context.started)
        {
            _animator.SetTrigger("MakeJump");
            _animator.SetBool("IsJumping", true);
            _animator.SetBool("IsOnGround", false);
            Invoke("ResetIsJumping", JUMP_COOLDOWN);
        }
    }


    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started == false || _playerState.IsPaused)
            return;

        if (_interact.IsHaveInteract || _interact.InteractIsFinished == false)
            _interact.Input();
        else if (_playerState.IsHoldingObject)
            DropObject();
        else
            TakeObject();
    }


    public override void Pause()
    {
        _animator.enabled = false;
    }


    public override void Resume()
    {
        _animator.enabled = true;
    }


    public void OnRewind()
    {
        DropObject();
    }
}