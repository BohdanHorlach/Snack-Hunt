using System;
using System.Collections.Generic;


public class EnemyState
{
    private Dictionary<EnemyState, Func<bool>> _transitionDictionary;
    private Dictionary<EnemyState, Action> _transitionEvents;
    private List<EnemyState> _addedTransitionStates;


    public EnemyState()
    {
        _transitionDictionary = new Dictionary<EnemyState, Func<bool>>();
        _transitionEvents = new Dictionary<EnemyState, Action>();
        _addedTransitionStates = new List<EnemyState>();
    }


    public void AddTransition(EnemyState state, Func<bool> condition, Action onTransition = null)
    {
        _transitionDictionary.Add(state, condition);
        _addedTransitionStates.Add(state);

        if (onTransition != null)
            _transitionEvents.Add(state, onTransition);
    }


    public bool CheckTransition(EnemyState nextState)
    {
        if(_transitionDictionary.ContainsKey(nextState))
            return _transitionDictionary[nextState].Invoke();

        return false;
    }


    public void InvokeTransitionEvent(EnemyState nextState)
    {
        _transitionEvents[nextState]?.Invoke();
    }
}