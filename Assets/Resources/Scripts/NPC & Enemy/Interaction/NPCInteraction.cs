using DG.Tweening;
using UnityEngine;


public class NPCInteraction : InteractActivator, IStateRewind
{
    private struct InteractionSnapshot
    {
        public InteractionObject Interaction;
        public bool IsInteractionFinished;
    }


    [SerializeField] private NPCMovement _movement;
    private StateRecorder<InteractionSnapshot> _stateRecorder;


    private void Awake()
    {
        _stateRecorder = new StateRecorder<InteractionSnapshot>(GetSnapshot, ApplySnapshot);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        OnInteractFinded += OnFindInteract;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        OnInteractFinded -= OnFindInteract;
    }


    private void OnFindInteract()
    {
        ApplyFindedInteract(false);
    }


    private void ApplyFindedInteract(bool isForceApply)
    {
        base.ApplyInteract(() =>
        {
            MoveToInteractPosition(_interaction.InteractPosition);
        }, isForceApply);
    }


    private void RotateOnReachedDestination()
    {
        _movement.transform
            .DORotate(_interaction.InteractRotate, 1f)
            .OnComplete(base.PlayIntearct);

        _movement.OnTargetReached -= RotateOnReachedDestination;
    }


    private void MoveToInteractPosition(Vector3 position)
    {
        _movement.SetTarget(position);
        _movement.OnTargetReached += RotateOnReachedDestination;
    }


    private InteractionSnapshot GetSnapshot()
    {
        return new InteractionSnapshot {
            Interaction = _interaction,
            IsInteractionFinished = InteractIsFinished
        };
    }


    private void ApplySnapshot(InteractionSnapshot snapshot)
    {
        _interaction = snapshot.Interaction;

        if (snapshot.IsInteractionFinished == false)
            ApplyFindedInteract(true);
    }


    public void Record(bool needRemove) { _stateRecorder.Record(needRemove); } 

    public void Rewind() { _stateRecorder.Rewind(); }
}