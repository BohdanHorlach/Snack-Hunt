using UnityEngine;
using UnityEngine.AI;


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 12f;
    [SerializeField] private float _creepWalkSpeed = 3f;
    [SerializeField] private float _stopDistanceThreshold = 1f;

    private Vector3 _lastTargetPosition;

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
        
        if (IsMoving)
            MoveToTarget(_lastTargetPosition, CurrentSpeed);
    }


    private void MoveToTarget(Vector3 target, float speed)
    {
        _navAgent.speed = speed;
        _navAgent.SetDestination(target);
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
