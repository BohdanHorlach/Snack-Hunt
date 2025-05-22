using DG.Tweening;
using TMPro;
using UnityEngine;


public class UIAnimateHandler : PausedObject
{
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] private TextMeshProUGUI _counterOfBackToPlay;
    [SerializeField] private GameObject[] _pauseMenuElements;

    private const float DURATION = 0.8f;
    private const float INTERVAL = 0.2f;


    public bool IsOnTransition { get; private set; } = false;


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

            countdownSequence.Append(_counterOfBackToPlay.transform.DOScale(startScale, DURATION / 2));
            countdownSequence.AppendInterval(INTERVAL);
            countdownSequence.Append(_counterOfBackToPlay.transform.DOScale(Vector3.zero, DURATION / 2));
        }

        countdownSequence.OnComplete(() => {
            PauseHandler.SwitchMode();
            _counterOfBackToPlay.enabled = false;
            _counterOfBackToPlay.transform.localScale = startScale;
            IsOnTransition = false;
        });
        countdownSequence.Play();
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


    public override void Pause()
    {
        _UIAnimator.SetTrigger("Pause");
    }


    public override void Resume() { }
}