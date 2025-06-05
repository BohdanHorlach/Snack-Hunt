using UnityEngine;


public class ObjectSearcher
{
    private SearchSettings _searchSettings;


    public ObjectSearcher(SearchSettings searchSettings)
    {
        _searchSettings = searchSettings;
    }


    private float GetDistanceToObjectFromRay(Vector3 position, Vector3 direction, float distance, LayerMask objectMask)
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(position, direction, out hit, distance, objectMask);

        return isHit ? hit.distance : float.MaxValue;
    }


    public bool SearchByRays(Transform target)
    {
        Vector3 rayPosition = _searchSettings.rayPosition.position;
        Vector3 direction = (target.position - rayPosition).normalized;
        float maxDistance = _searchSettings.rayDistance;
        float halfAngle = _searchSettings.searchForwardRaysAngle / 2f;
        float angleStep = _searchSettings.searchForwardRaysAngle / (_searchSettings.rayCount - 1);
        
        for (int i = 0; i < _searchSettings.rayCount; i++)
        {
            float angle = -halfAngle + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

            float targetDistance = GetDistanceToObjectFromRay(rayPosition, rayDirection, maxDistance, _searchSettings.targetMask);
            float obstacleDistance = GetDistanceToObjectFromRay(rayPosition, rayDirection, maxDistance, _searchSettings.obstacleMask);

            if (targetDistance < obstacleDistance)
                return true;
        }

        return false;
    }


    public bool SearchByDotProduct(Transform transform, Transform target)
    {
        Vector3 forwardByRays = _searchSettings.rayPosition ? _searchSettings.rayPosition.forward : transform.forward;

        Vector3 toTarget = target.position - transform.position;
        float dotPruduct = Vector3.Dot(forwardByRays, toTarget.normalized);

        float verticalAngle = Vector3.Angle(toTarget.normalized, forwardByRays);

        return dotPruduct >= _searchSettings.dotThreshold && verticalAngle <= _searchSettings.searchVerticalAngle;
    }


    #region Visualization

    public void DrawFieldOfView()
    {
        Vector3 rayPosition = _searchSettings.rayPosition.position;
        Vector3 forward = _searchSettings.rayPosition.forward * _searchSettings.rayDistance;

        float angle = Mathf.Acos(_searchSettings.dotThreshold) * Mathf.Rad2Deg;
        Vector3 leftLimit = Quaternion.AngleAxis(-angle, _searchSettings.rayPosition.up) * forward;
        Vector3 rightLimit = Quaternion.AngleAxis(angle, _searchSettings.rayPosition.up) * forward;

        Vector3 upperLimit = Quaternion.AngleAxis(_searchSettings.searchVerticalAngle / 2, _searchSettings.rayPosition.right) * forward;
        Vector3 bottomLimit = Quaternion.AngleAxis(-_searchSettings.searchVerticalAngle / 2, _searchSettings.rayPosition.right) * forward;

        Debug.DrawLine(rayPosition, rayPosition + rightLimit, Color.blue);
        Debug.DrawLine(rayPosition, rayPosition + leftLimit, Color.blue);

        Debug.DrawLine(rayPosition, rayPosition + upperLimit, Color.blue);
        Debug.DrawLine(rayPosition, rayPosition + bottomLimit, Color.blue);
    }


    public void DrawRay(Transform target)
    {
        Vector3 rayPosition = _searchSettings.rayPosition.position;

        Vector3 direction = target.position - rayPosition;
        float angleStep = _searchSettings.searchForwardRaysAngle / (_searchSettings.rayCount - 1);

        for (int i = 0; i < _searchSettings.rayCount; i++)
        {
            float angle = -_searchSettings.searchForwardRaysAngle / 2f + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * direction;

            Debug.DrawRay(rayPosition, rayDirection.normalized * _searchSettings.rayDistance, Color.red);
        }
    }

#endregion
}