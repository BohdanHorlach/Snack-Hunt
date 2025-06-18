using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class NPCMovement : MonoBehaviour, IPaused, IStateRewind
{
    private struct NPCMovementSnapshot
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsSprinting;
        public bool IsCreepWalking;
        public bool CanMove;
    }


    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _creepWalkSpeed = 3f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _dashSpeed = 35f;
    [SerializeField] private float _dashCooldown = 0.3f;

    private StateRecorder<NPCMovementSnapshot> _stateRecorder;
    private Vector3 _lastTargetPosition;
    private bool _isDashing = false;

    private Vector3 CurrentPosition => _navAgent.transform.position;

    public float CurrentSpeed =>
                IsSprinting ? _sprintSpeed :
                IsCreepWalking ? _creepWalkSpeed :
                _moveSpeed;
    public Vector3 Velocity => _navAgent.velocity;
    public bool IsReachedDestination { get; private set; } = true;
    public bool IsSprinting { get; private set; } = false;
    public bool IsCreepWalking { get; private set; } = false;
    public bool CanMove { get; set; } = true;

    public Action OnTargetReached;


    private void Awake()
    {
        _stateRecorder = new StateRecorder<NPCMovementSnapshot>(GetSnapshot, ApplySnapshot);
        _lastTargetPosition = CurrentPosition;
    }


    private void Update()
    {
        if (IsReachedDestination)
            return;

        Vector3 direction = _navAgent.steeringTarget - CurrentPosition;

        if(CanMove == true && _isDashing == false)
            MoveToTarget(_lastTargetPosition, CurrentSpeed);
        RotateToDirection(direction);

        IsReachedDestination = IsPathComplet();
    }


    private bool IsPathComplet()
    {
        if (Vector3.Distance(_lastTargetPosition, _navAgent.transform.position) <= _navAgent.stoppingDistance)
        {
            if (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f)
            {
                OnTargetReached?.Invoke();
                return true;
            }
        }

        return false;
    }


    private void RotateToDirection(Vector3 direction)
    {
        direction.y = 0;
        direction = direction == Vector3.zero ? _navAgent.transform.forward : direction;

        Quaternion lookAtOffset = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtOffset, _rotateSpeed * Time.deltaTime);
    }


    private void MoveToTarget(Vector3 target, float speed)
    {
        _navAgent.speed = speed;
        _navAgent.SetDestination(target);
    }


    private IEnumerator DashCoroutine(Vector3 targetPosition)
    {
        _isDashing = true;
        MoveToTarget(targetPosition, _dashSpeed);

        yield return new WaitForSeconds(_dashCooldown);
        _isDashing = false;
    }


    public void Dash(Vector3 targetPositon)
    {
        if (!_isDashing)
        {
            SetTarget(targetPositon);
            StartCoroutine(DashCoroutine(targetPositon));
        }
    }


    public float GetDistanceToDestination()
    {
        if (_navAgent.path.status == NavMeshPathStatus.PathInvalid)
            return -1f;

        Vector3[] corners = _navAgent.path.corners;
        float distance = 0f;

        for(int i = 0; i < corners.Length - 1; i++)
        {
            distance += Vector3.Distance(corners[i], corners[i + 1]);
        }

        return distance;
    }


    public void SetTarget(Vector3 target)
    {
        _lastTargetPosition = target;
        IsReachedDestination = false;
    }


    public void SetMovementMode(NPCMovementMode movementMode)
    {
        IsCreepWalking = movementMode == NPCMovementMode.CreepWalk;
        IsSprinting = movementMode == NPCMovementMode.Sprint;
    }


    public void Pause()
    {
        _navAgent.isStopped = true;
    }


    public void Resume()
    {
        _navAgent.isStopped = false;
    }


    private NPCMovementSnapshot GetSnapshot()
    {
        return new NPCMovementSnapshot
        {
            Position = _navAgent.transform.position,
            Rotation = _navAgent.transform.rotation,
            IsSprinting = this.IsSprinting,
            IsCreepWalking = this.IsCreepWalking,
            CanMove = this.CanMove
        };
    }


    private void ApplySnapshot(NPCMovementSnapshot snapshot)
    {
        SetTarget(snapshot.Position);
        _navAgent.transform.position = snapshot.Position;
        _navAgent.transform.rotation = snapshot.Rotation;
        IsSprinting = snapshot.IsSprinting;
        IsCreepWalking = snapshot.IsCreepWalking;
        CanMove = snapshot.CanMove;
    }


    public void Record(bool needRemove) { _stateRecorder.Record(needRemove); }

    public void Rewind() { _stateRecorder.Rewind(); }
}
