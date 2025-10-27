using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 5f;
    public float speed = 8f;
    public string ownerTag = "Enemy";
    public LayerMask environmentMask;
    private Rigidbody2D rb;
    private Vector2 direction;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisiones con el que disparó
        if (other.CompareTag(ownerTag)) return;

        // Si golpea al jugador
        if (other.CompareTag("Player"))
        {
            VidaJugador vida = other.GetComponent<VidaJugador>();
            if (vida != null)
            {
                vida.TomarDaño(damage);
            }

            Destroy(gameObject);
            return;
        }

        // Si golpea el entorno (opcional)
        if (environmentMask != 0)
        {
            if (((1 << other.gameObject.layer) & environmentMask) != 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        // En cualquier otro caso, se destruye
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }
}
