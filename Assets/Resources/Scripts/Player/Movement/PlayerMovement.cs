using System;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
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

    private const float VELOCITY_THRESHOLD = 0.1f;
    private const int ANGLE_HIT_FOR_RESET_VELOCITY = 140;

    private Transform _camera;
    private Action _onTargetReached;
    private CameraDirectionBuffer _cameraBufferDirection;
    private Vector3 _velocity;
    private Vector3 _surfaceNormal;
    private Vector3? _targetPosition = null;
    private Vector2 _inputDirection;
    private Vector2 _bufferInputDirection;
    private float _jumpMoveSpeed;
    private bool _isInput = false;
    private bool _isMoveToTarget = false;
    private bool _isLockedCameraDirection = true;
    private bool _isSilentWalk = false;
    private bool _isSprint = false;
    private bool _isJump = false;
    private bool _isHaveHitOnJump = false;


    private float CurrentSpeed =>
       _isJump ? _jumpMoveSpeed :
       _isSilentWalk ? _silentWalkSpeed :
       _isSprint ? _sprintSpeed :
       _moveSpeed;

    private bool IsCanMove => (_isInput && _playerState.IsBusy == false) || _isMoveToTarget;
    private bool IsCanJump => _playerState.IsOnGround
                            && _playerState.IsBusy == false;

    public Transform MainTransform => _characterController.transform;
    public Vector3 Velocity { get => _characterController.velocity; }

    public bool IsSptinting { get => _isSprint; }
    public bool IsSilentWalking { get => _isSilentWalk; }
    public bool IsJumping { get => _isJump; }



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

        SetSurfaceNormal(angle, hit);
        ResetVerticalVelocityOnHit(angle);
        AdjustJumpMoveSpeedOnHit(angle);
    }


    private void FixedUpdate()
    {
        if (_playerState.IsPaused)
            return;

        ApplyGravity();
    }


    private void Update()
    {
        if (_playerState.IsClimbing || _playerState.IsPaused)
            return;

        if (_isJump || IsCanMove)
            ProcessingMovement();
        else
            Move(Vector3.zero, CurrentSpeed);
    }


    private void LateUpdate()
    {
        if (_playerState.IsPaused)
            return;

        if (_isJump && _playerState.IsOnGround)
            ResetLockerForJump();
    }


    private void SetSurfaceNormal(float angle, ControllerColliderHit hit)
    {
        if (angle <= _characterController.slopeLimit)
            _surfaceNormal = hit.normal;
    }


    private void ResetVerticalVelocityOnHit(float angle)
    {
        if (angle >= ANGLE_HIT_FOR_RESET_VELOCITY)
            _velocity.y = 0;
    }


    private void AdjustJumpMoveSpeedOnHit(float angle)
    {
        if (_isHaveHitOnJump)
            return;

        float speedFactor = Mathf.Abs(Mathf.Cos(angle));

        _jumpMoveSpeed = _jumpMoveSpeed * speedFactor;
        _isHaveHitOnJump = true;
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

        moveDirection.y = 0;

        return moveDirection.normalized;
    }


    private Vector3 CalculateMoveDirection(Vector3? inputDirection = null)
    {
        Vector3 direction = inputDirection ?? GetDirectionByCamera();
        Vector3 moveDirection = direction - Vector3.Dot(direction, _surfaceNormal) * _surfaceNormal;

        moveDirection += Vector3.down * _downforceByMotion;

        return moveDirection.normalized;
    }


    private void Move(Vector3 direction, float speed)
    {
        Vector3 moveOffset = direction * speed * Time.deltaTime;

        if (_playerState.IsOnGround == false || _isJump)
        {
            moveOffset.y = 0;
            moveOffset += _velocity * Time.deltaTime;
        }

        _characterController.Move(moveOffset);
    }


    private void MoveToTarget(Vector3 direction)
    {
        if (Vector3.Distance(MainTransform.position, _targetPosition.Value) <= VELOCITY_THRESHOLD)
        {
            _targetPosition = null;
            _onTargetReached?.Invoke();
            _isMoveToTarget = false;
            return;
        }

        Move(direction, CurrentSpeed);
    }


    private void RotateToDirection(Vector3 direction)
    {
        direction.y = 0;
        direction = direction == Vector3.zero ? MainTransform.forward : direction;

        Quaternion lookAtOffset = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtOffset, _rotateSpeed * Time.deltaTime);
    }


    private void ProcessingMovement()
    {
        bool hasTarget = _targetPosition.HasValue;

        Vector3 moveDirection = hasTarget == false ? 
                        CalculateMoveDirection() :
                        CalculateMoveDirection(_targetPosition.Value - MainTransform.position);

        if (hasTarget)
            MoveToTarget(moveDirection);
        else
            Move(moveDirection, CurrentSpeed);

        RotateToDirection(moveDirection);
    }


    private void PrepareOnJump()
    {
        _velocity.y += _jumpForce;
        _jumpMoveSpeed = CurrentSpeed;
        _isJump = true;
        _isHaveHitOnJump = false;
        _cameraBufferDirection = new CameraDirectionBuffer(_camera.forward, _camera.right);
        _bufferInputDirection = _inputDirection;
    }


    public void SetTarget(Vector3? position, Action onReached = null)
    {
        _targetPosition = position;
        _onTargetReached = onReached;
        _isMoveToTarget = position != null;
    }


    public void InputMove(InputAction.CallbackContext context)
    {
        if (_isMoveToTarget)
            return;

        _isInput = context.performed;

        _inputDirection = context.ReadValue<Vector2>();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (IsCanJump == false || context.started == false)
            return;

        PrepareOnJump();
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



    public void InputUnlockCamera(InputAction.CallbackContext context)
    {
        if (_playerState.IsThrowing)
            return;

        _isLockedCameraDirection = context.canceled;

        _cameraBufferDirection = new CameraDirectionBuffer(_camera.forward, _camera.right);
    }
}
