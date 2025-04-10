using DG.Tweening;
using UnityEngine;


public class CameraTargetSwitcher : MonoBehaviour
{
    [SerializeField] private ObjectThrower _objectThrower;
    [SerializeField] private Climber _climber;
    [SerializeField] private Transform _targetParent;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _defaultPosition;
    [SerializeField] private Transform _throwPosition;
    [SerializeField] private float _duration;

    private Vector3 _targetLocalPosition;


    private void Awake()
    {
        _targetLocalPosition = _cameraTarget.localPosition;
    }


    private void OnEnable()
    {
        _objectThrower.OnPrepareToThrow += SwitchToThrowMode;
        _objectThrower.OnCanceledPrepareToThrow += SwitchToDefaultMode;
        _objectThrower.OnThrow += SwitchToDefaultMode;
        _climber.OnClimb += RemoveTargetParent;
        _climber.OnClimbFinished += SetTargetParent;
    }


    private void OnDisable()
    {
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
}
