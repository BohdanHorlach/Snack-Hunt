using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MotionDetecter : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _finder;
    [SerializeField] private SearchSettings _searchSettings;
    [SerializeField] private float _motionThreshold = 0.2f;

    private List<Transform> _detected;
    private List<Vector3> _previousPositions; 
    private ObjectSearcher _objectSearcher;
    private Transform _target;

    public Action<Vector3> OnMotionDetected;


    private void Awake()
    {
        _detected = new List<Transform>();
        _previousPositions = new List<Vector3>();
        _objectSearcher = new ObjectSearcher(_searchSettings);
    }


    private void OnEnable()
    {
        _finder.OnObjectDetect += AddTarget;
        _finder.OnObjectLost += RemoveTarget;
    }


    private void OnDisable()
    {
        _finder.OnObjectDetect -= AddTarget;
        _finder.OnObjectLost -= RemoveTarget;
    }


    private void Update()
    {
        Search();
    }


    private void LateUpdate()
    {
        FindTarget();
        UpdatePreviousPositions();
    }


    private void AddTarget(Transform transform)
    {
        if (_detected.Contains(transform))
            return;

        _detected.Add(transform);
        _previousPositions.Add(transform.position);

        Debug.Log($"Detect {transform.name}");
    }


    private void RemoveTarget(Transform transform)
    {
        if (!_detected.Contains(transform))
            return;

        int index = _detected.IndexOf(transform);
        _detected.RemoveAt(index);
        _previousPositions.RemoveAt(index);

        Debug.Log($"Lost {transform.name}");
    }


    private void UpdatePreviousPositions()
    {
        for (int i = 0; i < _previousPositions.Count; i++)
        {
            _previousPositions[i] = _detected[i].position;
        }
    }


    private void FindTarget()
    {
        _target = null;

        if (_detected.Count == 0)
            return;

        _target = _detected
                .Select((t, index) => new { Target = t, Index = index })
                .Where(t => Vector3.Distance(_previousPositions[t.Index], t.Target.position) >= _motionThreshold)
                .OrderBy(t => Vector3.Distance(transform.position, t.Target.position))
                .Select(t => t.Target)
                .FirstOrDefault();
    }


    private void Search()
    {
        if (_target != null && _objectSearcher.SearchByRays(_target))
            OnMotionDetected?.Invoke(_target.position);
    }


    private void OnDrawGizmos()
    {
        if (_objectSearcher == null)
            return;

        _objectSearcher.DrawFieldOfView();

        if (_target != null)
            _objectSearcher.DrawRay(_target);
    }
}
