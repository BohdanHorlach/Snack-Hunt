using System;
using UnityEngine;


public class PlayerDetecter : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private SearchSettings _searchConfig;

    private ObjectSearcher _objectSearcher;

    public Action<Vector3> OnPlayerDetect;
    public Action<Vector3> OnPlayerLost;


    private void Awake()
    {
        _objectSearcher = new ObjectSearcher(_searchConfig);
    }


    private void Update()
    {
        Search();
    }


    private void Detect()
    {
        OnPlayerDetect?.Invoke(_player.position);
    }


    private void Lost()
    {
        OnPlayerLost?.Invoke(_player.position);
    }


    private void Search()
    {
        bool playerDetected = _objectSearcher.SearchByDotProduct(transform, _player)
                            && _objectSearcher.SearchByRays(_player);

        //Debug.Log($"_objectSearcher.SearchByDotProduct(transform, _player): {_objectSearcher.SearchByDotProduct(transform, _player) }");
        //Debug.Log($"_objectSearcher.SearchByRays(_player): {_objectSearcher.SearchByRays(_player)}");
       
        //Debug.Log(playerDetected);
        if (playerDetected)
        {
            Detect();
        }
        else
        {
            Lost();
        }
    }



    #region Visualization
    private void OnDrawGizmos()
    {
        if (_objectSearcher == null)
            return;

        _objectSearcher.DrawFieldOfView();
        _objectSearcher.DrawRay(_player);
    }
    #endregion
}