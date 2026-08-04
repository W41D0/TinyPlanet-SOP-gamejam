using UnityEngine;
using TMPro;

public class MainMenuHighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public string prefixText = "HIGHEST ROUND: ";

    void Start()
    {
        if (highScoreText != null)
        {
            if (PlayerPrefs.HasKey("HighestRound"))
            {
                int highestRound = PlayerPrefs.GetInt("HighestRound");
                highScoreText.text = prefixText + highestRound;
            }
            else
            {
                highScoreText.text = prefixText + "__"; 
            }
        }
    }
}