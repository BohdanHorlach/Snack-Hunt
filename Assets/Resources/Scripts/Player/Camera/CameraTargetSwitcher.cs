using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEngine;


public class CameraTargetSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private UIAnimateHandler _uiAnimator;
    [SerializeField] private ObjectThrower _objectThrower;
    [SerializeField] private Climber _climber;
    [SerializeField] private Transform _targetParent;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _defaultPosition;
    [SerializeField] private Transform _throwPosition;
    [SerializeField] private float _duration;
    [SerializeField] private float _stopDistance = 8f;

    private Vector3 _targetLocalPosition;


    private void Awake()
    {
        _targetLocalPosition = _cameraTarget.localPosition;
    }


    private void OnEnable()
    {
        _uiAnimator.OnRewindEnd += OnRewindEnd;
        _objectThrower.OnPrepareToThrow += SwitchToThrowMode;
        _objectThrower.OnCanceledPrepareToThrow += SwitchToDefaultMode;
        _objectThrower.OnThrow += SwitchToDefaultMode;
        _climber.OnClimb += RemoveTargetParent;
        _climber.OnClimbFinished += SetTargetParent;
    }


    private void OnDisable()
    {
        _uiAnimator.OnRewindEnd -= OnRewindEnd;
        _objectThrower.OnPrepareToThrow -= SwitchToThrowMode;
        _objectThrower.OnCanceledPrepareToThrow -= SwitchToDefaultMode;
        _objectThrower.OnThrow -= SwitchToDefaultMode;
        _climber.OnClimb -= RemoveTargetParent;
        _climber.OnClimbFinished -= SetTargetParent;
    }


    private void SwitchTo(Vector3 position)
    {
        _cameraTarget.DOLocalMove(position, _duration);
    }


    private void SwitchToThrowMode()
    {
        SwitchTo(_throwPosition.localPosition);
    }


    private void SwitchToDefaultMode()
    {
        SwitchTo(_defaultPosition.localPosition);
    }


    private void RemoveTargetParent()
    {
        _cameraTarget.SetParent(null);
    }


    private void SetTargetParent()
    {
        _cameraTarget.SetParent(_targetParent);
        SwitchTo(_targetLocalPosition);
    }


    public void LookAt(Transform transform, Action callback = null)
    {
        _virtualCamera.enabled = false;
        Transform camera = Camera.main.transform;

        Vector3 direction = (transform.position - camera.position).normalized;
        Vector3 targetPosition = transform.position - direction * _stopDistance;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        camera.DOMove(targetPosition, _duration).SetEase(Ease.OutSine);

        camera.DORotateQuaternion(targetRotation, _duration)
            .SetEase(Ease.OutSine)
            .OnComplete(() => callback?.Invoke());
    }


    public void OnRewindEnd()
    {
        _virtualCamera.enabled = true;
    }
}
