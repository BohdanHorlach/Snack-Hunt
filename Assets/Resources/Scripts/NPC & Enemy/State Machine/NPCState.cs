using System;
using System.Collections.Generic;


public class NPCState
{
    private Dictionary<NPCState, Func<bool>> _transitionDictionary;
    private Dictionary<NPCState, Action> _transitionEvents;
    private List<NPCState> _addedTransitionStates;


    public NPCState()
    {
        _transitionDictionary = new Dictionary<NPCState, Func<bool>>();
        _transitionEvents = new Dictionary<NPCState, Action>();
        _addedTransitionStates = new List<NPCState>();
    }


    public void AddTransition(NPCState state, Func<bool> condition, Action onTransition = null)
    {
        _transitionDictionary.Add(state, condition);
        _addedTransitionStates.Add(state);

        if (onTransition != null)
            _transitionEvents.Add(state, onTransition);
    }


    public bool CheckTransition(NPCState nextState)
    {
        if(_transitionDictionary.ContainsKey(nextState))
            return _transitionDictionary[nextState].Invoke();

        return false;
    }


    public void InvokeTransitionEvent(NPCState nextState)
    {
        _transitionEvents[nextState]?.Invoke();
    }
}