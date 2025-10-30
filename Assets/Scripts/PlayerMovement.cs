using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
    // --- NUEVA LÍNEA: Velocidad de correr ---
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
    [SerializeField] private Transform attackPoint;

    [Header("Configuración de Bumerán")]
    [SerializeField] private float boomerangCooldown = 5f;
    private bool canThrowBoomerang = true;
    private Coroutine boomerangCooldownCoroutine;

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    // --- NUEVA LÍNEA: Estado de correr ---
    private bool isRunning = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1);

    private CombateCaC combateCaC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combateCaC = GetComponent<CombateCaC>();
    }

    void Update()
    {
        if (isDashing || isAttacking) return;

        HandleMovementInput();

        // Input de Ataque Cuerpo a Cuerpo (Clic izquierdo)
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackCoroutine(true));
            return;
        }

        // Input de Lanzar Bumerán (Tecla R)
        if (Input.GetKeyDown(KeyCode.R) && canThrowBoomerang)
        {
            StartCoroutine(AttackCoroutine(false));
            return;
        }

        // --- LÍNEA CORREGIDA ---
        // Input de Dash (Cualquier Tecla Control)
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }
        // -------------------------

        // Lógica de Correr (Tecla Shift Izquierdo)
        isRunning = Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("isRunning", isRunning);
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            lastMoveDirection = moveInput;
        }

        if (moveInput.sqrMagnitude > 0.1f)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
        else
        {
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }

        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        if (isDashing || isAttacking) return;

        // --- MOVIMIENTO MODIFICADO: Usa la velocidad adecuada ---
        float currentSpeed = isRunning ? runSpeed : speed; // Elige la velocidad según si corre
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
        // -------------------------------------------------------
    }

    // ... (AttackCoroutine, BoomerangCooldownCoroutine, ResetBoomerangCooldown, DashCoroutine siguen igual) ...
    // --- NO CHANGES NEEDED IN THE COROUTINES BELOW THIS LINE ---

    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true;
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.1f);
        if (isMelee)
        {
            if (combateCaC != null) combateCaC.EjecutarGolpe();
        }
        else
        {
            if (boomerangPrefab != null && attackPoint != null && canThrowBoomerang)
            {
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);
                canThrowBoomerang = false;
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }
        yield return new WaitForSeconds(attackDuration - 0.1f);
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private IEnumerator BoomerangCooldownCoroutine()
    {
        yield return new WaitForSeconds(boomerangCooldown);
        canThrowBoomerang = true;
        boomerangCooldownCoroutine = null;
    }

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