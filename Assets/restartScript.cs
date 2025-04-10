using UnityEngine;
using UnityEngine.SceneManagement;

public class restartScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void restartScene()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
