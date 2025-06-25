using System;
using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public List<int> snackTypes;
    public float timeToComplete;
    public float timeRemaining;

    public Order(List<int> snackTypes, float timeToComplete)
    {
        this.snackTypes = snackTypes;
        this.timeToComplete = timeToComplete;
        this.timeRemaining = timeToComplete;
    }
}
