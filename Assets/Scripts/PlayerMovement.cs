using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Configuraciï¿½n de Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private Transform attackPoint; // Punto desde donde se lanza el bumerán/ataque

    [Header("Configuración de Bumerán")]
    [SerializeField] private float boomerangCooldown = 5f; // Tiempo de espera para volver a lanzar
    private bool canThrowBoomerang = true; // Controla si podemos lanzar
    private Coroutine boomerangCooldownCoroutine; // Referencia a la corutina de cooldown

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1); // Por defecto mirando abajo

    // **NUEVA VARIABLE: Guarda la ï¿½ltima direcciï¿½n de movimiento vï¿½lida**
    private Vector2 lastMoveDirection = new Vector2(0, -1); // Por defecto: mirando hacia abajo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combateCaC = GetComponent<CombateCaC>(); // Obtiene la referencia al script de combate
    }

    void Update()
    {
        // Si está haciendo dash o atacando, no procesa nuevo input
        if (isDashing || isAttacking)
        {
            return;
        }

        // Procesa el input de movimiento y actualiza la animación base
        HandleMovementInput();

        // Input de Ataque Cuerpo a Cuerpo (Clic izquierdo)
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackCoroutine(true)); // true indica que es ataque melee
            return; // Sale del Update para evitar procesar otros inputs en el mismo frame
        }

        // Input de Lanzar Bumerán (Tecla R) y comprobación del cooldown
        if (Input.GetKeyDown(KeyCode.R) && canThrowBoomerang)
        {
            StartCoroutine(AttackCoroutine(false)); // false indica que es lanzar bumerán
            return; // Sale del Update
        }

        // Input de Dash (Shift Izquierdo)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    /// <summary>
    /// Lee el input de movimiento y actualiza los parámetros del Animator para el Blend Tree.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // Guarda la última dirección en la que se movió el jugador
        if (moveInput.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

        // Actualiza los parámetros del Animator para el Blend Tree de Idle/Walk
        if (moveInput.sqrMagnitude > 0.1f) // Si se está moviendo
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
        else // Si está quieto, usa la última dirección
        {
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }

        // Actualiza el parámetro Speed para la transición Idle <-> Walk
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    /// <summary>
    /// Aplica el movimiento al Rigidbody en el ciclo de físicas.
    /// </summary>
    private void FixedUpdate()
    {
        // No mueve al jugador si está haciendo dash o atacando
        if (isDashing || isAttacking)
        {
            return;
        }

        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Corutina que gestiona la lógica de ataque (animación y ejecución).
    /// </summary>
    /// <param name="isMelee">True si es ataque cuerpo a cuerpo, False si es lanzar bumerán.</param>
    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true;

        // Orienta la animación de ataque según la última dirección
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetBool("isAttacking", true); // Activa el estado de ataque en el Animator

        // Espera un breve momento para sincronizar con la animación (opcional)
        yield return new WaitForSeconds(0.1f);

        // Ejecuta la lógica de daño correspondiente
        if (isMelee)
        {
            if (combateCaC != null)
            {
                combateCaC.EjecutarGolpe(); // Llama al script de combate CaC
            }
        }
        else // Lanzar bumerán
        {
            if (boomerangPrefab != null && attackPoint != null && canThrowBoomerang) // Comprobación añadida
            {
                // Instancia el bumerán en el punto de ataque
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                // Llama al método Throw del bumerán para lanzarlo
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);

                // Inicia el cooldown del bumerán
                canThrowBoomerang = false;
                // Guarda la referencia a la corutina para poder cancelarla si se recoge
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }

        // Espera el resto de la duración de la animación
        yield return new WaitForSeconds(attackDuration - 0.1f);

        // Desactiva el estado de ataque
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    /// <summary>
    /// Corutina que espera el tiempo de cooldown del bumerán.
    /// </summary>
    private IEnumerator BoomerangCooldownCoroutine()
    {
        // ... (Tu cï¿½digo de Dash)
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(boomerangCooldown);
        canThrowBoomerang = true;
        boomerangCooldownCoroutine = null; // Limpia la referencia
    }

    /// <summary>
    /// Método público llamado por el bumerán para reiniciar el cooldown al ser recogido.
    /// </summary>
    public void ResetBoomerangCooldown()
    {
        if (boomerangCooldownCoroutine != null)
        {
            StopCoroutine(boomerangCooldownCoroutine); // Detiene la corutina de espera
            boomerangCooldownCoroutine = null;
        }
        canThrowBoomerang = true; // Permite lanzar de nuevo
        Debug.Log("¡Bumerán recogido! Cooldown reiniciado.");
    }

    /// <summary>
    /// Corutina que gestiona la lógica del Dash.
    /// </summary>
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        Vector2 dashDirection = moveInput; // Usa la dirección actual del input
        float startTime = Time.time;

        // Bucle que dura lo que dure el dash
        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate(); // Espera al siguiente frame de físicas
        }

        isDashing = false;

        // Espera el cooldown antes de poder volver a hacer dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}