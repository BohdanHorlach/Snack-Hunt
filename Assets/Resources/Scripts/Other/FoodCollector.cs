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
        _foodFinder.OnObjectLost += LostFood;
    }


    private void OnDisable()
    {
        _foodFinder.OnObjectDetect -= DetectFood;
        _foodFinder.OnObjectLost -= LostFood;
    }


    private void DetectFood(Transform food)
    {
        Item itemInfo = food.GetComponent<Item>();
        _counter += itemInfo.ExpPoint;
        
        OnCounterChange?.Invoke(_counter);
    }


    private void LostFood(Transform food)
    {
        Item itemInfo = food.GetComponent<Item>();
        _counter -= itemInfo.ExpPoint;

        Debug.Log(itemInfo);

        OnCounterChange?.Invoke(_counter);
    }
}
