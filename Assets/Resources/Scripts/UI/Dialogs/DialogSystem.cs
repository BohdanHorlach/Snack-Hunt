using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class DialogSystem : MonoBehaviour
{
    [SerializeField] private Animator _dialogAnimator;
    [SerializeField] private DialogDisplay _display;
    [SerializeField] private UIAnimateHandler _UIAnimateHandler;

    private static DialogSystem _instance;
    private DialogInfo _dialogInfo;
    private Action _hideCallback;
    private int _index = -1;
    private bool _isHide = true;
    private bool _callbackIsCalled = false;


    public static bool IsHide { get => _instance._isHide; }


    private void Awake()
    {
        if(_instance == null)
            _instance = this;
    }


    private void NextLine()
    {
        _index++;

        if (_index < _dialogInfo.Lines.Length)
            _display.Input(_dialogInfo.Lines[_index]);
        else
            _hideCallback?.Invoke();
    }


    private void InputNext()
    {
        if (_isHide)
            return;

        if (_display.IsTyping)
            _display.StopTyping();
        else
            NextLine();
    }


    private void HideDialog()
    {
        _isHide = true;
        _dialogAnimator.SetBool("IsHide", true);

        if (_callbackIsCalled == false)
            _hideCallback?.Invoke();
    }


    private void ShowDialog(DialogInfo dialog, Action callback = null)
    {
        _isHide = false;
        _dialogInfo = dialog;
        _dialogAnimator.SetBool("IsHide", false);

        _hideCallback = () => 
        {
            _callbackIsCalled = true;
            _index = -1;

            Debug.Log(nameof(HideDialog));
            callback?.Invoke();
            HideDialog();
        };

        NextLine();
    }


    public static void Show(DialogInfo dialog, Action callback = null)
    {
        _instance.ShowDialog(dialog, callback);
    }


    public static void Hide()
    {
        _instance.HideDialog();
    }


    public void OnInput(InputAction.CallbackContext context)
    {
        if (context.canceled == false || _UIAnimateHandler.IsPaused)
            return;

        _instance.InputNext();
    }
}