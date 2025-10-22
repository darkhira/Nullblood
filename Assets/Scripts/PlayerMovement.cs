using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento Base")]
    [SerializeField] private float speed = 3f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Configuración de Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDuration = 0.4f;

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private Vector2 lastMoveDirection = new Vector2(0, -1);

    // --- NUEVA VARIABLE: Referencia al script de combate ---
    private CombateCaC combateCaC;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // --- OBTENEMOS LA REFERENCIA AL SCRIPT DE COMBATE ---
        combateCaC = GetComponent<CombateCaC>();
    }

    void Update()
    {
        if (isDashing || isAttacking)
        {
            return;
        }

        HandleMovementInput();

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(AttackCoroutine());
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(DashCoroutine());
        }
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
        if (isDashing || isAttacking)
        {
            return;
        }

        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);
        animator.SetBool("isAttacking", true);

        // --- LLAMADA A LA LÓGICA DE DAÑO ---
        // Esperamos un pequeño instante para que la animación comience...
        yield return new WaitForSeconds(0.1f);
        // ...y luego le ordenamos a CombateCaC que aplique el daño.
        if (combateCaC != null)
        {
            combateCaC.EjecutarGolpe();
        }

        // Esperamos el resto de la duración de la animación.
        yield return new WaitForSeconds(attackDuration - 0.1f);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
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