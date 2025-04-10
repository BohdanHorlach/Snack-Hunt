using System;
using UnityEngine;


public class SearcherInSpace : Patroller
{
    [SerializeField] private SearchingSpace[] _searchingSpaces;
    [SerializeField] private NoiseDetecter _noiseDetect;

    private Vector3 _noisePosition;
    private int _currentPointIndex = 0;

    public Action OnSearchEnd;


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += AddSearchPosition;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= AddSearchPosition;
    }


    private void AddSearchPosition(Vector3 position)
    {
        if (_isSearching)
            return;

        _noisePosition = position;
        //SwitchToNextTarget();
        //Debug.Log("Switch to noise");
    }


    private SearchingSpace FindSearchingSpace(Vector3 noisePosition)
    {
        SearchingSpace searchingSpace = null;
        float minDistance = float.MaxValue;

        foreach (SearchingSpace space in _searchingSpaces)
        {
            float distanceToSpace = Vector3.Distance(noisePosition, space.transform.position);
            if (distanceToSpace < minDistance)
            {
                searchingSpace = space;
                minDistance = distanceToSpace;
            }
        }

        return searchingSpace;
    }


    protected override void SwitchToNextTarget()
    {
        base.SwitchToNextTarget();

        _currentPointIndex++;
        if(_currentPointIndex >= _patrolWaypoints.Length)
        {
            StopSearch();
        }
    }


    public override void StartSearch()
    {
        SearchingSpace searchingSpace = FindSearchingSpace(_noisePosition);
        _patrolWaypoints = searchingSpace.GetPoints();
        _currentPointIndex = 0;

        base.StartSearch();
    }


    public override void StopSearch()
    {
        base.StopSearch();
        OnSearchEnd?.Invoke();
    }
}
