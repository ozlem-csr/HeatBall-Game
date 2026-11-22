using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [Header("Hareket")]
    public float moveSpeed = 10f;

    [Header("Top Sekme")]
    public float maxBounceAngle = 60f;

    [Header("Arena Duvarları")]
    public Collider leftWall;
    public Collider rightWall;

    private Rigidbody rb;
    private Collider paddleCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        paddleCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        // Oyun başlamadıysa raket hareket etmesin
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.gameStarted)
            return;

        // Game Over olduysa raket hareket etmesin
        if (GameManager.Instance.gameOver)
            return;

        float direction = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                direction = -1f;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                direction = 1f;
            }
        }

        Vector3 newPosition = rb.position;

        newPosition.x += direction * moveSpeed * Time.fixedDeltaTime;

        float minX = leftWall.bounds.max.x + paddleCollider.bounds.extents.x;
        float maxX = rightWall.bounds.min.x - paddleCollider.bounds.extents.x;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        rb.MovePosition(newPosition);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();

        if (ballRb == null)
            return;

        Vector3 hitPoint = collision.GetContact(0).point;

        float paddleLeft = paddleCollider.bounds.min.x;
        float paddleRight = paddleCollider.bounds.max.x;

        float hitPercent = Mathf.InverseLerp(
            paddleLeft,
            paddleRight,
            hitPoint.x
        );

        float hitOffset = (hitPercent - 0.5f) * 2f;

        float bounceAngle = hitOffset * maxBounceAngle;

        float angleInRadians = bounceAngle * Mathf.Deg2Rad;

        Vector3 newDirection = new Vector3(
            Mathf.Sin(angleInRadians),
            0f,
            Mathf.Cos(angleInRadians)
        );

        float currentSpeed = ballRb.linearVelocity.magnitude;

        ballRb.linearVelocity = newDirection.normalized * currentSpeed;
    }
}