using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    public Text itemNameText;
    public Slider timerSlider;
    public Order order;

    public void Setup(Order order)
    {
        this.order = order;
        itemNameText.text = order.itemName;
        timerSlider.maxValue = order.timeToComplete;
        timerSlider.value = order.timeRemaining;
    }

    public void UpdateUI()
    {
        timerSlider.value = order.timeRemaining;
    }
}
