using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _creepWalkSpeed = 3f;
    [SerializeField] private float _stopDistanceThreshold = 1f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _dashSpeed = 35f;
    [SerializeField] private float _dashCooldown = 0.3f;

    private Vector3 _lastTargetPosition;
    private bool _isDashing = false;


    private Vector3 CurrentPosition => _navAgent.transform.position;
    private float CurrentSpeed =>
                IsSprinting ? _sprintSpeed :
                IsCreepWalking ? _creepWalkSpeed :
                _moveSpeed;

    public bool IsMoving { get; private set; }
    public bool IsSprinting { get; private set; } = false;
    public bool IsCreepWalking { get; private set; } = false;



    private void Awake()
    {
        _lastTargetPosition = CurrentPosition;
    }


    private void Update()
    {
        IsMoving = Vector3.Distance(CurrentPosition, _lastTargetPosition) > _stopDistanceThreshold;

        if (IsMoving == false)
            return;

        Vector3 direction = _navAgent.steeringTarget - CurrentPosition;

        if(_enemyAttack.IsAttaking == false && _isDashing == false)
            MoveToTarget(_lastTargetPosition, CurrentSpeed);
        RotateToDirection(direction);
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
            _lastTargetPosition = targetPositon;
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
    }


    public void SetMovementMode(EnemyMovementMode movementMode)
    {
        IsCreepWalking = movementMode == EnemyMovementMode.CreepWalk;
        IsSprinting = movementMode == EnemyMovementMode.Sprint;
    }
}
