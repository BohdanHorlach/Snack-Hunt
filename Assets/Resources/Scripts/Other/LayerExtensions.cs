using UnityEngine;


public static class LayerMaskExtensions
{
    public static bool IsEnterOnMask(this LayerMask layerMask, Transform transform)
    {
        return ((1 << transform.gameObject.layer) & layerMask) != 0;
    }
}