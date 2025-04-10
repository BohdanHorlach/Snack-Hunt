using UnityEngine;


public class LoaderAnimateHandler : MonoBehaviour
{
    [SerializeField] private FullScreenPassRendererFeature[] _rendererFeatures;
    [SerializeField] private Item _food;
    [SerializeField] private IKHandAnimation[] _iKHandAnimations;


    private void Start()
    {
        foreach (var iKHand in _iKHandAnimations) 
        {
            iKHand.TakeObject(_food.HoldingInfo);
        }
    }


    private void OnEnable()
    {
        SetActiveRenderFeatures(false);
    }


    private void OnDisable()
    {
        SetActiveRenderFeatures(true);
    }


    private void SetActiveRenderFeatures(bool flag)
    {
        foreach (var rendererFeature in _rendererFeatures)
        {
            rendererFeature.SetActive(flag);
        }
    }
}