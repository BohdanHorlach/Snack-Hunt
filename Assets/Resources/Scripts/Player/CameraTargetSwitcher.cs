using DG.Tweening;
using UnityEngine;


public class CameraTargetSwitcher : MonoBehaviour
{
    [SerializeField] private ObjectThrower _objectThrower;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Transform _defaultPosition;
    [SerializeField] private Transform _throwPosition;
    [SerializeField] private float _duration;


    private void OnEnable()
    {
        _objectThrower.OnPrepareToThrow += SwitchToThrowMode;
        _objectThrower.OnCanceledPrepareToThrow += SwitchToDefaultMode;
        _objectThrower.OnThrow += SwitchToDefaultMode;
    }


    private void OnDisable()
    {
        _objectThrower.OnPrepareToThrow -= SwitchToThrowMode;
        _objectThrower.OnCanceledPrepareToThrow -= SwitchToDefaultMode;
        _objectThrower.OnThrow -= SwitchToDefaultMode;
    }


    private void SwitchTo(Transform target)
    {
        _cameraTarget.DOLocalMove(target.localPosition, _duration);
    }


    private void SwitchToThrowMode()
    {
        SwitchTo(_throwPosition);
    }


    private void SwitchToDefaultMode()
    {
        SwitchTo(_defaultPosition);
    }
}
