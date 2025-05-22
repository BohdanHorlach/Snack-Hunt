using System;
using UnityEngine;


public class NPCStateMachine : MonoBehaviour
{
    [SerializeField] private NPCMovement _enemyMovement;
    [SerializeField] private NPCInteractivePatroller _patroller;
    [SerializeField] private SearcherInSpace _searcherInSpace;
    [SerializeField] private NPCInteraction _interaction;
    [SerializeField] private NoiseDetecter _noiseDetect;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private Transform _startPosition;
    [SerializeField] private bool _needInteraction;

    private const int COUNT_STATES = 3;

    private NPCState[] _enemyStates;
    private NPCState _currentState;
    private bool _isDetectedNoise = false;
    private bool _isSearchingEnd = false;

    private bool IsIdle;// => _currentState == _enemyStates[(int)NPCStateType.Idle];
    private bool IsSearch;
    public bool IsPatrolling;// => _currentState == _enemyStates[(int)NPCStateType.Patrol];

    public Action OnSleepToSearch;
    public Action OnTransitionSearchToAttack;
    public Action OnDetectPlayer;


    private void Awake()
    {
        InitializeStatesArray();
        InitializeStates();

        _currentState = _enemyStates[(int)NPCStateType.Idle];
    }


    private void Start()
    {
        _enemyMovement.SetTarget(_startPosition.position);
    }


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += NoiseDetect;
        _playerDetecter.OnPlayerDetect += DetectPlayer;
        _searcherInSpace.OnSearchEnd += SearchEnd;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= NoiseDetect;
        _playerDetecter.OnPlayerDetect -= DetectPlayer;
        _searcherInSpace.OnSearchEnd -= SearchEnd;
    }


    private void Update()
    {
        IsIdle = _currentState == _enemyStates[(int)NPCStateType.Idle];
        IsSearch = _currentState == _enemyStates[(int)NPCStateType.Search];
        IsPatrolling = _currentState == _enemyStates[(int)NPCStateType.Patrol];

        SwitchState();
    }


    private void InitializeStatesArray()
    {
        _enemyStates = new NPCState[COUNT_STATES];

        for (int i = 0; i < COUNT_STATES; i++)
            _enemyStates[i] = new NPCState();
    }


    private void InitializeStates()
    {
        AddTransition(NPCStateType.Idle, NPCStateType.Search, () => _isDetectedNoise, IdleToSearch);
        AddTransition(NPCStateType.Idle, NPCStateType.Patrol, () => _needInteraction, ToPatroll);
        AddTransition(NPCStateType.Search, NPCStateType.Idle, () => _isSearchingEnd && !_needInteraction, SearhToIdle);
        AddTransition(NPCStateType.Search, NPCStateType.Patrol, () => _isSearchingEnd && _needInteraction, ToPatroll);
        AddTransition(NPCStateType.Patrol, NPCStateType.Search, () => _isDetectedNoise, PatrolToSearch);
    }


    private void AddTransition(NPCStateType from, NPCStateType to, Func<bool> condition, Action onTransition)
    {
        NPCState currentState = _enemyStates[(int)from];
        NPCState nextState = _enemyStates[(int)to];

        currentState.AddTransition(nextState, condition, onTransition);
    }


    private void NoiseDetect(Vector3 position)
    {
        _isDetectedNoise = true;
    }


    private void DetectPlayer(Vector3 position)
    {
        OnDetectPlayer?.Invoke();
    }


    private void SearchEnd()
    {
        _isSearchingEnd = true;
        _isDetectedNoise = false;
    }


    private void SwitchState()
    {
        for (int i = 0; i < COUNT_STATES; i++)
        {
            NPCState enemyState = _enemyStates[i];

            if (_currentState.CheckTransition(enemyState))
            {
                _currentState.InvokeTransitionEvent(enemyState);
                _currentState = enemyState;
                break;
            }
        }
    }


    private void IdleToSearch()
    {
        //Debug.Log("IdleToSearch");
        _interaction.AbortInteract(() =>
        {
            _searcherInSpace.StartSearch();
            OnSleepToSearch?.Invoke();
        });

        _isSearchingEnd = false;
    }


    private void ToPatroll()
    {
        _patroller.StartPatrolling();
    }


    private void SearhToIdle()
    {
        //Debug.Log("OnSearhToIdle");

        _enemyMovement.SetTarget(_startPosition.position);
    }


    private void PatrolToSearch()
    {
        //Debug.Log("OnPatrolToSearch");
        _isSearchingEnd = false;

        _patroller.StopPatrolling(() =>
        {
            _searcherInSpace.StartSearch();
            OnSleepToSearch?.Invoke();
        });
    }
}