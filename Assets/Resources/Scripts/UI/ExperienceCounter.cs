using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class ExperienceCounter : MonoBehaviour
{
    [SerializeField] private GameOverTrigger _exitTrigger;
    [SerializeField] private FoodCollector[] _foodCollectors;
    [SerializeField] private TextMeshProUGUI _output;
    [SerializeField, Min(0.1f)] private float _speedTransition = 0.5f;


    private void OnEnable()
    {
        foreach (var foodCollector in _foodCollectors)
        {
            foodCollector.OnCounterChange += StartTransition;
            foodCollector.OnDropFood += DropFood;
        }

        _exitTrigger.OnTriggered += ResetOutput;
    }


    private void OnDisable()
    {
        foreach (var foodCollector in _foodCollectors)
        {
            foodCollector.OnCounterChange -= StartTransition;
            foodCollector.OnDropFood += DropFood;
        }

        _exitTrigger.OnTriggered -= ResetOutput;
    }


    private void StartTransition(int exp)
    {
        StopCoroutine("UpdateCounter");
        StartCoroutine(UpdateCounter(exp));
    }


    private void DropFood(Item item)
    {
        StartTransition(item.ExpPoint);
    }


    private void ResetOutput()
    {
        _output.text = "0";
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