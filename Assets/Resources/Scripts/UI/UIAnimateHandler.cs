using DG.Tweening;
using System;
using TMPro;
using UnityEngine;


public class UIAnimateHandler : MonoBehaviour
{
    [SerializeField] private Health _player;
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] private TextMeshProUGUI _counterOfBackToPlay;
    [SerializeField] private float _pauseDuration = 0.8f;
    [SerializeField] private float _pauseInterval = 0.2f;

    public bool IsOnTransition { get; private set; } = false;

    public Action OnRewindEnd;


    private void OnEnable()
    {
        _player.OnTakeDamage += DoRewind;
    }


    private void OnDisable()
    {
        _player.OnTakeDamage -= DoRewind;
    }


    private void StartCounterForEnterOnPlayMode()
    {
        _counterOfBackToPlay.enabled = true;
        IsOnTransition = true;

        Vector3 startScale = _counterOfBackToPlay.transform.localScale;
        _counterOfBackToPlay.transform.localScale = Vector3.zero;

        Sequence countdownSequence = DOTween.Sequence();

        for (int i = 3; i >= 0; i--)
        {
            int currentNumber = i;

            countdownSequence.AppendCallback(() => {
                _counterOfBackToPlay.text = currentNumber.ToString();
                _counterOfBackToPlay.transform.localScale = Vector3.zero;
            });

            countdownSequence.Append(_counterOfBackToPlay.transform.DOScale(startScale, _pauseDuration / 2));
            countdownSequence.AppendInterval(_pauseInterval);
            countdownSequence.Append(_counterOfBackToPlay.transform.DOScale(Vector3.zero, _pauseDuration / 2));
        }

        countdownSequence.OnComplete(() => {
            PauseHandler.Play();
            _counterOfBackToPlay.enabled = false;
            _counterOfBackToPlay.transform.localScale = startScale;
            IsOnTransition = false;
        });
        countdownSequence.Play();
    }


    private void DoRewind(Transform obj, DamageSource damageSource)
    {
        _UIAnimator.SetTrigger("MakeRewind");
    }


    //Call from animator
    private void EndRewind()
    {
        OnRewindEnd?.Invoke();
    }


    public void EndGame()
    {
        _UIAnimator.SetTrigger("EndGame");
    }


    public void BackToPlay()
    {
        _UIAnimator.SetTrigger("BackToPlay");
        StartCounterForEnterOnPlayMode();
    }


    public void Pause()
    {
        _UIAnimator.SetTrigger("Pause");
    }
}