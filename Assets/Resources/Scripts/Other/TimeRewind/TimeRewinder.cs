using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TimeRewinder : MonoBehaviour
{
    [SerializeField] private float _recordingDuration = 10f;

    private IOnRewind[] _otherRewinds;
    private PositionRecorder _recorder;


    private void Awake()
    {
        RecordingObject[] recordingObjects = FindObjectsByType<RecordingObject>(FindObjectsSortMode.None);
        _recorder = new PositionRecorder(recordingObjects, _recordingDuration);

        _otherRewinds = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IOnRewind>()
                .ToArray();
    }


    private void Update()
    {
        _recorder.Record();
    }


    private void RewindPosition()
    {
        List<RecordingInfo> recordingPositions = _recorder.GetLastPositions();

        foreach (var info in recordingPositions)
        {
            info.RecordingObject.transform.position = info.Position;
        }

        _recorder.ClearRecorded();
    }


    private void PrepareToRewind()
    {
        foreach (var item in _otherRewinds)
            item.OnBeforeRewind();
    }



    //Call from animator
    public void Rewind()
    {
        PrepareToRewind();
        RewindPosition();

        PauseHandler.Play();
    }
}