using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class DialogSystem : MonoBehaviour
{
    [SerializeField] private Animator _dialogAnimator;
    [SerializeField] private DialogDisplay _display;
    
    private DialogInfo _dialogInfo;
    private Action _hideCallback;
    private int _index = -1;
    private bool _isHide = true;
    private bool _callbackIsCalled = false;


    private void NextLine()
    {
        _index++;

        if (_index < _dialogInfo.Lines.Length)
            _display.Input(_dialogInfo.Lines[_index]);
        else
            _hideCallback?.Invoke();
    }


    public void OnInput(InputAction.CallbackContext context)
    {
        if (context.canceled == false || _isHide)
            return;

        if (_display.IsTyping)
            _display.StopTyping();
        else
            NextLine();
    }


    public void Show(DialogInfo dialog, Action callback = null)
    {
        _isHide = false;
        _dialogInfo = dialog;
        _dialogAnimator.SetBool("IsHide", false);

        _hideCallback = () => 
        {
            _callbackIsCalled = true;
            _index = -1;   

            callback?.Invoke();
            Hide();
        };

        NextLine();
    }


    public void Hide()
    {
        _isHide = true;
        _dialogAnimator.SetBool("IsHide", true);

        if (_callbackIsCalled == false)
            _hideCallback?.Invoke();
    }
}