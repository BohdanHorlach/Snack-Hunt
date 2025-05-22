using UnityEngine;


public class VisibilityByPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private ObjectSearcher _objectSearcher;
    public bool IsVisible { get; private set; }


    private void Awake()
    {
        _objectSearcher = new ObjectSearcher(new SearchSettings());
    }


    private void Update()
    {
        IsVisible = _objectSearcher.SearchByDotProduct(_player, transform);
    }
}