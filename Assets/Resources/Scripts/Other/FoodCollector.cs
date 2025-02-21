using System;
using UnityEngine;


public class FoodCollector : MonoBehaviour
{
    [SerializeField] private ObjectFinderByMask _foodFinder;

    private int _counter = 0;


    public Action<int> OnCounterChange;


    private void OnEnable()
    {
        _foodFinder.OnObjectDetect += DetectFood;
    }


    private void OnDisable()
    {
        _foodFinder.OnObjectDetect -= DetectFood;
    }


    private void DetectFood(Transform food)
    {
        Item itemInfo = food.GetComponent<Item>();
        _counter += itemInfo.ExpPoint;
        
        OnCounterChange?.Invoke(_counter);
    }
}
