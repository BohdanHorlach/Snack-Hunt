using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class Climber : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _freeSpaceOfClimbFinder;
    [SerializeField] private ObjectFinderByMask _obstacleFinder;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _mainTransform;
    [SerializeField] private Transform _finalClimbPosition;
    [SerializeField] private LayerMask _obstacleMask;

    private const float Y_OFFSET_FOR_CLIMB = 0.6f;
    private const float RAY_CAST_DISTANCE = 5f;

    private Vector3 _finalPosition;
    private bool IsCanClimb => _freeSpaceOfClimbFinder.IsHaveObjectInSpace == false
                            && _obstacleFinder.IsHaveObjectInSpace == true
                            && _playerState.IsBusy == false
                            && _playerState.IsHoldingObject == false;

    public bool IsClimbing { get; private set; } = false;

    public Action OnClimb;
    public Action OnClimbFinished;


    private void FixedUpdate()
    {
        if (_playerState.IsPaused)
            return;

        AutoClimb();
    }


    private float GetYOffsetForClimb()
    {
        if (Physics.Raycast(_freeSpaceOfClimbFinder.transform.position, Vector3.down, out RaycastHit hit, RAY_CAST_DISTANCE, _obstacleMask))
            return -(hit.distance - Y_OFFSET_FOR_CLIMB);

        return 0f;
    }


    private void SetPosition()
    {
        float yOffset = GetYOffsetForClimb();

        _finalPosition = _finalClimbPosition.position;
        _root.localPosition = _finalClimbPosition.localPosition;
        _root.position += new Vector3(0, yOffset, 0);
    }


    private void MakeClimb()
    {
        OnClimb?.Invoke();
        IsClimbing = true;
        SetPosition();
    }


    private void AutoClimb()
    {
        if (_playerState.IsOnGround == false && IsCanClimb)
            MakeClimb();
    }


    public void Climb(InputAction.CallbackContext context)
    {
        if (context.started && IsCanClimb && !_playerState.IsPaused)
            MakeClimb();
    }


    public void ClimbFinished()
    {
        _mainTransform.position = _finalPosition;
        _root.localPosition = Vector3.zero;
        IsClimbing = false;

        OnClimbFinished?.Invoke();
    }
}