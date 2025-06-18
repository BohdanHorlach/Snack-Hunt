using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class Item : MonoBehaviour, IPaused
{
    [SerializeField] private ObjectFinderByMask _groundFinder;
    [SerializeField] private AudioInvoker _audioInvoker;
    
    private Vector3 _velocityBeforePause;
    private bool _isEnableGravityBeforePause;

    public HoldingObjectInfo HoldingInfo;
    public Rigidbody Rigidbody;
    public int ExpPoint;
    public bool IsThrowet { get; set; }


    private void Update()
    {
        if (IsThrowet == false || PauseHandler.IsPaused)
            return;

        if (_groundFinder.IsHaveObjectInSpace)
        {
            _audioInvoker.PlayAudio();
            IsThrowet = false;
        }
    }


    private void SwitchPauseMode(bool isPaused)
    {
        if (_isEnableGravityBeforePause == false)
            return;

        Rigidbody.useGravity = !isPaused;
        Rigidbody.isKinematic = isPaused;

        if(Rigidbody.isKinematic == false)
            Rigidbody.linearVelocity = _velocityBeforePause;
    }


    public void Pause()
    {
        _isEnableGravityBeforePause = Rigidbody.useGravity;
        _velocityBeforePause = Rigidbody.linearVelocity;

        SwitchPauseMode(true);
    }


    public void Resume()
    {
        SwitchPauseMode(false);
    }
}