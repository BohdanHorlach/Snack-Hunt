using System;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class NoiseDetecter : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField, Range(0, 1)] private float _noiseAudibilityThreshold = 0.6f;

    public Action<Vector3> OnNoiseDetect;


    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out AudioInvoker audioInvoker)){
            audioInvoker.OnPlay += OnPlayAudio;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out AudioInvoker audioInvoker))
        {
            audioInvoker.OnPlay -= OnPlayAudio;
        }
    }


    private void OnPlayAudio(float volume, float audibilityDistance, Vector3 noisePosition)
    {
        bool isHearIt = _noiseAudibilityThreshold <= volume;
        float distanceToNoise = Vector3.Distance(noisePosition, transform.position);

        if (isHearIt && distanceToNoise <= audibilityDistance)
            OnNoiseDetect?.Invoke(noisePosition);
    }
}