using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class Item : MonoBehaviour, IPaused
{
    private Vector3 _velocityBeforePause;
    private bool _isEnableGravityBeforePause;

    public HoldingObjectInfo HoldingInfo;
    public Rigidbody Rigidbody;
    public int ExpPoint;


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