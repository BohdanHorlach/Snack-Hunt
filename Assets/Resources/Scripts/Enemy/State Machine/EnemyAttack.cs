using System;
using UnityEngine;


public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private ObjectFinderByMask _playerFinderInAttackSpace;
    [SerializeField] private ObjectFinderByMask _spaceNearby;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private NoiseDetecter _noiseDetecter;
    [SerializeField] private float _cooldownAttack = 1f;
    [SerializeField] private float _timeToLosePlayer = 8f;

    private Vector3 _playerPosition;
    private Vector3 _noisePosition;
    private bool _isLostOfVisibility = false;
    private bool _isNoisePositionChecked = false;
    private bool _isSearchBeforeLost = false;
    private bool _isNeedAttacking = false;

    public bool IsAttaking { get; private set; } = false;

    public Action OnAttack;
    public Action OnBeforeLost;
    public Action OnBackToAttack;
    public Action OnLost;
    

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
        if (_isNeedAttacking == false)
            return;

        MoveToTarget();
        CheckNoisePosition();
        if (_playerFinderInAttackSpace.IsHaveObjectInSpace)
            Attack();
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
    }


    private void SetPlayerPosition(Vector3 position)
    {
        if (_isSearchBeforeLost)
            OnBackToAttack?.Invoke();

        _playerPosition = position;
        _isSearchBeforeLost = false;
    }


    private void DetectPlayer(Vector3 position)
    {
        //Debug.Log("SetPlayerPosition");

        SetPlayerPosition(position);
        _isLostOfVisibility = false;
    }


    private void OnLosOfVisibility(Vector3 position)
    {
        SetPlayerPosition(position);
        _isLostOfVisibility = true;
    }


    private void MoveToTarget()
    {
        Vector3 position = _isLostOfVisibility ? _noisePosition : _playerPosition;

        _enemyMovement.SetTarget(position);
    }


    private void LostPlayer()
    {
        if (_isNoisePositionChecked && _isLostOfVisibility)
            OnLost?.Invoke();
    }


    private void CheckNoisePosition()
    {
        if (_isNoisePositionChecked)
            return;

        if (Vector3.Distance(_enemyMovement.transform.position, _noisePosition) <= 0.1f)
        {
            _isNoisePositionChecked = true;
            OnBeforeLost?.Invoke();
            _isSearchBeforeLost = true;
            Invoke("LostPlayer", _timeToLosePlayer);
        }
    }


    private void Attack()
    {
        //Debug.Log("Attack");
        if (IsAttaking)
            return;

        IsAttaking = true;
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
}