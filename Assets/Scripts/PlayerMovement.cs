using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float runSpeed = 6f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Configuración de Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private Transform attackPoint; // Punto desde donde se lanza el bumerán/ataque

    [Header("Configuración de Bumerán")]
    [SerializeField] private float boomerangCooldown = 5f;
    private bool canThrowBoomerang = true;
    private Coroutine boomerangCooldownCoroutine;

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private bool isRunning = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1); // Por defecto mirando abajo

    // Referencia al script de combate cuerpo a cuerpo
    private CombateCaC combateCaC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combateCaC = GetComponent<CombateCaC>();
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
            return;
        }

        // Input de Lanzar Bumerán (Tecla R)
        if (Input.GetKeyDown(KeyCode.R) && canThrowBoomerang)
        {
            StartCoroutine(AttackCoroutine(false)); // false indica que es lanzar bumerán
            return;
        }

        // Input de Dash (Cualquier Tecla Control)
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }

        // Input de Correr (Shift Izquierdo)
        isRunning = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("isRunning", isRunning);
    }

    /// <summary>
    /// Lee el input de movimiento y actualiza los parámetros del Animator para los Blend Trees.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

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

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    /// <summary>
    /// Aplica el movimiento al Rigidbody en el ciclo de físicas.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDashing || isAttacking) return;

        float currentSpeed = isRunning ? runSpeed : speed;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Corutina que gestiona la lógica de ataque (animación y ejecución).
    /// </summary>
    /// <param name="isMelee">True si es ataque cuerpo a cuerpo, False si es lanzar bumerán.</param>
    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true; // Flag interno del script para detener el movimiento

        // 1. Apunta la animación
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);

        // Espera un frame para que el Animator actualice los floats
        yield return null;

        // 2. Activa la animación y la lógica de daño
        if (isMelee)
        {
            animator.SetTrigger("MeleeAttack"); // Activa el Trigger de golpe
            yield return new WaitForSeconds(0.1f); // Pequeño delay
            if (combateCaC != null)
            {
                combateCaC.EjecutarGolpe();
            }
        }
        else // Lanzar bumerán
        {
            animator.SetTrigger("BoomerangThrow"); // Activa el Trigger de bumerán
            yield return new WaitForSeconds(0.1f); // Pequeño delay
            if (boomerangPrefab != null && attackPoint != null)
            {
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);

                canThrowBoomerang = false;
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }

        // 3. Espera a que termine la duración del ataque
        yield return new WaitForSeconds(attackDuration - 0.1f);

        // 4. Volver al estado normal
        isAttacking = false; // Permite el movimiento de nuevo
    }

    /// <summary>
    /// Corutina que espera el tiempo de cooldown del bumerán.
    /// </summary>
    private IEnumerator BoomerangCooldownCoroutine()
    {
        yield return new WaitForSeconds(boomerangCooldown);
        canThrowBoomerang = true;
        boomerangCooldownCoroutine = null;
    }

    /// <summary>
    /// Método público llamado por el bumerán para reiniciar el cooldown al ser recogido.
    /// </summary>
    public void ResetBoomerangCooldown()
    {
        if (boomerangCooldownCoroutine != null)
        {
            StopCoroutine(boomerangCooldownCoroutine);
            boomerangCooldownCoroutine = null;
        }
        canThrowBoomerang = true;
        Debug.Log("¡Bumerán recogido! Cooldown reiniciado.");
    }

    /// <summary>
    /// Corutina que gestiona la lógica del Dash.
    /// </summary>
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        Vector2 dashDirection = moveInput;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}