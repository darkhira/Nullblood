using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeZone : MonoBehaviour
{
    public float damage = 15f;
    public float warningDuration = 1.0f; // Tiempo de advertencia
    public float activeDuration = 0.5f; // Tiempo que la zona hace daño

    private Collider2D damageCollider;
    private SpriteRenderer visual;
    private List<PlayerStats> hitPlayers = new List<PlayerStats>();

    void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
        visual = GetComponent<SpriteRenderer>();
        damageCollider.enabled = false;
    }

    void Start()
    {
        StartCoroutine(ActivateZone());
    }

    private IEnumerator ActivateZone()
    {
        // 1. Fase de Advertencia
        visual.color = Color.yellow;
        yield return new WaitForSeconds(warningDuration);

        // 2. Fase Activa (Daño)
        visual.color = Color.red;
        damageCollider.enabled = true;

        yield return new WaitForSeconds(activeDuration);

        // 3. Desaparición
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null && !hitPlayers.Contains(player))
            {
                player.TakeDamage(damage, gameObject);
                hitPlayers.Add(player);
            }
        }
    }
}