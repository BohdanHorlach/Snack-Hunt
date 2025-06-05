using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _slider;


    private void Awake()
    {
        _slider.maxValue = _health.CurrentHealth;
        _slider.value = _health.CurrentHealth;
    }


    private void OnEnable()
    {
        _health.OnTakeDamage += TakeDamage;
    }


    private void OnDisable()
    {
        _health.OnTakeDamage -= TakeDamage;
    }


    private void TakeDamage(Transform transform, DamageSource damageSource)
    {
        _slider.value = _health.CurrentHealth;
    }
}