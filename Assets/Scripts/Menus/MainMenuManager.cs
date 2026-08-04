using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menus")]
    public GameObject mainMenuPanel; 
    public GameObject controlsPanel; 
    
    [Header("Story/Info Panels Sequence")]
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;
    public GameObject stuffpanel;
    
    [Header("Settings")]
    public string gameSceneName = "Level1"; 

    public void PlayGame()
    {   
        if (panel1 != null) panel1.SetActive(true);
    }

    public void GoToPanel2()
    {
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(true);
    }

    public void GoToPanel3()
    {
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(true);
    }

    public void GoToPanel4()
    {
        if (panel3 != null) panel3.SetActive(false);
        if (panel4 != null) panel4.SetActive(true);
    }

    public void GoToStuffPanel()
    {
        if (panel4 != null) panel4.SetActive(false);
        if (stuffpanel != null) stuffpanel.SetActive(true);
    }

    public void okiee()
    {
        SceneManager.LoadScene(gameSceneName);
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
        
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);
        if (panel4 != null) panel4.SetActive(false);
        if (stuffpanel != null) stuffpanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}