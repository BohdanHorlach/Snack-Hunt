using System;
using UnityEngine;


public class HitBox : MonoBehaviour
{
    [SerializeField] private Health _player;
    [SerializeField] private DialogInfo _showedDialog;

    public Action OnBeforeTakeObject;

    public void TakeDamage()
    {
        OnBeforeTakeObject?.Invoke();

        PauseHandler.Pause(false);
        DialogSystem.Show(_showedDialog, () => { _player.TakeDamage(); });
    }
}