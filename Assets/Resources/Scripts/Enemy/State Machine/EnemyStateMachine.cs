using System;
using UnityEngine;


public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private Patroller _patroller;
    [SerializeField] private SearcherInSpace _searcherInSpace;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private VisibilityByPlayer _visibilityByPlayer;
    [SerializeField] private NoiseDetecter _noiseDetect;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private float _distanceForSprint = 30f;
    [SerializeField] private float _distanceForCreepWalk = 10f;

    private const int COUNT_STATES = 4;

    private EnemyState[] _enemyStates;
    private EnemyState _currentState;
    private bool _isDetectedNoise = false;
    private bool _isSearchingEnd = false;
    private bool _isPlayerDetected = false;
    private bool _isWasNotised = false;

    private bool IsSleeped => _currentState == _enemyStates[(int)EnemyStateType.Sleep];
    private bool IsAttaking => _currentState == _enemyStates[(int)EnemyStateType.Attack];
    public bool IsPatrolling => _currentState == _enemyStates[(int)EnemyStateType.Patrol];


    public Action OnSleepToSearch;
    public Action OnTransitionSearchToAttack;


    private void Awake()
    {
        InitializeStatesArray();
        InitializeStates();

        _currentState = _enemyStates[(int)EnemyStateType.Sleep];
    }


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += NoiseDetect;
        _playerDetecter.OnPlayerDetect += PlayerDetect;
        _enemyAttack.OnLost += PlayerLost;
        _searcherInSpace.OnSearchEnd += SearchEnd;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= NoiseDetect;
        _playerDetecter.OnPlayerDetect -= PlayerDetect;
        _enemyAttack.OnLost -= PlayerLost;
        _searcherInSpace.OnSearchEnd -= SearchEnd;
    }


    private void Update()
    {
        SwitchState();

        if (IsSleeped)
            return;

        if (IsAttaking)
            CheckIfWasNoticed();

        SwitchSpeedMode();
    }


    private void InitializeStatesArray()
    {
        _enemyStates = new EnemyState[COUNT_STATES];

        for (int i = 0; i < COUNT_STATES; i++)
            _enemyStates[i] = new EnemyState();
    }


    private void InitializeStates()
    {
        AddTransition(EnemyStateType.Sleep, EnemyStateType.Search, () => _isDetectedNoise, SleepToSearch);
        AddTransition(EnemyStateType.Search, EnemyStateType.Patrol, () => _isSearchingEnd && !_isPlayerDetected, SearhToPatrol);
        AddTransition(EnemyStateType.Search, EnemyStateType.Attack, () => !_isSearchingEnd && _isPlayerDetected, SearchToAttack);
        AddTransition(EnemyStateType.Patrol, EnemyStateType.Attack, () => _isPlayerDetected, PatrolToAttack);
        AddTransition(EnemyStateType.Patrol, EnemyStateType.Search, () => _isDetectedNoise, PatrolToSearch);
        AddTransition(EnemyStateType.Attack, EnemyStateType.Search, () => !_isPlayerDetected, AttackToSearch);
    }


    private void AddTransition(EnemyStateType from, EnemyStateType to, Func<bool> condition, Action onTransition)
    {
        EnemyState currentState = _enemyStates[(int)from];
        EnemyState nextState = _enemyStates[(int)to];

        currentState.AddTransition(nextState, condition, onTransition);
    }


    private void NoiseDetect(Vector3 position)
    {
        _isDetectedNoise = true;
    }


    private void PlayerDetect(Vector3 position)
    {
        _isPlayerDetected = true;
    }


    private void PlayerLost()
    {
        Debug.Log("PlayerLost");
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
            _isWasNotised = true;
    }


    private void SwitchSpeedMode()
    {
        float distance = _enemyMovement.GetDistanceToDestination();

        EnemyMovementMode movementMode;
        
        if (distance >= _distanceForSprint || _isWasNotised)
            movementMode = EnemyMovementMode.Sprint;
        else if (IsAttaking && distance <= _distanceForCreepWalk && _isWasNotised == false)
            movementMode = EnemyMovementMode.CreepWalk;
        else
            movementMode = EnemyMovementMode.Default;

        _enemyMovement.SetMovementMode(movementMode);
    }


    private void SwitchState()
    {
        for (int i = 0; i < COUNT_STATES; i++)
        {
            EnemyState enemyState = _enemyStates[i];

            if (_currentState.CheckTransition(enemyState))
            {
                _currentState.InvokeTransitionEvent(enemyState);
                _currentState = enemyState;
                break;
            }
        }
    }


    private void SleepToSearch()
    {
        _isSearchingEnd = false;
        _searcherInSpace.StartSearch();
        OnSleepToSearch?.Invoke();
    }


    private void SearhToPatrol()
    {
        _patroller.StartSearch();
    }


    private void SearchToAttack()
    {
        _searcherInSpace.StopSearch();
        _enemyAttack.GoAttack();

        OnTransitionSearchToAttack?.Invoke();
    }


    private void PatrolToAttack()
    {
        Debug.Log("OnSearchToAttack");
        _patroller.StopSearch();
        _enemyAttack.GoAttack();

        OnTransitionSearchToAttack?.Invoke();
    }


    private void PatrolToSearch()
    {
        _patroller.StopSearch();
        _searcherInSpace.StartSearch();
        _isSearchingEnd = false;
    }


    private void AttackToSearch()
    {
        Debug.Log("OnAttackToSearch");
        
        _enemyAttack.StopAttack();
        _patroller.StartSearch();
        _isWasNotised = false;
    }
}