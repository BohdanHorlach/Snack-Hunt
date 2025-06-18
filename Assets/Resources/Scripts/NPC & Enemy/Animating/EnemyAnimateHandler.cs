using System;
using UnityEngine;


public class EnemyAnimateHandler : MonoBehaviour, IPaused, IStateRewind
{
    private struct AnimatorSnapshot
    {
        public int SavedStateHash;
        public float SavedNormalizedTime;
    }


    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyStateMachine _enemyStateMachine;
    [SerializeField] private Patroller _patroller;
    [SerializeField] private SearcherInSpace _searcherInSpace;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private EnemyAttack _enemyAttack;
    [SerializeField] private NPCMovement _enemyMovement;

    private StateRecorder<AnimatorSnapshot> _stateRecorder;

    public Action OnAttack;


    private void Awake()
    {
        _stateRecorder = new StateRecorder<AnimatorSnapshot>(GetSnapshot, ApplySnapshot);
        _enemyMovement.CanMove = false;
    }


    private void OnEnable()
    {
        _enemyAttack.OnAttack += PlayAttack;
        _enemyAttack.OnBeforeLost += PlaySearch;
        _searcherInSpace.OnReachingWaypoint += PlaySearch;
        _enemyStateMachine.OnSleepToSearch += PlayStandUp;
    }


    private void OnDisable()
    {
        _enemyAttack.OnAttack -= PlayAttack;
        _enemyAttack.OnBeforeLost -= PlaySearch;
        _searcherInSpace.OnReachingWaypoint -= PlaySearch;
        _enemyStateMachine.OnSleepToSearch -= PlayStandUp;
    }


    private void Update()
    {
        if (PauseHandler.IsPaused)
            return;

        _animator.SetBool("IsPatrolling", _enemyStateMachine.IsPatrolling);
        _animator.SetBool("IsMoving", _enemyMovement.IsReachedDestination == false);
        _animator.SetBool("IsSprinting", _enemyMovement.IsSprinting);
        _animator.SetBool("IsCreepWalking", _enemyMovement.IsCreepWalking);
        _animator.SetBool("IsDetect", _playerDetecter.IsDetect);
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
        if (_enemyAttack.IsAttaking == false)
            _animator.SetTrigger("Search");
    }



    //Calls from Animator
    private void Attack()
    {
        OnAttack?.Invoke();
    }


    //Calls from Animator
    private void StartMove()
    {
        _enemyMovement.CanMove = true;
    }


    public void Pause()
    {
        _animator.enabled = false;
    }


    public void Resume()
    {
        _animator.enabled = true;
    }


    private AnimatorSnapshot GetSnapshot()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        return new AnimatorSnapshot
        {
            SavedStateHash = stateInfo.shortNameHash,
            SavedNormalizedTime = stateInfo.normalizedTime
        };
    }


    private void ApplySnapshot(AnimatorSnapshot snapshot)
    {
        _animator.Play(snapshot.SavedStateHash, 0, snapshot.SavedNormalizedTime);
    }


    public void Record(bool needRemove) { _stateRecorder.Record(needRemove); }

    public void Rewind() { _stateRecorder.Rewind(); }
}