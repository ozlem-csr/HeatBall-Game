using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public TMP_Text livesText;
    public BallController ball;

    private void Start()
    {
        UpdateLivesUI();
    }

    private void Update()
    {
        UpdateLivesUI();
    }

    private void UpdateLivesUI()
    {
        if (ball == null || livesText == null)
            return;

        int lives = ball.GetCurrentLives();

        string hearts = "";

        for (int i = 0; i < lives; i++)
        {
            hearts += "♥";
        }

        livesText.text = "CAN: " + hearts;
    }
}