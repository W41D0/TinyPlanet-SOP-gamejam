using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel; 
    public GameObject controlsPanel; 
    public GameObject stuffpanel;
    public string gameSceneName = "Level1"; 

    public void PlayGame()
    {   
        if (stuffpanel != null) stuffpanel.SetActive(true);
    }

    public void ShowControlsPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void ShowMainMenuPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }
    public void okiee()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}