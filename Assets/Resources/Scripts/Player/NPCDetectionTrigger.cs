using UnityEngine;


public class NPCDetectionTrigger : MonoBehaviour, IOnRewind
{
    [SerializeField] private NPCStateMachine[] _NPCs;
    [SerializeField] private CameraTargetSwitcher _cameraSwitcher;
    [SerializeField] private DialogInfo _showedDialog;
    [SerializeField] private Health _health;

    private bool _isDetected = false;


    private void OnEnable()
    {
        foreach (NPCStateMachine npc in _NPCs)
            npc.OnDetectPlayer += OnDetectPlayer;
    }


    private void OnDisable()
    {
        foreach (NPCStateMachine npc in _NPCs)
            npc.OnDetectPlayer -= OnDetectPlayer;
        
    }


    private void OnDetectPlayer(Transform transform)
    {
        if (_isDetected)
            return;

        _isDetected = true;
        PauseHandler.Pause(false);
        _cameraSwitcher.LookAt(transform, 
            () => {
                DialogSystem.Show(_showedDialog, () => _health.TakeDamage());
            });
    }


    public void OnBeforeRewind()
    {
        _isDetected = false;
    }
}