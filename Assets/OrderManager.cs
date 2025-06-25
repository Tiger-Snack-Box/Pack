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
            Destroy(expired.gameObject);
            orderQueue = new Queue<Order>(orderQueue.ToArray()); // clean up queue
            Debug.Log("? Order expired.");
        }
    }

    void GenerateRandomOrder()
    {
        List<int> snackTypes = new List<int>();
        int count = Random.Range(1, 4); // 1–3 snacks per order

        for (int i = 0; i < count; i++)
        {
            int snackType = Random.Range(1, 5); // Assuming 1–4 are valid snack types
            snackTypes.Add(snackType);
        }

        float time = Random.Range(10f, 20f);
        Order newOrder = new Order(snackTypes, time);
        orderQueue.Enqueue(newOrder);

        GameObject uiObject = Instantiate(orderUIPrefab, orderPanel);
        OrderUI uiScript = uiObject.GetComponent<OrderUI>();
        uiScript.Setup(newOrder);
    }

    public bool TryFulfill(List<int> collectedSnackTypes)
    {
        if (orderQueue.Count == 0) return false;

        Order current = orderQueue.Peek();

        if (current.snackTypes.Count != collectedSnackTypes.Count)
            return false;

        List<int> a = new List<int>(current.snackTypes);
        List<int> b = new List<int>(collectedSnackTypes);
        a.Sort();
        b.Sort();

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }

        FulfillOrder();
        return true;
    }

    public void FulfillOrder()
    {
        if (orderQueue.Count == 0) return;

        Order fulfilled = orderQueue.Dequeue();

        foreach (Transform child in orderPanel)
        {
            OrderUI ui = child.GetComponent<OrderUI>();
            if (ui != null && ui.order == fulfilled)
            {
                Destroy(child.gameObject);
                break;
            }
        }

        Debug.Log("Fulfilled order: " + string.Join(", ", fulfilled.snackTypes));
    }

    public Order PeekOrder()
    {
        return orderQueue.Count > 0 ? orderQueue.Peek() : null;
    }
}
