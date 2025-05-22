using System;
using System.Collections;
using UnityEngine;


[System.Serializable]
public struct InteractClip
{
    public AnimationClip Clip;
    public bool NeedDinamicBlend;
}


public class InteractionObject : MonoBehaviour
{
    public Transform AttachebleObject;
    public Vector3 InteractPosition;
    public Vector3 InteractRotate;
    public InteractClip[] Clips;
    public InteractClip[] AbortClips;
    public float Cooldown = 3f;
    public bool AutomaticAbortOfAnimation = true;

    [HideInInspector] public bool IsActive { get; private set; }


    private IEnumerator InvokeWithDelay(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);

        callback?.Invoke();
    }


    public void SetActive(bool active)
    {
        IsActive = active;
    }


    public void SetActiveWithDelay(bool active)
    {
        StartCoroutine(InvokeWithDelay(() => { SetActive(active); }, Cooldown));
    }
}