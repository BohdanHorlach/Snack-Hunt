using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class ObjectThrower : MonoBehaviour
{
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private Transform _throwPlace;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private ObjectGrabber _objectGrabber;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _releasePosition;
    [SerializeField, Range(1, 100)] private float _throwStrength = 10f;
    [SerializeField, Range(0, 1)] private float _forwardForce = 1f;
    [SerializeField, Range(0, 1)] private float _upForce = 0.5f;
    [SerializeField, Range(10, 100)] private int _linePoints = 25;
    [SerializeField, Range(0.01f, 0.25f)] private float _timeBetweenPoints = 0.1f;
    [SerializeField] private LayerMask _collisionMask;


    private Vector3 _throwDirection;
    private bool IsCanThrow => _playerState.IsOnGround
                            && _playerState.IsHoldingObject
                            && IsReadyToThrow;   
    public bool IsReadyToThrow { get; private set; } = false;
    public bool IsThrowing { get; private set; } = false;


    public Action OnPrepareToThrow;
    public Action OnCanceledPrepareToThrow;
    public Action OnThrow;


    private void Start()
    {
        _lineRenderer.enabled = false;
    }


    private void OnEnable()
    {
        _objectGrabber.OnDrop += Drop;
    }


    private void OnDisable()
    {
        _objectGrabber.OnDrop -= Drop;
    }


    private void Update()
    {
        if (IsCanThrow == false || _playerState.IsPaused)
            return;

        if (IsThrowing == false)
        {
            _baseTransform.rotation = Quaternion.Euler(
                _baseTransform.eulerAngles.x,
                Camera.main.transform.rotation.eulerAngles.y,
                _baseTransform.eulerAngles.z
            );
            DrawProjection();
        }
    }


    private Vector3 GetThrowDirection()
    {
        return (Camera.main.transform.forward * _forwardForce + Camera.main.transform.up * _upForce).normalized;
    }


    private void DrawProjection()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = Mathf.CeilToInt(_linePoints / _timeBetweenPoints) + 1;
        Vector3 startPosition = _releasePosition.position;
        Vector3 startVelocity = _throwStrength * GetThrowDirection() / _objectGrabber.Item.Rigidbody.mass;
        int i = 0;
        _lineRenderer.SetPosition(i, startPosition);
        for (float time = 0; time < _linePoints; time += _timeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            _lineRenderer.SetPosition(i, point);

            Vector3 lastPosition = _lineRenderer.GetPosition(i - 1);

            if (Physics.Raycast(lastPosition,
                (point - lastPosition).normalized,
                out RaycastHit hit,
                (point - lastPosition).magnitude,
                _collisionMask))
            {
                _lineRenderer.SetPosition(i, hit.point);
                _lineRenderer.positionCount = i + 1;
                return;
            }
        }
    }


    private void Cancele()
    {
        _lineRenderer.enabled = false;
        IsReadyToThrow = false;
    }


    private void Drop()
    {
        Cancele();
        OnCanceledPrepareToThrow?.Invoke();
    }


    private void PrepareToThrow()
    {
        _objectGrabber.SetNewPlace(_throwPlace);
        HoldingPosition throwPosition = _objectGrabber.Item.HoldingInfo.ThrowTransform;
        _objectGrabber.Item.transform.localPosition = throwPosition.Position;
        _objectGrabber.Item.transform.localRotation = throwPosition.Rotation;

        _throwDirection = GetThrowDirection();
    }


    public void GetReadyToThrow(InputAction.CallbackContext context)
    {
        if (_playerState.IsOnGround == false)
            return;

        if (Application.isFocused == false || _objectGrabber.IsHoldingObject == false)
        {
            Cancele();
        }
        else if(context.performed)
        {
            IsReadyToThrow = true;

            OnPrepareToThrow?.Invoke();
        }
        else if(context.canceled)
        {
            Drop();
        }
    }


    public void ThrowObject(InputAction.CallbackContext context)
    {
        if (context.canceled && IsReadyToThrow)
        {
            Cancele();

            PrepareToThrow();
            OnThrow?.Invoke();
            IsThrowing = true;
        }
    }


    public void ThrowToCameraDirection()
    {
        if (_objectGrabber.Item == null)
            return;

        Rigidbody itemBody = _objectGrabber.Item.Rigidbody;
        _objectGrabber.Item.IsThrowet = true;
        _objectGrabber.DropObject();

        itemBody.isKinematic = false;
        itemBody.useGravity = true;
        itemBody.freezeRotation = false;
        itemBody.linearVelocity = Vector3.zero;
        itemBody.angularVelocity = Vector3.zero;
        itemBody.AddForce(_throwDirection * _throwStrength, ForceMode.Impulse);
    }


    public void CompletingOfTheThrow()
    {
        IsThrowing = false;
    }
}