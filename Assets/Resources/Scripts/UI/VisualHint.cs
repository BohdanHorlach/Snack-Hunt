using DG.Tweening;
using TMPro;
using UnityEngine;


public class VisualHint : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask[] _finders;
    [SerializeField] private PlayerInteraction _interact;
    [SerializeField] private TextMeshPro _hint;
    [SerializeField] private float _yOffset = 1f;
    [SerializeField] private float _showDuration;

    private Tween[] _tweens;
    private Transform _lastDetect;
    private Camera _mainCamera;
    private Vector3 _basePosition;
    private Vector3 _baseScale;


    private void Start()
    {
        _tweens = new Tween[2];
        _mainCamera = Camera.main;
        _baseScale = _hint.transform.localScale;
        _basePosition = _hint.transform.localPosition;

        _hint.text = "E";
        _hint.transform.localScale = Vector3.zero;
    }


    private void OnEnable()
    {
        foreach (ObjectFinderByMask finder in _finders)
        {
            finder.OnObjectDetect += ShowHint;
            finder.OnObjectLost += HideHint;
        }
    }


    private void OnDisable()
    {
        foreach (ObjectFinderByMask finder in _finders)
        {
            finder.OnObjectDetect -= ShowHint;
            finder.OnObjectLost -= HideHint;
        }
    }


    private void Update()
    {
        Vector3 direction = (_mainCamera.transform.position - _hint.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        _hint.transform.rotation = targetRotation;
    }


    private void DisableTweens()
    {
        foreach (Tween tween in _tweens)
            tween.Kill();
    }


    private void ShowHint(Transform transform)
    {
        if (_interact.IsCanInteract == false || _lastDetect == transform)
            return;

        _lastDetect = transform;

        DisableTweens();

        _tweens[0] = _hint.transform.DOScale(_baseScale, _showDuration);
        _tweens[1] = _hint.transform.DOLocalMoveY(_basePosition.y + _yOffset, _showDuration);
    }


    private void HideHint(Transform transform)
    {
        if (transform != _lastDetect)
            return;

        _lastDetect = null;
        DisableTweens();
        _hint.transform.localScale = Vector3.zero;
        _hint.transform.localPosition = _basePosition;
    }
}