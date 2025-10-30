using System.Collections.Generic;
using UnityEngine;

public class SupportEnemy : MonoBehaviour
{
    [Header("Configuración de Buff")]
    [SerializeField] private float buffAmount = 20f;       // +20 HP extra o escudo
    [SerializeField] private bool grantShield = false;     // si prefieres escudo
    [SerializeField] private float checkRadius = 15f;      // radio para encontrar enemigos
    [SerializeField] private LayerMask enemyLayer;         // capa de enemigos

    private List<Enemigo> buffedEnemies = new List<Enemigo>();
    private bool isAlive = true;

    void Start()
    {
        // Dar buff al inicio
        ApplyBuffToNearbyEnemies();
    }

    private void ApplyBuffToNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Enemigo e = hit.GetComponent<Enemigo>();
            if (e != null && e != GetComponent<Enemigo>())
            {
                if (!buffedEnemies.Contains(e))
                {
                    buffedEnemies.Add(e);
                    e.ApplyShield(buffAmount, grantShield);
                }
            }
        }
    }

    private void RemoveBuffFromEnemies()
    {
        foreach (var e in buffedEnemies)
        {
            if (e != null)
                e.RemoveShield(buffAmount, grantShield);
        }
        buffedEnemies.Clear();
    }

    // Llamar este método cuando muera
    public void OnDeath()
    {
        if (!isAlive) return;
        isAlive = false;

        RemoveBuffFromEnemies();

        // Aquí puedes añadir animación de muerte o efectos visuales
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}

