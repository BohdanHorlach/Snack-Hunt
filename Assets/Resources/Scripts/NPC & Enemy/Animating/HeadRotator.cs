using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class HeadRotator : MonoBehaviour, IOnRewind
{
    [SerializeField] private Transform _head;
    [SerializeField] private TwoBoneIKConstraint _IKHead;
    [SerializeField] private float _rotateDuratuin = 0.5f;

    private Tween _rotateTween;
    private Tween _IKTween;
    private Quaternion _defaultRotate;

    public Transform Head { get => _head; }

    public Action OnStartRotateHead;
    public Action OnEndRotateHead;


    private void Start()
    {
        _defaultRotate = _head.rotation;
    }


    public void RotateHeadTowards(Vector3 targetPosition)
    {
        if (_IKTween.active == false)
            _IKTween = DOVirtual.Float(_IKHead.weight, 1f, _rotateDuratuin, (value) => _IKHead.weight = value);
        
        _rotateTween.Kill();

        OnStartRotateHead?.Invoke();

        Vector3 direction = (targetPosition - _head.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        _rotateTween = _head.DORotateQuaternion(targetRotation, _rotateDuratuin)
            .SetEase(Ease.OutSine)
            .OnComplete(() => OnEndRotateHead?.Invoke());
    }


    public void LostTarget()
    {
        _rotateTween.Kill();
        _IKTween.Kill();

        _rotateTween = _head.DORotateQuaternion(_defaultRotate, _rotateDuratuin)
            .SetEase(Ease.OutSine);
        _IKTween = DOVirtual.Float(_IKHead.weight, 0.1f, _rotateDuratuin, (value) => _IKHead.weight = value);
    }

    
    public void OnBeforeRewind()
    {
        _head.rotation = _defaultRotate;
    }
}
