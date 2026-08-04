using UnityEngine;

public class HighScoreObserver : MonoBehaviour
{
    private WaveManager waveManager;
    private int highestSavedRound;
    private int currentTrackedRound = -1;

    void Start()
    {
        waveManager = GetComponent<WaveManager>();
        highestSavedRound = PlayerPrefs.GetInt("HighestRound", 1);
    }

    void Update()
    {
        if (waveManager != null)
        {
            if (waveManager.currentRound != currentTrackedRound)
            {
                currentTrackedRound = waveManager.currentRound;
                
                PlayerPrefs.SetInt("LastRound", currentTrackedRound);

                if (currentTrackedRound > highestSavedRound)
                {
                    highestSavedRound = currentTrackedRound;
                    PlayerPrefs.SetInt("HighestRound", highestSavedRound);
                }

                PlayerPrefs.Save();
            }
        }
    }
}