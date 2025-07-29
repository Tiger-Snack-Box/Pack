using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    public Image iconImage;
    public Text capacityText;
    public Order order;

    public void Setup(Order order)
    {
        this.order = order;

        if (iconImage != null)
            iconImage.sprite = order.sprite;
        else
            Debug.LogWarning("iconImage is not assigned in OrderUI.");

        if (capacityText != null)
            capacityText.text = $"{order.currentFill} / {order.requiredCapacity}";
        else
            Debug.LogWarning("capacityText is not assigned in OrderUI.");
    }

    public void UpdateUI()
    {
        if (iconImage != null)
            iconImage.sprite = order.sprite;

        if (capacityText != null)
            capacityText.text = $"Avoid: {order.sprite.name}, Capacity: {order.requiredCapacity}";

        if (capacityText != null)
            capacityText.text = $"{order.currentFill}/{order.requiredCapacity}";
    }
}
