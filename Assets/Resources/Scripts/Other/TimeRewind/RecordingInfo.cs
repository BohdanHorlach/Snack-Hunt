using UnityEngine;

public struct RecordingInfo
{
    public RecordingObject RecordingObject;
    public Vector3 Position;

    public RecordingInfo(RecordingObject recordingObject, Vector3 position)
    {
        RecordingObject = recordingObject;
        Position = position;
    }
}