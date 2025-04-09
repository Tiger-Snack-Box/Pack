using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public GameObject orderUIPrefab;
    public Transform orderPanel;
    public float orderSpawnInterval = 5f;

    private float spawnTimer;
    private Queue<Order> orderQueue = new Queue<Order>();
    private string[] itemPool = { "Burger", "Fries", "Pizza", "Soda", "Salad" };

    void Start()
    {
        spawnTimer = orderSpawnInterval;
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
            orderQueue = new Queue<Order>(orderQueue.ToArray()); // Rebuild queue without that order
            Debug.Log($"Order expired: {ui.order.itemName}");
            Destroy(expired.gameObject);
        }
    }


    void GenerateRandomOrder()
    {
        string item = itemPool[Random.Range(0, itemPool.Length)];
        float time = Random.Range(10f, 20f);
        Order newOrder = new Order(item, time);
        orderQueue.Enqueue(newOrder);

        GameObject uiObject = Instantiate(orderUIPrefab, orderPanel);
        OrderUI uiScript = uiObject.GetComponent<OrderUI>();
        uiScript.Setup(newOrder);
    }

    public void FulfillOrder(string itemName)
    {
        if (orderQueue.Count == 0) return;

        Order[] orders = orderQueue.ToArray();

        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i].itemName == itemName)
            {
                orderQueue = new Queue<Order>(orders);
                orderQueue.Dequeue(); // Remove fulfilled order

                foreach (Transform child in orderPanel)
                {
                    OrderUI ui = child.GetComponent<OrderUI>();
                    if (ui != null && ui.order.itemName == itemName)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }

                Debug.Log($"Fulfilled: {itemName}");
                return;
            }
        }

        Debug.Log("No matching order found.");
    }

    void RemoveOrder(Order order)
    {
        orderQueue = new Queue<Order>(orderQueue.ToArray());
        Debug.Log($"Order expired: {order.itemName}");
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 160, 30), "Fulfill First Order"))
        {
            if (orderQueue.Count > 0)
            {
                string itemToFulfill = orderQueue.Peek().itemName;
                FulfillOrder(itemToFulfill);
            }
        }
    }


}