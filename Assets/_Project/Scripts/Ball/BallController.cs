using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Top Ayarları")]
    public float launchForce = 12f;
    public Vector3 launchDirection = new Vector3(0f, 0f, 1f);

    [Header("Başlangıç Noktası")]
    public Transform startPoint;

    [Header("Can Sistemi")]
    public int maxLives = 3;

    [Header("Ölüm Alanı")]
    public string deathZoneTag = "BallDeathZone";

    [Header("🔥 Top Efekti")]
    public ParticleSystem fireEffect;

    private int currentLives;

    private Rigidbody rb;

    private bool launched = false;
    private bool losingLife = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        currentLives = maxLives;

        // Oyun başlarken efekt kapalı.
        StopFireEffect();
    }

    private void Start()
    {
        ResetBall();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        // Start butonuna basılmadıysa hiçbir şey yapma.
        if (!GameManager.Instance.gameStarted)
            return;

        if (GameManager.Instance.gameOver)
            return;

        if (GameManager.Instance.levelCompleted)
            return;

        // Space sadece oyun başladıktan sonra çalışır.
        if (!launched &&
            !losingLife &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Launch();
        }
    }

    public void Launch()
    {
        if (launched || losingLife)
            return;

        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.gameStarted)
                return;

            if (GameManager.Instance.gameOver)
                return;

            if (GameManager.Instance.levelCompleted)
                return;
        }

        launched = true;

        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity =
            launchDirection.normalized * launchForce;

        // 🔥 Top fırlatılınca ateş başlasın.
        PlayFireEffect();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!launched || losingLife)
            return;

        if (other.CompareTag(deathZoneTag))
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        if (losingLife)
            return;

        losingLife = true;
        launched = false;

        // 🔥 Top kaybedildi.
        StopFireEffect();

        currentLives--;

        Debug.Log(
            "Top kaybedildi! Kalan can: "
            + currentLives
        );

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLifeLost(currentLives);
        }

        // Can kalmadıysa Game Over.
        if (currentLives <= 0)
        {
            GameOver();
            return;
        }

        // Can kaldıysa başlangıç noktasına dön.
        ResetBall();

        losingLife = false;
    }

    public void ResetBall()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = false;

        if (startPoint != null)
        {
            rb.position = startPoint.position;
        }

        transform.rotation = Quaternion.identity;

        launched = false;

        // 🔥 Başlangıçta beklerken efekt kapalı.
        StopFireEffect();
    }

    private void GameOver()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        launched = false;

        // 🔥 Game Over'da efekt kapalı.
        StopFireEffect();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    // =========================
    // 🔥 EFEKT KONTROLLERİ
    // =========================

    private void PlayFireEffect()
    {
        if (fireEffect != null)
        {
            fireEffect.Play();
        }
    }

    private void StopFireEffect()
    {
        if (fireEffect != null)
        {
            fireEffect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }
}