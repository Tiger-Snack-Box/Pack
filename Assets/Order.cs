using UnityEngine;

[System.Serializable]
public class Order
{
    public Sprite sprite;
    public int requiredCapacity;
    public float timeRemaining;
    public int currentFill;

    public Order(Sprite sprite, int requiredCapacity, float timeRemaining)
    {
        this.sprite = sprite;
        this.requiredCapacity = requiredCapacity;
        this.timeRemaining = timeRemaining;
        this.currentFill = 0;
    }
}
