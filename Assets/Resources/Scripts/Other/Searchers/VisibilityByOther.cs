using UnityEngine;


public class VisibilityByOther : MonoBehaviour
{
    [SerializeField] private Transform _other;

    private ObjectSearcher _objectSearcher;
    public bool IsVisible { get; private set; }


    private void Awake()
    {
        _objectSearcher = new ObjectSearcher(new SearchSettings());
    }


    private void Update()
    {
        IsVisible = _objectSearcher.SearchByDotProduct(_other, transform);
    }
}