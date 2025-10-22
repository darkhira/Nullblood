using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class FollowIA : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    private Transform jugador;
    [SerializeField] private float minDistance = 1.5f;

    // Nueva Referencia
    private Rigidbody2D rb;

    [Header("Animaciones")]
    private Animator animator;
    private Vector2 movimiento;
    private bool isFacingRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            jugador = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontr ningn objeto con el tag 'Player'. La IA no se movera.");
        }
    }

    void Update()
    {
        if (jugador == null) return;

        // ... (código existente) ...

        // Calcular direccin hacia el jugador
        Vector2 direccion = (jugador.position - transform.position).normalized;

        // Si el enemigo est muy cerca, se detiene
        if (Vector2.Distance(transform.position, jugador.position) > minDistance)
        {
            movimiento = direccion;
        }
        else
        {
            movimiento = Vector2.zero;
            Atacar();
        }

        // Actualizar animaciones (Blend Tree)
        animator.SetFloat("MoveX", movimiento.x);
        animator.SetFloat("MoveY", movimiento.y);
        animator.SetFloat("Speed", movimiento.sqrMagnitude);

        // ... (código existente de Flip/Giro si está descomentado) ...
    }

    void FixedUpdate()
    {
        if (jugador == null) return;

        // ... (código existente) ...
        if (movimiento != Vector2.zero)
        {
            rb.linearVelocity = movimiento * velocidad;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Atacar()
    {
        //Debug.Log("Atacando al jugador");
    }

    // **NUEVO MÉTODO:** Detiene el movimiento de la IA
    public void DesactivarIA()
    {
        // 1. Detener el Rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // Opcional: Desactivar la gravedad o hacerlo cinemático si es necesario.
            // rb.isKinematic = true; 
        }

        // 2. Desactivar el script (detiene Update/FixedUpdate)
        enabled = false;
    }

    // ... (El resto de la funcin Flip permanece igual) ...
    private void Flip(bool movingRight)
    {
        if (isFacingRight != movingRight)
        {
            isFacingRight = movingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
}