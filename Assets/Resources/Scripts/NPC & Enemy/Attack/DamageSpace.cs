using UnityEngine;


public class DamageSpace : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _damageSpace;


    private void OnEnable()
    {
        _damageSpace.OnObjectDetect += MakeDamage;
    }


    private void OnDisable()
    {
        _damageSpace.OnObjectDetect -= MakeDamage;
    }


    private void MakeDamage(Transform transform)
    {
        if(transform.TryGetComponent(out HitBox hitBox))
            hitBox.TakeDamage();
    }
}