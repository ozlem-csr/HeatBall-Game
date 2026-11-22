using UnityEngine;

public class Brick : MonoBehaviour
{
    public int heat = 0;
    public int maxHeat = 4;

    // X ve Z eksenlerinde tuğlalar arasındaki gerçek mesafe
    public float neighborDistanceX = 2.0f;
    public float neighborDistanceZ = 1.0f;

    public float neighborRadius = 0.65f;

    public Renderer brickRenderer;

    // Tuğla kırılınca çıkacak efekt
    public ParticleSystem particlePrefab;

    private bool destroyed = false;

    void Start()
    {
        if (brickRenderer == null)
            brickRenderer = GetComponent<Renderer>();

        UpdateVisual();
    }

    public void AddHeat(int amount)
    {
        if (destroyed)
            return;

        if (heat >= maxHeat)
            return;

        heat += amount;

        UpdateVisual();

        if (heat >= maxHeat)
        {
            BreakBrick();
        }
    }

    void UpdateVisual()
    {
        if (brickRenderer == null)
            brickRenderer = GetComponent<Renderer>();

        if (heat == 0)
        {
            brickRenderer.material.color = Color.blue;
        }
        else if (heat == 1)
        {
            brickRenderer.material.color = Color.yellow;
        }
        else if (heat == 2)
        {
            brickRenderer.material.color = new Color(1f, 0.5f, 0f);
        }
        else
        {
            brickRenderer.material.color = Color.red;
        }
    }

    void BreakBrick()
    {
        if (destroyed)
            return;

        destroyed = true;

        // Komşu tuğlaları ısıt
        HeatNeighbors();

        // Skor sistemi
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(100);
        }

        // 💥 Tuğla kırılma efektini oluştur
        if (particlePrefab != null)
        {
            Instantiate(
                particlePrefab,
                transform.position,
                Quaternion.identity
            );
        }

       

        // Tuğlayı yok et
        Destroy(gameObject);
    }

    void HeatNeighbors()
    {
        // X ekseni = sağ / sol
        Vector3[] directionsX =
        {
            Vector3.right,
            Vector3.left
        };

        // Z ekseni = ön / arka
        Vector3[] directionsZ =
        {
            Vector3.forward,
            Vector3.back
        };

        // X yönündeki komşular
        foreach (Vector3 direction in directionsX)
        {
            Vector3 searchPosition =
                transform.position + direction * neighborDistanceX;

            Collider[] hits =
                Physics.OverlapSphere(searchPosition, neighborRadius);

            foreach (Collider hit in hits)
            {
                Brick neighbor = hit.GetComponent<Brick>();

                if (neighbor != null && neighbor != this)
                {
                    neighbor.AddHeat(1);
                }
            }
        }

        // Z yönündeki komşular
        foreach (Vector3 direction in directionsZ)
        {
            Vector3 searchPosition =
                transform.position + direction * neighborDistanceZ;

            Collider[] hits =
                Physics.OverlapSphere(searchPosition, neighborRadius);

            foreach (Collider hit in hits)
            {
                Brick neighbor = hit.GetComponent<Brick>();

                if (neighbor != null && neighbor != this)
                {
                    neighbor.AddHeat(1);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddHeat(1);
        }
    }
}