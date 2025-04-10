using System;
using UnityEngine;

[Serializable]
public struct AudioInvokerInfo
{
    public AudioSource Source;
    [SerializeField, Range(0, 1)] public float Volume;
    [SerializeField] public float AudibilityDistance;
}
