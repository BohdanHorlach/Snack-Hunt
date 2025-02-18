using UnityEngine;


public class EnemyAnimateHandler : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private TargetSeeker _targetSeeker;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private EnemyMovement _enemyMovement;


    private void OnEnable()
    {
        _enemyAttack.OnAttack += PlayAttack;
        _targetSeeker.OnReachingWaypoint += PlaySearch;
        _enemyStateMachine.OnTransitionAttackToSearch += DetectPlayer;
    }


    private void OnDisable()
    {
        _enemyAttack.OnAttack -= PlayAttack;
        _targetSeeker.OnReachingWaypoint -= PlaySearch;
        _enemyStateMachine.OnTransitionAttackToSearch -= DetectPlayer;
    }


    private void Update()
    {
        _animator.SetBool("IsMoving", _enemyMovement.IsMoving);
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


    private void PlaySearch()
    {
        _animator.SetTrigger("Search");
    }
}