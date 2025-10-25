using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
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
    [SerializeField] private float boomerangCooldown = 5f; // Tiempo de espera para volver a lanzar
    private bool canThrowBoomerang = true; // Controla si podemos lanzar
    private Coroutine boomerangCooldownCoroutine; // Referencia a la corutina de cooldown

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1); // Por defecto mirando abajo

    // Referencia al script de combate cuerpo a cuerpo
    private CombateCaC combateCaC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combateCaC = GetComponent<CombateCaC>(); // Obtiene la referencia al script de combate
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

        // Input de Ataque Cuerpo a Cuerpo (Clic izquierdo)
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackCoroutine(true)); // true indica que es ataque melee
            return; // Sale del Update para evitar procesar otros inputs en el mismo frame
        }

        // Input de Lanzar Bumer�n (Tecla R) y comprobaci�n del cooldown
        if (Input.GetKeyDown(KeyCode.R) && canThrowBoomerang)
        {
            StartCoroutine(AttackCoroutine(false)); // false indica que es lanzar bumer�n
            return; // Sale del Update
        }

        // Input de Dash (Shift Izquierdo)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    /// <summary>
    /// Lee el input de movimiento y actualiza los par�metros del Animator para el Blend Tree.
    /// </summary>
    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // Guarda la �ltima direcci�n en la que se movi� el jugador
        if (moveInput.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

        // Actualiza los par�metros del Animator para el Blend Tree de Idle/Walk
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

        // Actualiza el par�metro Speed para la transici�n Idle <-> Walk
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    /// <summary>
    /// Aplica el movimiento al Rigidbody en el ciclo de f�sicas.
    /// </summary>
    private void FixedUpdate()
    {
        // No mueve al jugador si est� haciendo dash o atacando
        if (isDashing || isAttacking)
        {
            return;
        }

        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Corutina que gestiona la l�gica de ataque (animaci�n y ejecuci�n).
    /// </summary>
    /// <param name="isMelee">True si es ataque cuerpo a cuerpo, False si es lanzar bumer�n.</param>
    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true;

        // Orienta la animaci�n de ataque seg�n la �ltima direcci�n
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetBool("isAttacking", true); // Activa el estado de ataque en el Animator

        // Espera un breve momento para sincronizar con la animaci�n (opcional)
        yield return new WaitForSeconds(0.1f);

        // Ejecuta la l�gica de da�o correspondiente
        if (isMelee)
        {
            if (combateCaC != null)
            {
                combateCaC.EjecutarGolpe(); // Llama al script de combate CaC
            }
        }
        else // Lanzar bumer�n
        {
            if (boomerangPrefab != null && attackPoint != null && canThrowBoomerang) // Comprobaci�n a�adida
            {
                // Instancia el bumer�n en el punto de ataque
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                // Llama al m�todo Throw del bumer�n para lanzarlo
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);

                // Inicia el cooldown del bumer�n
                canThrowBoomerang = false;
                // Guarda la referencia a la corutina para poder cancelarla si se recoge
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }

        // Espera el resto de la duraci�n de la animaci�n
        yield return new WaitForSeconds(attackDuration - 0.1f);

        // Desactiva el estado de ataque
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    /// <summary>
    /// Corutina que espera el tiempo de cooldown del bumer�n.
    /// </summary>
    private IEnumerator BoomerangCooldownCoroutine()
    {
        yield return new WaitForSeconds(boomerangCooldown);
        canThrowBoomerang = true;
        boomerangCooldownCoroutine = null; // Limpia la referencia
    }

    /// <summary>
    /// M�todo p�blico llamado por el bumer�n para reiniciar el cooldown al ser recogido.
    /// </summary>
    public void ResetBoomerangCooldown()
    {
        if (boomerangCooldownCoroutine != null)
        {
            StopCoroutine(boomerangCooldownCoroutine); // Detiene la corutina de espera
            boomerangCooldownCoroutine = null;
        }
        canThrowBoomerang = true; // Permite lanzar de nuevo
        Debug.Log("�Bumer�n recogido! Cooldown reiniciado.");
    }

    /// <summary>
    /// Corutina que gestiona la l�gica del Dash.
    /// </summary>
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        Vector2 dashDirection = moveInput; // Usa la direcci�n actual del input
        float startTime = Time.time;

        // Bucle que dura lo que dure el dash
        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate(); // Espera al siguiente frame de f�sicas
        }

        isDashing = false;

        // Espera el cooldown antes de poder volver a hacer dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}