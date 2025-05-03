using UnityEngine;
using UnityEngine.SceneManagement;

public class OnButtonClick : MonoBehaviour
{
    [SerializeField] private string nextPage = "worldMap";

    public void playButton()
    {
        SceneManager.LoadScene(nextPage);
    }
}
