using UnityEngine;


public class RecordingObject : MonoBehaviour
{
    public Vector3 RecordPosition()
    {
        return transform.position;
    }


    public void RewindPosition(Vector3 recordingPosition)
    {
        transform.position = recordingPosition;
    }
}