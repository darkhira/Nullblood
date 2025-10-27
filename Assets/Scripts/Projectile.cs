using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 5f;
    public float speed = 8f;
    public string ownerTag = "Enemigo";
    public LayerMask environmentMask;
    private Rigidbody2D rb;
    private Vector2 direction;

    public void Initialize(Vector2 direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;

        // Rotar sprite hacia la dirección de disparo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        
    }
    void Start()
    {
        //obtener los componentes necesarios
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        // si se sobrepasa la lifetime el projectil es destruido
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisiones con el que disparó
        if (other.CompareTag(ownerTag)) return;

        // Si golpea al jugador
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.TakeDamage(damage, gameObject);
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
        // Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);
    }
}
