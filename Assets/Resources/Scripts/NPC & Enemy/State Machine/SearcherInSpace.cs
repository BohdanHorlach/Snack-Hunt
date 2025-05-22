using System;
using System.Linq;
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
    }


    private SearchingSpace FindSearchingSpace(Vector3 noisePosition)
    {
        SearchingSpace closestSpace = _searchingSpaces
            .OrderBy(space => space.GetPoints()
                .Min(point => Vector3.Distance(point.position, noisePosition)))
            .FirstOrDefault();

        return closestSpace;
    }


    private void SortPointsByDistance(Transform[] points)
    {
        Array.Sort(points, (a, b) =>
        {
            float distA = Vector3.Distance(a.position, _noisePosition);
            float distB = Vector3.Distance(b.position, _noisePosition);
            
            return distA.CompareTo(distB);
        });
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
        SortPointsByDistance(_patrolWaypoints);

        _currentPointIndex = 0;

        base.StartSearch();
    }


    public override void StopSearch()
    {
        base.StopSearch();
        OnSearchEnd?.Invoke();
    }
}
