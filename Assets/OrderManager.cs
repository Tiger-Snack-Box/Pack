using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private GameObject orderUIPrefab;
    [SerializeField] public Transform orderPanel;
    [SerializeField] private float orderSpawnInterval = 5f;
    [SerializeField] private int capacity = 3; // Max items per order
    [SerializeField] private GridScript gridScript;
    [SerializeField] private GameProgressManager gameProgressManager;


    [Header("Order Lifetime (in seconds)")]
    [SerializeField] private float expireTime = 15f;

    private float spawnTimer;
    private Queue<Order> orderQueue = new Queue<Order>();

    void Start()
    {
        spawnTimer = orderSpawnInterval;
        if (gridScript != null)
        {
            gridScript.OnTilesFadeComplete += OnTilesFadeCompleteHandler;
        }
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            GenerateRandomOrder();
            spawnTimer = orderSpawnInterval;
        }

        UpdateOrders();
    }

    void UpdateOrders()
    {
        List<Transform> toRemove = new List<Transform>();

        foreach (Transform child in orderPanel)
        {
            OrderUI ui = child.GetComponent<OrderUI>();
            if (ui != null)
            {
                ui.order.timeRemaining -= Time.deltaTime;
                ui.UpdateUI();

                if (ui.order.timeRemaining <= 0f)
                {
                    toRemove.Add(child);
                }
            }
        }

        foreach (Transform expired in toRemove)
        {
            OrderUI ui = expired.GetComponent<OrderUI>();
            if (ui != null)
            {
                // Remove the corresponding order from the queue
                List<Order> updatedOrders = new List<Order>(orderQueue);
                if (updatedOrders.Remove(ui.order)) // only removes matching order
                {
                    orderQueue = new Queue<Order>(updatedOrders);
                }

                Debug.Log($"Order expired: {ui.order.sprite.name}");
                Destroy(expired.gameObject);
                gameProgressManager?.LosePoints();
                Debug.Log("Removed Points from ProgressManager");
            }
        }
    }
    public void OnTilesFadeCompleteHandler(Sprite sprite)
    {
        // Find the order by sprite
        Order orderToRemove = null;
        foreach (Order order in orderQueue)
        {
            if (order.sprite == sprite && order.currentFill >= order.requiredCapacity)
            {
                orderToRemove = order;
                break;
            }
        }

        if (orderToRemove != null)
        {
            RemoveOrder(orderToRemove); // Only remove here
        }
    }

    public void ProcessSwipe(List<Sprite> swipedItems)
    {
        if (swipedItems == null || swipedItems.Count == 0) return;

        Order[] orders = orderQueue.ToArray();

        bool anyOrderFulfilled = false;

        foreach (Order order in orders)
        {
            // Skip incompatible orders
            if (swipedItems.Contains(order.sprite))
            {
                gameProgressManager?.LosePoints();
                Debug.Log("Removed Points from ProgressManager");
                continue;
            }

            int availableCapacity = order.requiredCapacity - order.currentFill;

            // Check if swiped count fits capacity
            if (swipedItems.Count <= availableCapacity)
            {
                order.currentFill += swipedItems.Count;
                Debug.Log($"Added {swipedItems.Count} to order {order.sprite.name}, fill {order.currentFill}/{order.requiredCapacity}");

                if (order.currentFill >= order.requiredCapacity)
                {
                    gameProgressManager?.GainPoints();
                    Debug.Log("Added Points to ProgressManager");
                    RemoveOrder(order);  // Remove only full orders
                    anyOrderFulfilled = true;
                }

                // Continue to next order (don't return)
            }
        }

        if (!anyOrderFulfilled)
        {
            Debug.Log("No compatible order found for swipe.");
        }
    }


    public void RemoveOrder(Order order)
    {
        // Rebuild the queue without the order to remove
        var orders = new List<Order>(orderQueue);
        if (orders.Remove(order))
        {
            orderQueue = new Queue<Order>(orders);
        }

        // Destroy UI element matching this order's sprite
        foreach (Transform child in orderPanel)
        {
            OrderUI ui = child.GetComponent<OrderUI>();
            if (ui != null && ui.order == order)
            {
                Destroy(child.gameObject);
                break;
            }
        }

        Debug.Log($"Order {order.sprite.name} fulfilled and removed.");
    }

    void GenerateRandomOrder()
    {
        // Limit to 4 active orders
        if (orderPanel.childCount >= 4)
        {
            Debug.Log("Order queue is full. Skipping spawn.");
            return;
        }

        List<Sprite> spritePool = gridScript.itemSprites;
        if (spritePool == null || spritePool.Count == 0)
        {
            Debug.LogWarning("No item sprites found in GridScript.");
            return;
        }

        Sprite chosenSprite = spritePool[Random.Range(0, spritePool.Count)];
        int randomCapacity = Random.Range(1, capacity + 1);
        float time = expireTime;

        Order newOrder = new Order(chosenSprite, randomCapacity, time);
        orderQueue.Enqueue(newOrder);

        GameObject uiObject = Instantiate(orderUIPrefab, orderPanel);
        OrderUI uiScript = uiObject.GetComponent<OrderUI>();
        uiScript.Setup(newOrder);
    }

    public Transform GetCompatibleOrderTarget(List<Sprite> swipedItems)
    {
        foreach (Transform child in orderPanel)
        {
            OrderUI ui = child.GetComponent<OrderUI>();
            if (ui == null) continue;

            Order order = ui.order;
            if (swipedItems.Contains(order.sprite)) continue; // incompatible

            int availableCapacity = order.requiredCapacity - order.currentFill;
            if (swipedItems.Count <= availableCapacity)
            {
                return ui.transform; // return the UI's position
            }
        }

        return null; // no compatible order
    }

    public Order ProcessSwipeGetOrder(List<Sprite> swipedItems)
    {
        if (swipedItems == null || swipedItems.Count == 0) return null;

        Order[] orders = orderQueue.ToArray();

        foreach (Order order in orders)
        {
            if (swipedItems.Contains(order.sprite))
            {
                gameProgressManager?.LosePoints();
                Debug.Log("Removed Points from ProgressManager");
                continue;
            }

            int availableCapacity = order.requiredCapacity - order.currentFill;

            if (swipedItems.Count <= availableCapacity)
            {
                order.currentFill += swipedItems.Count;
                Debug.Log($"Added {swipedItems.Count} to order {order.sprite.name}, fill {order.currentFill}/{order.requiredCapacity}");

                if (order.currentFill >= order.requiredCapacity)
                {
                    gameProgressManager?.GainPoints();
                    Debug.Log("Added Points to ProgressManager");
                    // Do NOT remove here, removal deferred until fade completes
                }

                return order;  // Return the matched order (may or may not be full)
            }
        }

        Debug.Log("No compatible order found for swipe.");
        return null;
    }

    public void FulfillOrder(Sprite sprite)
    {
        if (orderQueue.Count == 0) return;

        Order[] orders = orderQueue.ToArray();

        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i].sprite == sprite)
            {
                orderQueue = new Queue<Order>(orders);
                orderQueue.Dequeue();

                foreach (Transform child in orderPanel)
                {
                    OrderUI ui = child.GetComponent<OrderUI>();
                    if (ui != null && ui.order.sprite == sprite)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }

                Debug.Log($"Fulfilled: {sprite.name}");
                return;
            }
        }

        Debug.Log("No matching order found.");
    }
}
