using System;
using UnityEngine;


public class EnemyAnimateHandler : PausedObject
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private Patroller _patroller;
    [SerializeField] private SearcherInSpace _searcherInSpace;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private NPCMovement _enemyMovement;

    public Action OnAttack;

    private void OnEnable()
    {
        _enemyAttack.OnAttack += PlayAttack;
        _enemyAttack.OnBeforeLost += PlaySearch;
        _enemyAttack.OnBackToAttack += DetectPlayer;
        _searcherInSpace.OnReachingWaypoint += PlaySearch;
        _enemyStateMachine.OnSleepToSearch += PlayStandUp;
        _enemyStateMachine.OnTransitionSearchToAttack += DetectPlayer;
    }


    private void OnDisable()
    {
        _enemyAttack.OnAttack -= PlayAttack;
        _enemyAttack.OnBeforeLost -= PlaySearch;
        _enemyAttack.OnBackToAttack -= DetectPlayer;
        _searcherInSpace.OnReachingWaypoint -= PlaySearch;
        _enemyStateMachine.OnSleepToSearch -= PlayStandUp;
        _enemyStateMachine.OnTransitionSearchToAttack -= DetectPlayer;
    }


    private void Update()
    {
        _animator.SetBool("IsPatrolling", _enemyStateMachine.IsPatrolling);
        _animator.SetBool("IsMoving", _enemyMovement.IsReachedDestination == false);
        _animator.SetBool("IsSprinting", _enemyMovement.IsSprinting);
        _animator.SetBool("IsCreepWalking", _enemyMovement.IsCreepWalking);
    }


    private void DetectPlayer()
    {
        _animator.SetTrigger("Detect");
    }


    private void PlayAttack()
    {
        _animator.SetTrigger("Attack");
    }


    private void PlayStandUp()
    {
        _animator.SetTrigger("StandUp");
    }


    //Calls from Animator
    private void Attack()
    {
        OnAttack?.Invoke();
    }


    private void PlaySearch()
    {
        if(_enemyAttack.IsAttaking == false)
            _animator.SetTrigger("Search");
    }


    public override void Pause()
    {
        _animator.enabled = false;
    }


    public override void Resume()
    {
        _animator.enabled = true;
    }
}