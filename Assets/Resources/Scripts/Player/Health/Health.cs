using System;
using UnityEngine;


public class Health : MonoBehaviour
{
    [SerializeField] private int _healthPoint;

    private bool _isDeath = false;

    public int CurrentHealth { get => _healthPoint; }
    public Action<Transform, DamageSource> OnTakeDamage;
    public Action OnDeath;


    public void TakeDamage(DamageSource damageSource)
    {
        if (_isDeath)
            return;

        _healthPoint--;

        if (_healthPoint == 0)
        {
            _isDeath = true;
            OnDeath?.Invoke();
        }
        else
        {
            OnTakeDamage?.Invoke(transform, damageSource);
        }
    }
}