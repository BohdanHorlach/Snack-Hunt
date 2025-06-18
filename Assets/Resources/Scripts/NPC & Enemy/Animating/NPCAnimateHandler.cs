using UnityEngine;


public class NPCAnimateHandler : MonoBehaviour
{
    [SerializeField] private NPCMovement _movement;
    [SerializeField] private HeadRotator _headRotator;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _reactAnimator;

    private Vector3? _playerPosition;
    private bool _headRotateCalled = false;


    private void OnEnable()
    {
        _playerDetecter.OnPlayerDetect += SetPlayerPosition;
        _playerDetecter.OnPlayerLost += ResetPlayerPosition;
    }


    private void OnDisable()
    {
        _playerDetecter.OnPlayerDetect -= SetPlayerPosition;
        _playerDetecter.OnPlayerLost -= ResetPlayerPosition;
    }


    private void Update()
    {
        float magnitude = _movement.Velocity.magnitude;
        float normalizedVelocity = Mathf.Clamp01(magnitude / _movement.CurrentSpeed);

        _animator.SetFloat("Velocity", normalizedVelocity, 0.1f, Time.deltaTime);

        if (_playerPosition.HasValue && _headRotateCalled != true)
        {
            _headRotateCalled = true;
            _headRotator.RotateHeadTowards(_playerPosition.Value);
            _reactAnimator.ResetTrigger("Rewind");
            _reactAnimator.SetTrigger("Detect");
        }
    }


    private void SetPlayerPosition(Vector3 position)
    {
        _playerPosition = position;
    }


    private void ResetPlayerPosition(Vector3 position)
    {
        _playerPosition = null;
        _headRotateCalled = false;
        _headRotator.LostTarget();
        _reactAnimator.SetTrigger("Rewind");
    }
}