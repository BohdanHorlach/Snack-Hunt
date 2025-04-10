using UnityEngine;
using UnityEngine.Animations.Rigging;


public class IKHandAnimation : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint _twoBoneIKConstraint;
    [SerializeField] private Transform _IKHandTarget;
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


    public void TakeObject(HoldingObjectInfo item)
    {
        _holdingPosition = GetHoldingPosition(item);
    }


    public void DropObject()
    {
        _holdingPosition = null;
        _twoBoneIKConstraint.weight = 0;
    }
}
