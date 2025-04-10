using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class pauseMenuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    public void pauseMenu()
    {
        SceneManager.LoadSceneAsync(2);
    }

}
