using UnityEngine;


public class VisibilityByPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _dotThreshold = 0.1f;

    public bool IsVisible { get; private set; }


    private void Update()
    {
        Vector3 directionFromPlayerToEnemy = transform.position - _player.position;
        float dotPruduct = Vector3.Dot(_player.forward, directionFromPlayerToEnemy.normalized);

        IsVisible = dotPruduct >= _dotThreshold;
    }
}