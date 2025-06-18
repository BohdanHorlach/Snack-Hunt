using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class FoodCollector : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _foodFinder;
    [SerializeField] private Transform _dropPosition;
    [SerializeField] private float _dropDelay = 1f;

    private static List<Item> _collector;
    private static int _counter = 0;
    private static bool _isCanRemoved = true;    
    
    public static float Goal { get; private set; } = 250f;
    public static int Counter { get => _counter; }


    public Action<int> OnCounterChange;
    public Action<Item> OnDropFood;
    public Action OnDropingEding;


    private void Awake()
    {
        _collector = new List<Item>();
    }


    private void OnEnable()
    {
        _foodFinder.OnObjectDetect += DetectFood;
        _foodFinder.OnObjectLost += LostFood;
    }


    private void OnDisable()
    {
        _foodFinder.OnObjectDetect -= DetectFood;
        _foodFinder.OnObjectLost -= LostFood;
    }


    private void DetectFood(Transform food)
    {
        Item foodInfo = food.GetComponent<Item>();

        if (foodInfo == null || _collector.Contains(foodInfo))
            return;

        _counter += foodInfo.ExpPoint;

        _collector.Add(foodInfo);
        OnCounterChange?.Invoke(_counter);
    }


    private void LostFood(Transform food)
    {
        if (_isCanRemoved == false || PauseHandler.IsPaused)
            return;

        Item foodInfo = food.GetComponent<Item>();
        _counter -= foodInfo.ExpPoint;

        _collector.Remove(foodInfo);
        OnCounterChange?.Invoke(_counter);
    }


    private IEnumerator DropFood()
    {
        foreach (var food in _collector) 
        {
            food.transform.position = _dropPosition.position;
            OnDropFood?.Invoke(food);

            yield return new WaitForSeconds(_dropDelay);
        }

        OnDropingEding?.Invoke();
    }


    public void StartDropingFood()
    {
        _isCanRemoved = false;
        StartCoroutine("DropFood");
    }
}
