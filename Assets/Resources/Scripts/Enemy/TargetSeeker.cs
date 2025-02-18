using System;
using System.Collections.Generic;
using UnityEngine;


public class TargetSeeker : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolWaypoints;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private NoiseDetecter _noiseDetect;
    [SerializeField] private float _switchDelay = 7.5f;

    private float DISTANCE_THRESHOLD = 2f;
    private List<Vector3> _searchPosition;
    private Vector3 _currentDestination;
    private int _currentIndex = 0;
    private bool _isSearching = false;
    private bool _isSwitchingPoint = false;


    public Action OnReachingWaypoint;


    private void Awake()
    {
        _searchPosition = new List<Vector3>();
    }


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += AddSearchPosition;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= AddSearchPosition;
    }


    private void Update()
    {
        if (_isSearching == false || _isSwitchingPoint)
            return;

        MoveToNextPoint();
    }


    private Vector3 GetNextPosition() 
    {
        Vector3 nextPosition;

        if (_searchPosition.Count != 0)
        {
            nextPosition = _searchPosition[0];
            _searchPosition.RemoveAt(0);
        }
        else
        {
            nextPosition = _patrolWaypoints[_currentIndex].position;
            _currentIndex = (_currentIndex + 1) % _patrolWaypoints.Length;
        }

        return nextPosition;
    }


    private void SwitchToNextTarget()
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


    private void AddSearchPosition(Vector3 position)
    {
        _searchPosition.Add(position);
    }


    public void StartSearch()
    {
        _isSearching = true;
        SwitchToNextTarget();
    }


    public void StopSearch()
    {
        _isSearching = false;
    }
}