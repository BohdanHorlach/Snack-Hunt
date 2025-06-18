using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseHandler : MonoBehaviour
{
    [SerializeField] private UIAnimateHandler _UIAnimateHandler;

    private static PauseHandler _instance;
    private static IPaused[] _pauseds;
    public static bool IsPaused { get; private set; } = false;


    public void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        _pauseds = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IPaused>()
                    .ToArray();
    }


    public static void Play()
    {
        foreach (IPaused item in _pauseds)
            item.Resume();

        IsPaused = false;
    }


    public static void Pause(bool withUI)
    {
        foreach (IPaused item in _pauseds)
            item.Pause();

        IsPaused = true;

        if(withUI)
            _instance._UIAnimateHandler.Pause();
    }


    public void InputPause()
    {
        bool isDialogHidden = DialogSystem.IsHide;

        if (DialogSystem.IsHide)
        {
            if (IsPaused)
                _UIAnimateHandler.BackToPlay(true);
            else
                Pause(true);
        }
        else
        {
            if (_UIAnimateHandler.IsPaused)
                _UIAnimateHandler.BackToPlay(false);
            else
                _UIAnimateHandler.Pause(); 
        }
    }


    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.started == false || _UIAnimateHandler.IsOnTransition)
            return;

        InputPause();
    }
}