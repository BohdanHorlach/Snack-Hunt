using System.Collections;
using TMPro;
using UnityEngine;


public class LoaderAnimateHandler : MonoBehaviour
{
    [SerializeField] private FullScreenPassRendererFeature[] _rendererFeatures;
    [SerializeField] private IKHandAnimation[] _iKHandAnimations;
    [SerializeField] private Item _food;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private float _duration = 1.5f;

    private const string LOAD_TEXT_SAMPLE = "Loading";


    private void Awake()
    {
        foreach (var iKHand in _iKHandAnimations) 
            iKHand.TakeObject(_food.HoldingInfo);

        SetActiveRenderFeatures(false);
        StartCoroutine(AnimateLoadingText());
    }

    private void OnDestroy()
    {
        SetActiveRenderFeatures(true);
        StopAllCoroutines();
    }


    private void SetActiveRenderFeatures(bool flag)
    {
        foreach (var rendererFeature in _rendererFeatures)
        {
            rendererFeature.SetActive(flag);
        }
    }


    private IEnumerator AnimateLoadingText()
    {
        int dotCount = 0;

        while (true)
        {
            string dots = new string('.', dotCount);
            _loadingText.text = LOAD_TEXT_SAMPLE + dots;

            dotCount = (dotCount + 1) % 4;

            yield return new WaitForSeconds(_duration);
        }
    }
}