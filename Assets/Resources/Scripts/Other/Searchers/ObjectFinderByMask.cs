using System;
using UnityEngine;


public class ObjectFinderByMask : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private Transform _lastDetected;

    public Action<Transform> OnObjectDetect;
    public Action<Transform> OnObjectStay;
    public Action<Transform> OnObjectLost;
    public bool IsHaveObjectInSpace => _lastDetected != null;
    public Transform LastDetected { get => _lastDetected; }


    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer) == false)
            return;

        OnObjectDetect?.Invoke(other.transform);
        _lastDetected = other.transform;
    }


    private void OnTriggerStay(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer) == false)
            return;

        OnObjectStay?.Invoke(other.transform);
    }


    private void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer) == false)
            return;

        OnObjectLost?.Invoke(other.transform);

        if (_lastDetected == other.transform)
            _lastDetected = null;
    }


    private bool IsInLayerMask(LayerMask other)
    {
        return ((1 << other) & _layerMask) != 0;
    }


    public void Reset()
    {
        _lastDetected = null;
    }
}