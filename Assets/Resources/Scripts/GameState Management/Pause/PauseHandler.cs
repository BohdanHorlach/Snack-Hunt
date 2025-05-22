using UnityEngine;
using UnityEngine.InputSystem;


public class PauseHandler : MonoBehaviour
{
    [SerializeField] private UIAnimateHandler _UIAnimateHandler;

    private static PausedObject[] _pauseds;
    private static bool _isPaused = false;


    public void Awake()
    {
        _pauseds = FindObjectsByType<PausedObject>(FindObjectsSortMode.None);
    }


    private static void Play()
    {
        foreach (PausedObject item in _pauseds)
            item.Resume();
    }


    private static void Pause()
    {
        foreach (PausedObject item in _pauseds)
            item.Pause();
    }


    public static void SwitchMode()
    {
        if (_isPaused)
            Play();
        else
            Pause();

        _isPaused = !_isPaused;
    }


    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.started == false || _UIAnimateHandler.IsOnTransition)
            return;

        if (_isPaused)
            _UIAnimateHandler.BackToPlay();
        else
            SwitchMode();
    }
}