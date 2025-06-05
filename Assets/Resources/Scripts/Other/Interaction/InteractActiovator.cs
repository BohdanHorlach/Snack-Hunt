using System;
using UnityEngine;


public class InteractActivator : MonoBehaviour
{
    [SerializeField] private InteractAnimateHandler _animateHandler;
    [SerializeField] private ObjectFinderByMask _interactFinder;
    [SerializeField] private Transform _attacheblePlace;

    private Vector3 _defaultInteractionObjectPosition;
    protected InteractionObject _interaction;

    public bool IsHaveInteract => _interaction != null 
                                && !_interaction.IsActive 
                                && _interactFinder.IsHaveObjectInSpace;
    public bool InteractIsFinished { get; private set; } = true;


    public Action OnInteractFinded;
    public Action OnApplyInteract;
    public Action OnInteractFinished;


    protected virtual void OnEnable()
    {
        _interactFinder.OnObjectStay += FindInteract;
        _interactFinder.OnObjectLost += LostInteract;
    }


    protected virtual void OnDisable()
    {
        _interactFinder.OnObjectStay -= FindInteract;
        _interactFinder.OnObjectLost -= LostInteract;
    }


    private void AttachInteractObjectToPlace(Transform attacheblePlace)
    {
        if (_interaction.AttachebleObject == null)
            return;

        Transform interact = _interaction.AttachebleObject;

        if (attacheblePlace == null)
            interact.position = _defaultInteractionObjectPosition;
        else
            _defaultInteractionObjectPosition = interact.position;

        interact.SetParent(attacheblePlace);
    }


    private void OnAnimationFinished(Action callback = null)
    {
        _interaction.SetActiveWithDelay(false);
        InteractIsFinished = true;
        AttachInteractObjectToPlace(null);

        OnInteractFinished?.Invoke();
        callback?.Invoke();
    }


    private void FindInteract(Transform obj)
    {
        if (obj.TryGetComponent(out InteractionObject interaction))
        {
            _interaction = interaction;
            OnInteractFinded?.Invoke();
        }
    }


    private void LostInteract(Transform obj)
    {
        if (obj.TryGetComponent(out InteractionObject interaction))
        {
            if (interaction == _interaction && _interaction.IsActive == false)
            {
                _interaction = null;
            }
        }
    }


    protected void PlayIntearct()
    {
        AttachInteractObjectToPlace(_attacheblePlace);

        _animateHandler.PlayInteractClips(
                    _interaction.Clips,
                    _interaction.AutomaticAbortOfAnimation,
                    () => OnAnimationFinished()
        );
    }


    protected void ApplyInteract(Action callback = null)
    {
        if (IsHaveInteract == false)
            return;

        OnApplyInteract?.Invoke();

        _interaction.SetActive(true);
        InteractIsFinished = false;

        callback?.Invoke();
    }


    public void AbortInteract(Action callback)
    {
        if (InteractIsFinished)
        {
            callback?.Invoke();
        }
        else
        {
            _animateHandler.PlayAbortInteract(
                        _interaction.AbortClips,
                        () => OnAnimationFinished(callback)
            );
        }
    }
}