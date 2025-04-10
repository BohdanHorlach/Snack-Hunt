using UnityEngine;


public class SearchingSpace : MonoBehaviour
{
    [SerializeField] private Transform[] _searhingPoints;


    public Transform[] GetPoints()
    {
        return _searhingPoints;
    }
}