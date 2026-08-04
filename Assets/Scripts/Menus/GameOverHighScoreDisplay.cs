using UnityEngine;
using TMPro;

public class GameOverHighScoreDisplay : MonoBehaviour
{
    [Header("High Score UI")]
    public TextMeshProUGUI highScoreText;
    public string highScorePrefix = "HIGHEST ROUND: ";

    [Header("Last Run UI")]
    public TextMeshProUGUI lastRoundText;
    public string lastRoundPrefix = "SURVIVED ROUNDS: ";

    void Start()
    {
        if (highScoreText != null)
        {
            if (PlayerPrefs.HasKey("HighestRound"))
            {
                int highestRound = PlayerPrefs.GetInt("HighestRound");
                highScoreText.text = highScorePrefix + highestRound;
            }
            else
            {
                highScoreText.text = highScorePrefix + "__";
            }
        }

        if (lastRoundText != null)
        {
            if (PlayerPrefs.HasKey("LastRound"))
            {
                int lastRound = PlayerPrefs.GetInt("LastRound");
                lastRoundText.text = lastRoundPrefix + lastRound;
            }
            else
            {
                lastRoundText.text = lastRoundPrefix + "__";
            }
        }
    }
}