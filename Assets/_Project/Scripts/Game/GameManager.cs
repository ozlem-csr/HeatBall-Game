using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panelleri")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Game Over")]
    public TMP_Text finalScoreText;

    [Header("Win")]
    public TMP_Text winText;
    public GameObject nextLevelButton;

    [Header("Bölüm")]
    public int currentLevel = 1;
    public int totalLevels = 3;

    [HideInInspector]
    public bool gameStarted = false;

    [HideInInspector]
    public bool gameOver = false;

    [HideInInspector]
    public bool levelCompleted = false;

    private BallController ball;

    // Restart sonrası oyunun otomatik başlamasını sağlar.
    private static bool restartAndStartGame = false;

    private void Awake()
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

        Time.timeScale = 1f;

        ball = FindFirstObjectByType<BallController>();
    }

    private void Start()
    {
        if (ball == null)
            ball = FindFirstObjectByType<BallController>();

        // Başlangıç UI durumu
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (hudPanel != null)
            hudPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        gameStarted = false;
        gameOver = false;
        levelCompleted = false;

        // Restart butonundan geldiysek
        // menüyü göstermeden oyunu başlat.
        if (restartAndStartGame)
        {
            restartAndStartGame = false;

            StartGame();
        }
    }

    // START BUTTON
    public void StartGame()
    {
        if (gameOver || levelCompleted)
            return;

        gameStarted = true;
        gameOver = false;

        Time.timeScale = 1f;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        if (ball == null)
            ball = FindFirstObjectByType<BallController>();

        if (ball != null)
        {
            ball.Launch();
        }

        Debug.Log("Bölüm " + currentLevel + " başladı!");
    }

    // CAN KAYBEDİLDİĞİNDE
    public void OnLifeLost(int remainingLives)
    {
        Debug.Log("Kalan can: " + remainingLives);

        if (remainingLives <= 0)
        {
            GameOver();
        }
    }

    // GAME OVER
    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        gameStarted = false;

        if (ball == null)
            ball = FindFirstObjectByType<BallController>();

        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Final skor
        if (finalScoreText != null)
        {
            if (ScoreManager.Instance != null)
            {
                finalScoreText.text =
                    "SKOR: " + ScoreManager.Instance.GetScore();
            }
            else
            {
                finalScoreText.text = "SKOR: 0";
            }
        }

        if (hudPanel != null)
            hudPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

    // TÜM TUĞLALAR KIRILDIĞINDA
    public void LevelCompleted()
    {
        if (levelCompleted || gameOver)
            return;

        // Can kalmış olması gerekiyor.
        if (ball != null && ball.GetCurrentLives() <= 0)
            return;

        levelCompleted = true;
        gameStarted = false;

        if (ball == null)
            ball = FindFirstObjectByType<BallController>();

        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (hudPanel != null)
            hudPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(true);

        // Son bölüm mü?
        if (currentLevel >= totalLevels)
        {
            if (winText != null)
            {
                winText.text = "OYUNU TAMAMLADINIZ!";
            }

            if (nextLevelButton != null)
                nextLevelButton.SetActive(false);
        }
        else
        {
            if (winText != null)
            {
                winText.text =
                    "BÖLÜM " + currentLevel + " TAMAMLANDI!\nKAZANDINIZ!";
            }

            if (nextLevelButton != null)
                nextLevelButton.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("BÖLÜM " + currentLevel + " TAMAMLANDI!");
    }

    // SONRAKİ BÖLÜM BUTTON
    public void NextLevel()
    {
        if (currentLevel >= totalLevels)
            return;

        Time.timeScale = 1f;

        int nextSceneIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextSceneIndex);
    }

    // RESTART BUTTON
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Aynı bölüm tekrar açılacak
        // ve otomatik başlayacak.
        restartAndStartGame = true;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}