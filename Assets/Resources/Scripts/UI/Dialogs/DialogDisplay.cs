using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class DialogDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _mainText;
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private float _autoTransitionTime = 15f;

    private Action _callback;
    private DialogPart _currentText;

    public bool IsTyping { get; private set; } = false;


    private void Start()
    {
        _name.text = "";
        _mainText.text = "";
    }



    private IEnumerator TypeLine()
    {
        IsTyping = true;
        _name.text = _currentText.Name;
        _mainText.text = "";

        foreach (char letter in _currentText.Line.ToCharArray())
        {
            _mainText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        IsTyping = false;


        yield return new WaitForSeconds(_autoTransitionTime);
        _callback?.Invoke();
    }


    public void Input(DialogPart dialogPart, Action callback = null)
    {
        _callback = callback;
        _currentText = dialogPart;
        StartCoroutine(TypeLine());
    }


    public void StopTyping()
    {
        StopAllCoroutines();

        IsTyping = false;
        _mainText.text = _currentText.Line;

        _callback?.Invoke();
    }
}