using UnityEngine;
using TMPro;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI strikesText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (highScoreText)
        {
            highScoreText.text = $"BEST: {DataManager.s_instance.highScore}";
        }

        GameplayManager.OnScoreUpdated += UpdateScoreText;
        GameplayManager.OnStrikeUpdated += UpdateStrikesText;
        GameplayManager.OnLivesUpdated += UpdateLivesText;
        GameplayManager.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        GameplayManager.OnScoreUpdated -= UpdateScoreText;
        GameplayManager.OnStrikeUpdated -= UpdateStrikesText;
        GameplayManager.OnLivesUpdated -= UpdateLivesText;
        GameplayManager.OnGameOver -= ShowGameOver;
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText) scoreText.text = newScore.ToString();
    }

    private void UpdateStrikesText(int strikes)
    {
        if (strikesText)
        {
            if (strikes >= 2)
            {
                strikesText.text = $"STREAK x{strikes}!";
                strikesText.color = Color.yellow;
            }
            else if (strikes > 0)
            {
                strikesText.text = $"Perfect!";
                strikesText.color = Color.green;
            }
            else
            {
                strikesText.text = "";
            }
        }
    }

    private void UpdateLivesText(int currentLives)
    {
        if (livesText)
        {
            livesText.text = $"LIVES: {currentLives}";
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }
}