using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private bool levelFinished = false;

    private void Update()
    {
        if (levelFinished)
            return;

        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.gameStarted)
            return;

        if (GameManager.Instance.gameOver)
            return;

        CheckLevelComplete();
    }

    private void CheckLevelComplete()
    {
        Brick[] bricks =
            FindObjectsByType<Brick>(
                FindObjectsSortMode.None
            );

        // En az bir tuğla varsa oyun devam ediyor.
        if (bricks.Length > 0)
            return;

        levelFinished = true;

        Debug.Log("Bütün tuğlalar kırıldı!");

        GameManager.Instance.LevelCompleted();
    }
}