using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TMP_Text scoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;

        UpdateScoreUI();

        Debug.Log("SKOR: " + score);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SKOR: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }
}