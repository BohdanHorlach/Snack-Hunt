using UnityEngine;
using UnityEngine.InputSystem;


public class AnimateHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private ClimbHandler _climbHandler;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private ObjectThrower _objectThrower;

    private bool _isRunning;
    private bool _isSprint;
    private bool _isSilentWalking;



    private void OnEnable()
    {
        _objectThrower.OnThrow += PlayThrow;
        _climbHandler.OnClimb += PlayClimb;
    }


    private void OnDisable()
    {
        _objectThrower.OnThrow -= PlayThrow;
        _climbHandler.OnClimb -= PlayClimb;
    }


    private void Update()
    {
        _animator.SetBool("IsOnGround", _playerState.IsOnGround);
        _animator.SetBool("IsRunning", _isRunning && _playerState.IsOnGround && _playerState.IsReadyToThrow == false);
        _animator.SetBool("IsSprint", _isSprint);
        _animator.SetBool("IsSilentWalking", _isSilentWalking);
    }


    private void TakeObject()
    {
        if (_objectGrabber.TryTakeObject() == true)
            _animator.SetTrigger("TakeObject");
    }


    private void DropObject()
    {
        _objectGrabber.DropObject();
        _animator.SetTrigger("DropObject");
    }


    private void PlayThrow()
    {
        _animator.SetTrigger("ThrowObject");
    }


    private void PlayClimb()
    {
        _animator.SetTrigger("MakeClimb");
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


    public void PlayJump(InputAction.CallbackContext context)
    {
        if (context.started && _playerState.IsOnGround && _playerState.IsHoldingObject == false && _playerState.IsBusy == false)
            _animator.SetTrigger("Jump");
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