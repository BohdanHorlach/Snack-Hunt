using System;
using UnityEngine;


public class NoiseDetecter : MonoBehaviour
{
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


    private void OnPlayAudio(float audibility, Vector3 noisePosition)
    {
        if (_noiseAudibilityThreshold <= audibility)
            OnNoiseDetect?.Invoke(noisePosition);
    }
}