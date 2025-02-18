using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private ObjectThrower _objectThrower;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 10f;
    [SerializeField] private float _silentWalkSpeed = 1f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _jumpForce = 3f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _gravityMultiplier = 1.5f;
    [SerializeField] private float _downforceByMotion = 0.1f;


    private Transform _camera;
    private CameraDirectionBuffer _cameraBufferDirection;
    private Vector3 _velocity;
    private Vector3 _surfaceNormal;
    private Vector2 _inputDirection;
    private Vector2 _bufferInputDirection;
    private float _jumpMoveSpeed;
    private bool _isInput = false;
    private bool _isLockedCameraDirection = true;
    private bool _isSilentWalk = false;
    private bool _isSprint = false;
    private bool _isJump = false;


    private float CurrentSpeed =>
       _isJump ? _jumpMoveSpeed :
       _isSilentWalk ? _silentWalkSpeed :
       _isSprint ? _sprintSpeed :
       _moveSpeed;

    private bool IsCanMove => _isInput && _playerState.IsBusy == false;
    private bool IsCanJump => _playerState.IsOnGround
                            && _playerState.IsHoldingObject == false
                            && _playerState.IsBusy == false;


    private void Awake()
    {
        _camera = Camera.main.transform;
    }


    private void OnEnable()
    {
        _objectThrower.OnPrepareToThrow += UnlockCamera;
        _objectThrower.OnCanceledPrepareToThrow += LockCamera;
    }


    private void OnDisable()
    {
        _objectThrower.OnPrepareToThrow -= UnlockCamera;
        _objectThrower.OnCanceledPrepareToThrow -= LockCamera;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle <= _characterController.slopeLimit)
            _surfaceNormal = hit.normal;
    }


    private void FixedUpdate()
    {
        ApplyGravity();
    }


    private void Update()
    {
        if (_playerState.IsClimbing)
            return;

        if (_isJump || IsCanMove)
        {
            ProcessingMovement();
        }
        else
        {
            Move(Vector3.zero, 0);
        }
    }


    private void LateUpdate()
    {
        if (_isJump && _playerState.IsOnGround)
            ResetLockerForJump();
    }


    private void LockCamera()
    {
        _isLockedCameraDirection = true;
    }


    private void UnlockCamera()
    {
        _isLockedCameraDirection = false;
    }


    private void ApplyGravity()
    {
        if (_playerState.IsOnGround)
            _velocity.y = -_gravity * _gravityMultiplier * Time.fixedDeltaTime;
        else
            _velocity.y -= _gravity * _gravityMultiplier * Time.fixedDeltaTime;
    }


    private void ResetLockerForJump()
    {
        LockCamera();
        _isJump = false;
    }


    private Vector3 GetDirectionByCamera()
    {
        Vector3 cameraForward = _isLockedCameraDirection ? _camera.forward : _cameraBufferDirection.forward;
        Vector3 cameraRight = _isLockedCameraDirection ? _camera.right : _cameraBufferDirection.right;
        Vector2 inputDirection = _isJump ? _bufferInputDirection : _inputDirection;

        Vector3 moveDirection = cameraForward * inputDirection.y
                            + cameraRight * inputDirection.x;

        return moveDirection.normalized;
    }


    private Vector3 CalculateMoveDirection()
    {
        Vector3 cameraDirection = GetDirectionByCamera();
        Vector3 moveDirection = cameraDirection - Vector3.Dot(cameraDirection, _surfaceNormal) * _surfaceNormal;

        moveDirection += Vector3.down * _downforceByMotion;

        return moveDirection.normalized;
    }


    private void Move(Vector3 direction, float speed)
    {
        Debug.DrawRay(transform.position, direction * 10, Color.red);

        Vector3 offset = direction * speed * Time.deltaTime;
        offset += _velocity * Time.deltaTime;

        _characterController.Move(offset);
    }


    private void RotateToDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        direction.y = 0;

        Quaternion lookAtOffset = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtOffset, _rotateSpeed * Time.deltaTime);
    }


    private void ProcessingMovement()
    {
        Vector3 moveDirection = CalculateMoveDirection();

        Move(moveDirection, CurrentSpeed);
        RotateToDirection(moveDirection);
    }


    public void InputMove(InputAction.CallbackContext context)
    {
        _isInput = context.performed;

        _inputDirection = context.ReadValue<Vector2>();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (IsCanJump == false || context.started == false)
            return;

        _velocity.y += _jumpForce;
        _jumpMoveSpeed = CurrentSpeed;
        _isJump = true;
        _cameraBufferDirection = new CameraDirectionBuffer(_camera.forward, _camera.right);
        _bufferInputDirection = _inputDirection;

        Invoke("UnlockCamera", Time.deltaTime);
    }


    public void SetSilentWalkMode(InputAction.CallbackContext context)
    {
        _isSilentWalk = context.performed;
    }


    public void SetSprintMode(InputAction.CallbackContext context)
    {
        _isSprint = context.performed;
    }



    public void UnlockCamera(InputAction.CallbackContext context)
    {
        if (_playerState.IsThrowing)
            return;

        _isLockedCameraDirection = context.canceled;

        _cameraBufferDirection = new CameraDirectionBuffer(_camera.forward, _camera.right);
    }
}
