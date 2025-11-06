using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerSoundController playerSoundController;

    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float runSpeed = 6f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Configuraci�n de Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Configuraci�n de Ataque")]
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private Transform attackPoint; // Punto desde donde se lanza el bumer�n/ataque

    [Header("Configuraci�n de Bumer�n")]
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
        playerSoundController = GetComponent<PlayerSoundController>();
    }

    void Update()
    {
        // Si est� haciendo dash o atacando, no procesa nuevo input
        if (isDashing || isAttacking)
        {
            return;
        }

        // Procesa el input de movimiento y actualiza la animaci�n base
        HandleMovementInput();

        // --- �CONTROLES ACTUALIZADOS! ---

        // Input de Ataque Cuerpo a Cuerpo (Clic izquierdo o Bot�n 'X' de Xbox)
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            playerSoundController.playsonidoGolpe();
            StartCoroutine(AttackCoroutine(true)); // true indica que es ataque melee
            return;
        }

        // Input de Lanzar Bumer�n (Tecla R o Bot�n 'Y' de Xbox)
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3)) && canThrowBoomerang)
        {
            playerSoundController.playsonidoBoomerang();
            StartCoroutine(AttackCoroutine(false)); // false indica que es lanzar bumer�n
            return;
        }

        // Input de Dash (Barra Espaciadora o Bot�n 'A' de Xbox)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            playerSoundController.playsonidoDash();
            StartCoroutine(DashCoroutine());
        }

        // Input de Correr (Shift Izquierdo o Bot�n 'LB' de Xbox)
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);
        animator.SetBool("isRunning", isRunning);
    }

    /// <summary>
    /// Lee el input de movimiento y actualiza los par�metros del Animator para los Blend Trees.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // Esto ya funciona con mando
        float moveY = Input.GetAxisRaw("Vertical"); // Esto ya funciona con mando
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

        if (moveInput.sqrMagnitude > 0.1f) // Si se est� moviendo
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
        else // Si est� quieto, usa la �ltima direcci�n
        {
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    /// <summary>
    /// Aplica el movimiento al Rigidbody en el ciclo de f�sicas.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDashing || isAttacking) return;

        float currentSpeed = isRunning ? runSpeed : speed;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Corutina que gestiona la l�gica de ataque (animaci�n y ejecuci�n).
    /// </summary>
    /// <param name="isMelee">True si es ataque cuerpo a cuerpo, False si es lanzar bumer�n.</param>
    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true; // Flag interno del script para detener el movimiento

        // 1. Apunta la animaci�n
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);

        // Espera un frame para que el Animator actualice los floats
        yield return null;

        // 2. Activa la animaci�n y la l�gica de da�o
        if (isMelee)
        {
            animator.SetTrigger("MeleeAttack"); // Activa el Trigger de golpe
            yield return new WaitForSeconds(0.1f); // Peque�o delay
            if (combateCaC != null)
            {
                // --- �CORRECCI�N IMPORTANTE! ---
                // Tu script anterior llamaba a EjecutarGolpe() sin argumentos.
                // Esta versi�n le pasa la direcci�n para que el golpe sea direccional.
                combateCaC.EjecutarGolpe(lastMoveDirection);
            }
        }
        else // Lanzar bumer�n
        {
            animator.SetTrigger("BoomerangThrow"); // Activa el Trigger de bumer�n
            yield return new WaitForSeconds(0.1f); // Peque�o delay
            if (boomerangPrefab != null && attackPoint != null)
            {
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);

                canThrowBoomerang = false;
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }

        // 3. Espera a que termine la duraci�n del ataque
        yield return new WaitForSeconds(attackDuration - 0.1f);

        // 4. Volver al estado normal
        isAttacking = false; // Permite el movimiento de nuevo
    }

    /// <summary>
    /// Corutina que espera el tiempo de cooldown del bumer�n.
    /// </summary>
    private IEnumerator BoomerangCooldownCoroutine()
    {
        yield return new WaitForSeconds(boomerangCooldown);
        canThrowBoomerang = true;
        boomerangCooldownCoroutine = null;
    }

    /// <summary>
    /// M�todo p�blico llamado por el bumer�n para reiniciar el cooldown al ser recogido.
    /// </summary>
    public void ResetBoomerangCooldown()
    {
        if (boomerangCooldownCoroutine != null)
        {
            StopCoroutine(boomerangCooldownCoroutine);
            boomerangCooldownCoroutine = null;
        }
        canThrowBoomerang = true;
        Debug.Log("�Bumer�n recogido! Cooldown reiniciado.");
    }

    /// <summary>
    /// Corutina que gestiona la l�gica del Dash.
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