using System;
using UnityEngine;


public class ObjectFinderByMask : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private Transform _lastDetected;

    public Action<Transform> OnObjectDetect;
    public Action<Transform> OnObjectLost;
    public bool IsHaveObjectInSpace => _lastDetected != null;
    public Transform LastDetected { get => _lastDetected; }


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            OnObjectDetect?.Invoke(other.transform);
            _lastDetected = other.transform;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            OnObjectLost?.Invoke(other.transform);

            if (_lastDetected == other.transform)
                _lastDetected = null;
        }
    }
}