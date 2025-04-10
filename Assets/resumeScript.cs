using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class resumeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void resumeGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
