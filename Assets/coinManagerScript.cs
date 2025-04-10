using UnityEngine;

public class coinManagerScript : MonoBehaviour
{
    public const string Coins = "Coins";
    public static int coins = 0;
    void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
    }
    public static void updateCoins()
    {
        PlayerPrefs.SetInt("Coins", coins);
        coins = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.Save();
    }
}
