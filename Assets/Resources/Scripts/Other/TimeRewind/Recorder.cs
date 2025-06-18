using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PositionRecorder
{
    private Queue<List<RecordingInfo>> _recordingInfos;
    private RecordingObject[] _recordingObjects;
    private IStateRewind[] _otherRewinds;
    private float _recordingDuration;
    private float _recordingTime = 0f;


    public PositionRecorder(RecordingObject[] recordingObjects, IStateRewind[] otherRewinds, float recordingDuration = 10f)
    {
        _recordingInfos = new Queue<List<RecordingInfo>>();
        _otherRewinds = otherRewinds;
        _recordingObjects = recordingObjects;
        _recordingDuration = recordingDuration;
    }


    private void RecordPositions()
    {
        List<RecordingInfo> recordingPosition = new List<RecordingInfo>();
        
        foreach (var obj in _recordingObjects)
            recordingPosition.Add(new RecordingInfo(obj, obj.RecordPosition()));

        _recordingInfos.Enqueue(recordingPosition);
    }


    private void RecordOther(bool needRemoveLast)
    {
        foreach (var item in _otherRewinds)
            item.Record(needRemoveLast);
    }


    private void RemoveLastPositions()
    {
        _recordingInfos.Dequeue();
        _recordingTime -= Time.deltaTime;
    }


    public void Record()
    {
        _recordingTime += Time.deltaTime;

        bool needRemoveLast = _recordingTime >= _recordingDuration;

        RecordPositions();
        RecordOther(needRemoveLast);
        
        if(needRemoveLast)
            RemoveLastPositions();
    }


    public List<RecordingInfo> GetLastPositions()
    {
        return _recordingInfos.Dequeue();
    }


    public void ClearRecorded()
    {
        _recordingInfos.Clear();
        _recordingTime = 0f;
    }
}