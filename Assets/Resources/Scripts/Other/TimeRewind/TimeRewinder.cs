using System.Collections.Generic;
using UnityEngine;


public class TimeRewinder : MonoBehaviour
{
    [SerializeField] private float _recordingDuration = 10f;

    private PositionRecorder _recorder;


    private void Awake()
    {
        RecordingObject[] recordingObjects = FindObjectsByType<RecordingObject>(FindObjectsSortMode.None);
        _recorder = new PositionRecorder(recordingObjects, _recordingDuration);
    }


    private void Update()
    {
        _recorder.Record();
    }


    public void Rewind()
    {
        List<RecordingInfo> recordingPositions = _recorder.GetLastPositions();

        foreach (var info in recordingPositions)
        {
            info.RecordingObject.transform.position = info.Position;
        }

        _recorder.ClearRecorded();
    }
}