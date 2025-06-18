using System;
using UnityEngine;


public class EnemyAttack : MonoBehaviour, IStateRewind
{
    private struct EnemyAttackSnapshot
    {
        public Vector3 NoisePosition;
        public bool IsLostOfVisibility;
        public bool IsNoisePositionChecked;
        public bool IsLastPLayerPositionCheked;
        public bool IsSearchBeforeLost;
        public bool IsNeedAttacking;
        public bool IsAttaking;
    }


    [SerializeField] private NPCMovement _enemyMovement;
    [SerializeField] private ObjectFinderByMask _playerFinderInAttackSpace;
    [SerializeField] private ObjectFinderByMask _spaceNearby;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private NoiseDetecter _noiseDetecter;
    [SerializeField] private float _cooldownAttack = 1f;
    [SerializeField] private float _distanceThreshold = 1.5f;
    [SerializeField] private float _timeToLosePlayer = 8f;


    private StateRecorder<EnemyAttackSnapshot> _stateRecorder;
    private Vector3 _playerPosition;
    private Vector3 _noisePosition;
    private bool _isLostOfVisibility = false;
    private bool _isNoisePositionChecked = false;
    private bool _isLastPLayerPositionCheked = false;
    private bool _isSearchBeforeLost = false;
    private bool _isNeedAttacking = false;

    public bool IsAttaking { get; private set; } = false;

    public Action OnAttack;
    public Action OnBeforeLost;
    public Action OnBackToAttack;
    public Action OnLost;


    private void Awake()
    {
        _stateRecorder = new StateRecorder<EnemyAttackSnapshot>(GetSnapshot, ApplySnapshot);
    }


    private void OnEnable()
    {
        _playerDetecter.OnPlayerDetect += DetectPlayer;
        _playerDetecter.OnPlayerLost += OnLosOfVisibility;
        _spaceNearby.OnObjectDetect += DetectOnNearby;
        _spaceNearby.OnObjectLost += DetectOnNearby;
        _noiseDetecter.OnNoiseDetect += NoiseDetect;
    }


    private void OnDisable()
    {
        _playerDetecter.OnPlayerDetect -= DetectPlayer;
        _playerDetecter.OnPlayerLost -= OnLosOfVisibility;
        _spaceNearby.OnObjectDetect -= DetectOnNearby;
        _spaceNearby.OnObjectLost -= DetectOnNearby;
        _noiseDetecter.OnNoiseDetect -= NoiseDetect;
    }



    private void Update()
    {
        if (_isNeedAttacking == false || PauseHandler.IsPaused)
            return;

        MoveToTarget();

        if (_playerFinderInAttackSpace.IsHaveObjectInSpace)
        {
            Attack();
        }

        if (_isLostOfVisibility)
        {
            CheckLastPosition(_noisePosition, ref _isNoisePositionChecked);
            CheckLastPosition(_playerPosition, ref _isLastPLayerPositionCheked);
        }
    }



    private void DetectOnNearby(Transform transform)
    {
        _playerPosition = transform.position;
    }


    private void NoiseDetect(Vector3 position)
    {
        _noisePosition = position;
        _isNoisePositionChecked = false;
    }


    private void ResetAttack()
    {
        //Debug.Log("ResetAttack");
        IsAttaking = false;
        _enemyMovement.CanMove = true;
    }


    private void SetPlayerPosition(Vector3 position)
    {
        if (_isSearchBeforeLost)
            OnBackToAttack?.Invoke();

        _playerPosition = position;
        _isSearchBeforeLost = false;
        _isLastPLayerPositionCheked = false;
    }


    private void DetectPlayer(Vector3 position)
    {
        //Debug.Log("SetPlayerPosition");

        SetPlayerPosition(position);
        _isLostOfVisibility = false;
        _isNoisePositionChecked = true;
    }


    private void OnLosOfVisibility(Vector3 position)
    {
        SetPlayerPosition(position);
        _isLostOfVisibility = true;
    }


    private void MoveToTarget()
    {
        Vector3 position = _isLastPLayerPositionCheked && !_isNoisePositionChecked ? _noisePosition : _playerPosition;

        _enemyMovement.SetTarget(position);
    }


    private void LostPlayer()
    {
        if (_isNoisePositionChecked && _isLostOfVisibility)
            OnLost?.Invoke();
    }


    private void CheckLastPosition(Vector3 position, ref bool condition)
    {
        if (Vector3.Distance(_enemyMovement.transform.position, position) <= _distanceThreshold)
        {
            if (_isLostOfVisibility)
            {
                OnBeforeLost?.Invoke();
                _isSearchBeforeLost = true;
                Invoke("LostPlayer", _timeToLosePlayer);
            }

            condition = true;
        }
    }


    private void Attack()
    {
        //Debug.Log("Attack");
        if (IsAttaking)
            return;

        IsAttaking = true;
        _enemyMovement.CanMove = false;
        OnAttack?.Invoke();
        _enemyMovement.Dash(_playerPosition);
        Invoke("ResetAttack", _cooldownAttack);
    }


    public void GoAttack()
    {
        _isNeedAttacking = true;
    }


    public void StopAttack()
    {
        _isNeedAttacking = false;
    }


    private EnemyAttackSnapshot GetSnapshot()
    {
        return new EnemyAttackSnapshot
        {
            NoisePosition = _noisePosition,
            IsLostOfVisibility = _isLostOfVisibility,
            IsNoisePositionChecked = _isNoisePositionChecked,
            IsLastPLayerPositionCheked = _isLastPLayerPositionCheked,
            IsSearchBeforeLost = _isSearchBeforeLost,
            IsNeedAttacking = _isNeedAttacking,
            IsAttaking = this.IsAttaking
        };
    }


    private void ApplySnapshot(EnemyAttackSnapshot snapshot)
    {
        _noisePosition = snapshot.NoisePosition;
        _isLostOfVisibility = snapshot.IsLostOfVisibility;
        _isNoisePositionChecked = snapshot.IsNoisePositionChecked;
        _isLastPLayerPositionCheked = snapshot.IsLastPLayerPositionCheked;
        _isSearchBeforeLost = snapshot.IsSearchBeforeLost;
        _isNeedAttacking = snapshot.IsNeedAttacking;
        IsAttaking = snapshot.IsAttaking;
    }


    public void Record(bool needRemove) { _stateRecorder.Record(needRemove); }

    public void Rewind() { _stateRecorder.Rewind(); }
}