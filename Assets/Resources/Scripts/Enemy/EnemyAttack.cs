using System;
using UnityEngine;


public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private ObjectFinderByMask _playerFinderInAttackSpace;
    [SerializeField] private ObjectFinderByMask _spaceNearby;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private float _cooldown = 3f;

    private Vector3 _playerPosition;
    private bool _isNeedAttacking = false;

    public bool IsAttaking { get; private set; } = false;

    public Action OnAttack;
    

    private void OnEnable()
    {
        _playerDetecter.OnPlayerDetect += SetPlayerPosition;
        _playerFinderInAttackSpace.OnObjectDetect += Attack;
        _spaceNearby.OnObjectDetect += DetectOnNearby;
        _spaceNearby.OnObjectLost += DetectOnNearby;
    }


    private void OnDisable()
    {
        _playerDetecter.OnPlayerDetect -= SetPlayerPosition;
        _playerFinderInAttackSpace.OnObjectDetect -= Attack;
        _spaceNearby.OnObjectDetect -= DetectOnNearby;
        _spaceNearby.OnObjectLost -= DetectOnNearby;
    }



    private void Update()
    {
        if (_isNeedAttacking == false || IsAttaking)
            return;

        _enemyMovement.SetTarget(_playerPosition);
    }



    private void DetectOnNearby(Transform transform)
    {
        _playerPosition = transform.position;
    }


    private void ResetAttack()
    {
        Debug.Log("ResetAttack");
        IsAttaking = false;
    }


    private void SetPlayerPosition(Vector3 position)
    {
        Debug.Log("SetPlayerPosition");

        _playerPosition = position;
    }


    private void Attack(Transform transform)
    {
        Debug.Log("Attack");
        IsAttaking = true;
        OnAttack?.Invoke();
        Invoke("ResetAttack", _cooldown);
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