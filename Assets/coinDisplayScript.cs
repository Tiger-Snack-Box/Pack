using UnityEngine;
using UnityEngine.UI;
public class coinDisplayScript : MonoBehaviour
{
    private Text text;
    void Start()
    {
        text =GetComponent<Text>();
    }

    void Update()
    {
        string[] temp = text.text.Split('$');
        text.text = temp[0] + "$" + coinManagerScript.coins;

    }
}
