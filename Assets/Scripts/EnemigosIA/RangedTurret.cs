using UnityEngine;

public class RangedTurret : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private int projectileDamage = 10;

    private float fireCooldown = 0f;
    private Transform jugador;
    private bool puedeDisparar = false;
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
        else
            Debug.LogError("[RangedTurret] No se encontró objeto con tag 'Player'.");
    }

    void Update()
    {
        if (jugador == null) return;

        // Rotar hacia el jugador
        Vector2 dir = jugador.position - transform.position;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg)- 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Control de disparo
        if (puedeDisparar)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Disparar(dir.normalized);
                fireCooldown = 1f / Mathf.Max(fireRate, 0.0001f);
            }
        }
    }

    private void Disparar(Vector2 direction)
    {
        if (projectilePrefab == null || shootPoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector2 dir = (jugador.position - shootPoint.position).normalized;
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetDirection(dir);
            p.ownerTag = gameObject.tag;
            p.damage = projectileDamage;
            p.ownerTag = "Enemy";
        }
        
    }

    public void SetPuedeDisparar(bool value)
    {
        puedeDisparar = value;
    }
}
