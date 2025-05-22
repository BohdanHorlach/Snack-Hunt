using System;
using UnityEngine;


public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private NPCMovement _enemyMovement;
    [SerializeField] private Patroller _patroller;
    [SerializeField] private SearcherInSpace _searcherInSpace;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private VisibilityByPlayer _visibilityByPlayer;
    [SerializeField] private NoiseDetecter _noiseDetect;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private float _distanceForSprint = 30f;
    [SerializeField] private float _distanceForCreepWalk = 10f;

    private const int COUNT_STATES = 4;

    private NPCState[] _enemyStates;
    private NPCState _currentState;
    private bool _isDetectedNoise = false;
    private bool _isSearchingEnd = false;
    private bool _isPlayerDetected = false;
    private bool _isWasDetected = false;

    private bool IsIdle => _currentState == _enemyStates[(int)NPCStateType.Idle];
    private bool IsAttaking => _currentState == _enemyStates[(int)NPCStateType.Attack];
    public bool IsSearching => _currentState == _enemyStates[(int)NPCStateType.Search];
    public bool IsPatrolling => _currentState == _enemyStates[(int)NPCStateType.Patrol];


    public Action OnSleepToSearch;
    public Action OnTransitionSearchToAttack;


    private void Awake()
    {
        InitializeStatesArray();
        InitializeStates();

        _currentState = _enemyStates[(int)NPCStateType.Idle];
    }


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += NoiseDetect;
        _playerDetecter.OnPlayerDetect += DetectPlayer;
        _enemyAttack.OnLost += LostPlayer;
        _searcherInSpace.OnSearchEnd += SearchEnd;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= NoiseDetect;
        _playerDetecter.OnPlayerDetect -= DetectPlayer;
        _enemyAttack.OnLost -= LostPlayer;
        _searcherInSpace.OnSearchEnd -= SearchEnd;
    }


    private void Update()
    {
        Debug.Log($"IsAttaking: {IsAttaking}");
        Debug.Log($"IsPatrolling: {IsPatrolling}");
        Debug.Log($"IsSearching: {IsSearching}");

        SwitchState();

        if (IsIdle)
            return;

        if (IsAttaking)
            CheckIfWasNoticed();

        SwitchSpeedMode();
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
        AddTransition(NPCStateType.Search, NPCStateType.Patrol, () => _isSearchingEnd && !_isPlayerDetected, SearhToPatrol);
        AddTransition(NPCStateType.Search, NPCStateType.Attack, () => !_isSearchingEnd && _isPlayerDetected, SearchToAttack);
        AddTransition(NPCStateType.Patrol, NPCStateType.Attack, () => _isPlayerDetected, PatrolToAttack);
        AddTransition(NPCStateType.Patrol, NPCStateType.Search, () => _isDetectedNoise, PatrolToSearch);
        AddTransition(NPCStateType.Attack, NPCStateType.Search, () => !_isPlayerDetected, AttackToSearch);
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
        _isPlayerDetected = true;
    }


    private void LostPlayer()
    {
        //Debug.Log("PlayerLost");
        _isPlayerDetected = false;
    }


    private void SearchEnd()
    {
        _isSearchingEnd = true;
        _isDetectedNoise = false;
    }


    private void CheckIfWasNoticed()
    {
        if (_visibilityByPlayer.IsVisible == true)
            _isWasDetected = true;
    }


    private void SwitchSpeedMode()
    {
        float distance = _enemyMovement.GetDistanceToDestination();

        NPCMovementMode movementMode;
        
        if (distance >= _distanceForSprint || _isWasDetected)
            movementMode = NPCMovementMode.Sprint;
        else if (IsAttaking && distance <= _distanceForCreepWalk && _isWasDetected == false)
            movementMode = NPCMovementMode.CreepWalk;
        else
            movementMode = NPCMovementMode.Default;

        _enemyMovement.SetMovementMode(movementMode);
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
        _isSearchingEnd = false;
        _searcherInSpace.StartSearch();
        OnSleepToSearch?.Invoke();
    }


    private void SearhToPatrol()
    {
        //Debug.Log("OnSearhToPatrol");

        _patroller.StartSearch();
    }


    private void SearchToAttack()
    {
        //Debug.Log("OnSearchToAttack");

        _searcherInSpace.StopSearch();
        _enemyAttack.GoAttack();

        OnTransitionSearchToAttack?.Invoke();
    }


    private void PatrolToAttack()
    {
        //Debug.Log("OnSearchToAttack");
        _patroller.StopSearch();
        _enemyAttack.GoAttack();

        OnTransitionSearchToAttack?.Invoke();
    }


    private void PatrolToSearch()
    {
        //Debug.Log("OnPatrolToSearch");

        _patroller.StopSearch();
        _searcherInSpace.StartSearch();
        _isSearchingEnd = false;
    }


    private void AttackToSearch()
    {
        //Debug.Log("OnAttackToSearch");
        
        _enemyAttack.StopAttack();
        _patroller.StartSearch();
        _isWasDetected = false;
        LostPlayer();
    }
}