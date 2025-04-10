using UnityEngine;


public struct CameraDirectionBuffer
{
    public Vector3 forward;
    public Vector3 right;

    public CameraDirectionBuffer(Vector3 forward, Vector3 right)
    {
        this.forward = forward;
        this.right = right;
    }
}