using System.Collections.Generic;
using System;


public class StateRecorder<TSnapshot>
{
    private readonly Queue<TSnapshot> _snapshots = new();
    private readonly Func<TSnapshot> _getSnapshot;
    private readonly Action<TSnapshot> _applySnapshot;

    public StateRecorder(Func<TSnapshot> getSnapshot, Action<TSnapshot> applySnapshot)
    {
        _getSnapshot = getSnapshot;
        _applySnapshot = applySnapshot;
    }


    public void Record(bool needRemove)
    {
        _snapshots.Enqueue(_getSnapshot());

        if (needRemove && _snapshots.Count > 0)
            _snapshots.Dequeue();
    }


    public void Rewind()
    {
        if (_snapshots.Count == 0)
            return;

        var snapshot = _snapshots.Dequeue();
        _applySnapshot(snapshot);
    }
}
