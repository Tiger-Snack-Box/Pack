[System.Serializable]
public class Order
{
    public string itemName;
    public float timeToComplete;
    public float timeRemaining;

    public Order(string itemName, float timeToComplete)
    {
        this.itemName = itemName;
        this.timeToComplete = timeToComplete;
        this.timeRemaining = timeToComplete;
    }
}
