using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // ... (Tus variables anteriores siguen igual) ...
    public PlayerSoundController playerSoundController;

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
    [SerializeField] private Transform attackPoint;

    [Header("Configuración de Bumerán")]
    [SerializeField] private float boomerangCooldown = 5f;
    private bool canThrowBoomerang = true;
    private Coroutine boomerangCooldownCoroutine;

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private bool isRunning = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1);

    private CombateCaC combateCaC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        combateCaC = GetComponent<CombateCaC>();
        playerSoundController = GetComponent<PlayerSoundController>();
    }

    // ... (Tu Update, HandleMovementInput y FixedUpdate siguen igual) ...

    void Update()
    {
        if (isDashing || isAttacking) return;
        HandleMovementInput();

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            playerSoundController.playsonidoGolpe();
            StartCoroutine(AttackCoroutine(true));
            return;
        }

        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton3)) && canThrowBoomerang)
        {
            playerSoundController.playsonidoBoomerang();
            StartCoroutine(AttackCoroutine(false));
            return;
        }

        // DASH
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            playerSoundController.playsonidoDash();
            StartCoroutine(DashCoroutine());
        }

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4);
        animator.SetBool("isRunning", isRunning);
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveInput.sqrMagnitude > 0.1f) lastMoveDirection = moveInput;

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
        float currentSpeed = isRunning ? runSpeed : speed;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }

    // ... (AttackCoroutine, Boomerang logic siguen igual) ...
    private IEnumerator AttackCoroutine(bool isMelee)
    {
        isAttacking = true;
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        yield return null;

        if (isMelee)
        {
            animator.SetTrigger("MeleeAttack");
            yield return new WaitForSeconds(0.1f);
            if (combateCaC != null) combateCaC.EjecutarGolpe(lastMoveDirection);
        }
        else
        {
            animator.SetTrigger("BoomerangThrow");
            yield return new WaitForSeconds(0.1f);
            if (boomerangPrefab != null && attackPoint != null)
            {
                GameObject boomerangObj = Instantiate(boomerangPrefab, attackPoint.position, Quaternion.identity);
                boomerangObj.GetComponent<Boomerang>().Throw(transform, lastMoveDirection);
                canThrowBoomerang = false;
                boomerangCooldownCoroutine = StartCoroutine(BoomerangCooldownCoroutine());
            }
        }
        yield return new WaitForSeconds(attackDuration - 0.1f);
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
        Debug.Log("Bumerán recogido! Cooldown reiniciado.");
    }

    // --- CORRUTINA MODIFICADA PARA DASH ---
    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        // 1. Activar Invulnerabilidad usando el Singleton de PlayerStats
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.SetInvulnerable(true);
        }

        Vector2 dashDirection = moveInput;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;

        // 2. Desactivar Invulnerabilidad al terminar el movimiento
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.SetInvulnerable(false);
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}