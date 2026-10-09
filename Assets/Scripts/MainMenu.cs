
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayDrivingGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MadDriver");
    }

    public void PlayFlyingGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Flylikeabird");
    }

    public void PlaySumoGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SumoGame");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
    }
}
