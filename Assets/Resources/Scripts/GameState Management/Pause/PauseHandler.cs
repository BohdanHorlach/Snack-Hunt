using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseHandler : MonoBehaviour
{
    [SerializeField] private static UIAnimateHandler _UIAnimateHandler;

    private static IPaused[] _pauseds;
    private static bool _isPaused = false;


    public void Awake()
    {
        _pauseds = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IPaused>()
                    .ToArray();
    }


    public static void Play()
    {
        foreach (IPaused item in _pauseds)
            item.Resume();

        _isPaused = false;
    }


    public static void Pause(bool withUI)
    {
        foreach (IPaused item in _pauseds)
            item.Pause();

        _isPaused = true;

        if(withUI)
            _UIAnimateHandler.Pause();
    }



    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.started == false || _UIAnimateHandler.IsOnTransition)
            return;

        if (_isPaused)
            _UIAnimateHandler.BackToPlay();
        else
            Pause(true);
    }
}