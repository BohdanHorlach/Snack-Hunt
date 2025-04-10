using UnityEngine;


[System.Serializable]
public class SearchSettings
{
    public Transform rayPosition;
    public float rayDistance = 10f;
    public int rayCount = 7;
    [Range(0, 1)] public float dotThreshold = 0.1f;
    [Range(1, 90)] public float searchVerticalAngle = 50f;
    [Range(1, 90)] public float searchForwardRaysAngle = 10f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
}
