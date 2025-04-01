using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickButton : MonoBehaviour
{
    public void LoadNextPage()
    {
        SceneManager.LoadScene("next-page");
    }
}
