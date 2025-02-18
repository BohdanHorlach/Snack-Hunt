using System;
using UnityEngine;


public class PlayerDetecter : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _rayPosition;
    [SerializeField] private float _timeForLoseAPlayer = 20f;
    [SerializeField] private float _rayDistance = 10f;
    [SerializeField] private float _searchForwardRaysAngle = 50f;
    [SerializeField] private int _rayCount = 7;
    [SerializeField, Range(0, 1)] private float _dotThreshold = 0.1f;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private LayerMask _obstacleMask;

    private bool _playerPreviouslyDetected = false;
    private bool _isLost;

    public Action<Vector3> OnPlayerDetect;
    public Action OnPlayerLost;


    private void Update()
    {
        Search();
    }


    private bool SearchByDotProduct()
    {
        Vector3 directionFromPlayerToEnemy = _player.position - transform.position;
        float dotPruduct = Vector3.Dot(transform.forward, directionFromPlayerToEnemy.normalized);

        return dotPruduct >= _dotThreshold;
    }


    private float GetDistanceToObjectFromRay(Vector3 position, Vector3 direction, float distance, LayerMask objectMask)
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(position, direction, out hit, distance, objectMask);

        return isHit ? hit.distance : float.MaxValue;
    }


    private bool SearchByRays()
    {
        Vector3 direction = _player.position - _rayPosition.position;
        float angleStep = _searchForwardRaysAngle / (_rayCount - 1);

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = -_searchForwardRaysAngle / 2f + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;
            float distanceToPlayer = GetDistanceToObjectFromRay(_rayPosition.position, rayDirection, _rayDistance, _playerMask);
            float distanceToObstacle = GetDistanceToObjectFromRay(_rayPosition.position, rayDirection, _rayDistance, _obstacleMask);

            if (distanceToPlayer < distanceToObstacle)
            {
                Debug.Log("PlayerDetected");
                return true;
            }
        }

        return false;
    }


    private void OnLost()
    {
        if (_playerPreviouslyDetected == false)
            OnPlayerLost?.Invoke();

        _isLost = false;
    }


    private void Detect()
    {
        OnPlayerDetect?.Invoke(_player.position);
    }


    private void Lost()
    {
        _isLost = true;
        Invoke("OnLost", _timeForLoseAPlayer);
    }


    private void CancleLost()
    {
        if (_isLost == false)
            return;

        _isLost = false;
        CancelInvoke("OnLost");
    }


    private void Search()
    {
        bool playerDetected = SearchByDotProduct() && SearchByRays();

        if (playerDetected)
        {
            Detect();
            CancleLost();
        }
        else if (_playerPreviouslyDetected)
        {
            Lost();
        }

        _playerPreviouslyDetected = playerDetected;
    }



#region Visualization
    private void OnDrawGizmos()
    {
        DrawFieldOfView();
        DrawRay();
    }


    private void DrawFieldOfView()
    {
        Gizmos.color = new Color(0, 0, 1, 1f);
        Vector3 forward = _rayPosition.forward * _rayDistance;

        float angle = Mathf.Acos(_dotThreshold) * Mathf.Rad2Deg;
        Vector3 rightLimit = Quaternion.Euler(0, angle, 0) * forward;
        Vector3 leftLimit = Quaternion.Euler(0, -angle, 0) * forward;

        Gizmos.DrawLine(_rayPosition.position, _rayPosition.position + rightLimit);
        Gizmos.DrawLine(_rayPosition.position, _rayPosition.position + leftLimit);
    }


    private void DrawRay()
    {
        Vector3 direction = _player.position - _rayPosition.position;
        float angleStep = _searchForwardRaysAngle / (_rayCount - 1);

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = -_searchForwardRaysAngle / 2f + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

            Debug.DrawRay(_rayPosition.position, rayDirection.normalized * _rayDistance, Color.red);
        }
    }
#endregion
}