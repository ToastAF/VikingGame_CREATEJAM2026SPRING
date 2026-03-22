using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    public void GoToStartScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
