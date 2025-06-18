using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TimeRewinder : MonoBehaviour
{
    [SerializeField] private PlayerDetecter[] _enemys; 
    [SerializeField] private float _recordingDuration = 10f;

    private IOnRewind[] _onRewinds;
    private IStateRewind[] _otherRewinds;
    private PositionRecorder _recorder;


    private void Start()
    {
        var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        _onRewinds = allMonoBehaviours
            .OfType<IOnRewind>()
            .ToArray();

        _otherRewinds = allMonoBehaviours
            .OfType<IStateRewind>()
            .ToArray();

        RecordingObject[] recordingObjects = FindObjectsByType<RecordingObject>(FindObjectsSortMode.None);
        _recorder = new PositionRecorder(recordingObjects, _otherRewinds, _recordingDuration);
    }


    private void Update()
    {
        if (IsVisibleByEnemys())
            return;

        _recorder.Record();
    }


    private bool IsVisibleByEnemys()
    {
        foreach (PlayerDetecter enemy in _enemys)
        {
            if (enemy.IsDetect)
                return true;
        }

        return false;
    }


    private void RewindPosition()
    {
        List<RecordingInfo> recordingPositions = _recorder.GetLastPositions();

        foreach (var info in recordingPositions)
        {
            info.RecordingObject.transform.position = info.Position;
        }
    }


    private void PrepareToRewind()
    {
        foreach (var item in _onRewinds)
            item.OnBeforeRewind();
    }


    private void RewindOther()
    {
        foreach (var item in _otherRewinds)
            item.Rewind();
    }



    //Call from animator
    public void Rewind()
    {
        PrepareToRewind();
        RewindPosition();
        RewindOther();

        PauseHandler.Play();
    }
}