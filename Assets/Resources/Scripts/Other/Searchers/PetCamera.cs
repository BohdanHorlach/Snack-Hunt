using DG.Tweening;
using UnityEngine;


public class PetCamera : MonoBehaviour
{
    [SerializeField] private MotionDetecter _motionDetecter;
    [SerializeField] private Transform _case;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _speedRotate = 10f;


    private void OnEnable()
    {
        _motionDetecter.OnMotionDetected += DetecPlayer;
    }


    private void TurnTowardsThePosition(Vector3 position, Transform transform, AxisConstraint axisConstraint)
    {
        transform.DOKill();

        transform.DOLookAt(position, _speedRotate, axisConstraint);
    }


    private void DetecPlayer(Vector3 position) 
    {
        TurnTowardsThePosition(position, _case, AxisConstraint.Y);
        TurnTowardsThePosition(position, _camera, AxisConstraint.None);
    }
}
