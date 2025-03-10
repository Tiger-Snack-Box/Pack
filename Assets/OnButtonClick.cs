using UnityEngine;
using UnityEngine.SceneManagement;

public class OnButtonClick : MonoBehaviour
{
    [SerializeField] private string nextPage = "nextPage";

    public void playButton()
    {
        SceneManager.LoadScene(nextPage);
    }
}
