using System;
using UnityEngine;


public class Patroller : MonoBehaviour
{
    [SerializeField] protected Transform[] _patrolWaypoints;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private float _switchDelay = 7.5f;

    private float DISTANCE_THRESHOLD = 2f;

    private Vector3 _currentDestination;
    private int _currentIndex = 0;
    private bool _isSwitchingPoint = false;
    protected bool _isSearching = false;


    public Action OnReachingWaypoint;


    private void Update()
    {
        if (_isSearching == false || _isSwitchingPoint)
            return;

        MoveToNextPoint();
    }


    private Vector3 GetNextPosition()
    {
        Vector3 nextPosition = _patrolWaypoints[_currentIndex].position;
        _currentIndex = (_currentIndex + 1) % _patrolWaypoints.Length;

        return nextPosition;
    }


    protected virtual void SwitchToNextTarget()
    {
        Vector3 nextPoint = GetNextPosition();
        _isSwitchingPoint = false;

        if (_isSearching)
        {
            _enemyMovement.SetTarget(nextPoint);
            _currentDestination = nextPoint;
        }
    }


    private void MoveToNextPoint()
    {
        Vector3 position = _enemyMovement.transform.position;

        if (Vector3.Distance(_currentDestination, position) > DISTANCE_THRESHOLD)
            return;

        OnReachingWaypoint?.Invoke();
        _isSwitchingPoint = true;
        Invoke("SwitchToNextTarget", _switchDelay);
    }


    public virtual void StartSearch()
    {
        _isSearching = true;
        SwitchToNextTarget();
    }


    public virtual void StopSearch()
    {
        _isSearching = false;
    }
}