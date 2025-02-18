using System;
using UnityEngine;


public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private TargetSeeker _targetSeeker;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private VisibilityByPlayer _visibilityByPlayer;
    [SerializeField] private NoiseDetecter _noiseDetect;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private float _distanceForSprint = 30f;
    [SerializeField] private float _distanceForCreepWalk = 10f;

    private const int COUNT_STATES = 3;

    private EnemyState[] _enemyStates;
    private EnemyState _currentState;
    private bool _isDetectedNoise = false;
    private bool _isPlayerDetected = false;
    private bool _isWasNotised = false;

    private bool IsSleeped => _currentState == _enemyStates[(int)EnemyStateType.Sleep];
    private bool IsAttaking => _currentState == _enemyStates[(int)EnemyStateType.Attack];


    public Action OnTransitionAttackToSearch;


    private void Awake()
    {
        InitializeStatesArray();
        InitializeStates();

        _currentState = _enemyStates[(int)EnemyStateType.Search];
    }


    private void Start()
    {
        OnSleepToSearch();
    }


    private void OnEnable()
    {
        _noiseDetect.OnNoiseDetect += NoiseDetect;
        _playerDetecter.OnPlayerDetect += PlayerDetect;
        _playerDetecter.OnPlayerLost += PlayerLost;
    }


    private void OnDisable()
    {
        _noiseDetect.OnNoiseDetect -= NoiseDetect;
        _playerDetecter.OnPlayerDetect -= PlayerDetect;
        _playerDetecter.OnPlayerLost -= PlayerLost;
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
        AddTransition(EnemyStateType.Sleep, EnemyStateType.Search, () => _isDetectedNoise, OnSleepToSearch);
        AddTransition(EnemyStateType.Search, EnemyStateType.Attack, () => _isPlayerDetected, OnSearchToAttack);
        AddTransition(EnemyStateType.Attack, EnemyStateType.Search, () => _isPlayerDetected == false, OnAttackToSearch);
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


    private void OnSleepToSearch()
    {
        _targetSeeker.StartSearch();
    }


    private void OnSearchToAttack()
    {
        Debug.Log("OnSearchToAttack");
        _targetSeeker.StopSearch();
        _enemyAttack.GoAttack();
    }


    private void OnAttackToSearch()
    {
        Debug.Log("OnAttackToSearch");
        
        _enemyAttack.StopAttack();
        _targetSeeker.StartSearch();
        _isWasNotised = false;

        OnTransitionAttackToSearch?.Invoke();
    }
}