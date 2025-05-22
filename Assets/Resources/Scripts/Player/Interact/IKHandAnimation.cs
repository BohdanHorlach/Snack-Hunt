using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class IKHandAnimation : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint _twoBoneIKConstraint;
    [SerializeField] private Transform _IKHandTarget;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private bool _isRight;

    private Transform _holdingPosition;
    
    public HoldingObjectInfo HoldingObject { get; private set; }


    private void FixedUpdate()
    {
        if (_holdingPosition != null)
            _IKHandTarget.position = _holdingPosition.position;
    }


    private Transform GetHoldingPosition(HoldingObjectInfo holdingInfo)
    {
        HoldingObject = holdingInfo;
        return _isRight ? HoldingObject.Points.RightHandPosition : HoldingObject.Points.LeftHandPosition;
    }


    private void ChangeIKWeight(bool isTake)
    {
        float start = isTake ? 0 : 1;
        float end = isTake ? 1 : 0;

        DOVirtual.Float(start, end, _duration,
            (value) => {
                _twoBoneIKConstraint.weight = value;
            }
        );
    }


    public void TakeObject(HoldingObjectInfo item)
    {
        _holdingPosition = GetHoldingPosition(item);
        ChangeIKWeight(true);
    }


    public void DropObject()
    {
        _holdingPosition = null;
        ChangeIKWeight(false);
    }
}
