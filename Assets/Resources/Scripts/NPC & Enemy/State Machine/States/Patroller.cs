using System;
using UnityEngine;


public class Patroller : MonoBehaviour, IStateRewind
{
    private struct PatrollerSnapshot
    {
        public int IndexPoint;
        public bool IsSwitchingPoint;
        public bool IsSearching;
    }


    [SerializeField] protected Transform[] _patrolWaypoints;
    [SerializeField] private NPCMovement _enemyMovement;
    [SerializeField] private float _switchDelay = 7.5f;

    private StateRecorder<PatrollerSnapshot> _stateRecorder;
    private int _currentIndex = 0;
    private bool _isSwitchingPoint = false;
    protected bool _isSearching = false;

    public Action OnReachingWaypoint;


    protected virtual void Awake()
    {
        _stateRecorder = new StateRecorder<PatrollerSnapshot>(GetSnapshot, ApplySnapshot);
    }


    private void Update()
    {
        if (_isSearching == false || _isSwitchingPoint || PauseHandler.IsPaused)
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
            _enemyMovement.SetTarget(nextPoint);
    }


    private void MoveToNextPoint()
    {
        if (_enemyMovement.IsReachedDestination == false)
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


    private PatrollerSnapshot GetSnapshot()
    {
        return new PatrollerSnapshot
        {
            IndexPoint = _currentIndex,
            IsSwitchingPoint = _isSwitchingPoint,
            IsSearching = _isSearching
        };
    }


    private void ApplySnapshot(PatrollerSnapshot snapshot)
    {
        _currentIndex = snapshot.IndexPoint;
        _isSwitchingPoint = snapshot.IsSwitchingPoint;
        _isSearching = snapshot.IsSearching;
    }


    public virtual void Record(bool needRemove) { _stateRecorder.Record(needRemove); }

    public virtual void Rewind() { _stateRecorder.Rewind(); }
}