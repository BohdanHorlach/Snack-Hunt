using UnityEngine;


public class NPCAnimateHandler : MonoBehaviour
{
    [SerializeField] private NPCMovement _movement;
    [SerializeField] private HeadRotator _headRotator;
    [SerializeField] private PlayerDetecter _playerDetecter;
    [SerializeField] private Animator _animator;

    private Vector3? _playerPosition;


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

        if (_playerPosition.HasValue)
            _headRotator.RotateHeadTowards(_playerPosition.Value);
    }


    private void SetPlayerPosition(Vector3 position)
    {
        _playerPosition = position;
    }


    private void ResetPlayerPosition(Vector3 position)
    {
        _playerPosition = null;
        _headRotator.LostTarget();
    }
}