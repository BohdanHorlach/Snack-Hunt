using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class ExperienceCounter : MonoBehaviour
{
    [SerializeField] private FoodCollector _foodCollector;
    [SerializeField] private TextMeshProUGUI _output;
    [SerializeField, Min(0.1f)] private float _speedTransition = 0.5f;


    private void OnEnable()
    {
        _foodCollector.OnCounterChange += StartTransition;
    }


    private void OnDisable()
    {
        _foodCollector.OnCounterChange -= StartTransition;
    }


    private void StartTransition(int exp)
    {
        StopCoroutine("UpdateCounter");
        StartCoroutine(UpdateCounter(exp));
    }


    private IEnumerator UpdateCounter(int newExp)
    {
        float elapsed = 0.0f;
        int startExp = Convert.ToInt32(_output.text);

        while (elapsed < _speedTransition)
        {
            float duration = elapsed / _speedTransition;
            elapsed += Time.deltaTime;
            _output.SetText(Convert.ToInt32(Mathf.Lerp(startExp, newExp, duration)).ToString());
            yield return null;
        }

        _output.SetText(newExp.ToString());
    }
}