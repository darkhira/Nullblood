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

    // Variables de estado
    private bool isDashing = false;
    private bool canDash = true;

    // Estado de ataque
    private bool isAttacking = false;

    // **NUEVA VARIABLE: Guarda la �ltima direcci�n de movimiento v�lida**
    private Vector2 lastMoveDirection = new Vector2(0, -1); // Por defecto: mirando hacia abajo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing || isAttacking)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // **CAMBIO 1: Actualizar la �ltima direcci�n solo cuando hay movimiento**
        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

        // --- INPUT DE ATAQUE ---
        if (Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.Joystick1Button1) )
        {
            StartCoroutine(Golpe());
            return;
        }

        // Actualizar animaciones de movimiento (usa moveX, moveY)
        animator.SetFloat("Horizontal", moveX);
        animator.SetFloat("Vertical", moveY);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Detectar input para Dash (ejemplo: tecla Shift izquierda)
        if ((Input.GetKeyDown(KeyCode.LeftShift) ||Input.GetKeyDown(KeyCode.Joystick1Button1)) && canDash)
        {
            if (moveInput != Vector2.zero)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isAttacking)
            return;

        // Movimiento base
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    private IEnumerator Golpe()
    {
        isAttacking = true;

        // **CAMBIO 2: Forzar los par�metros del Blend Tree con la �ltima direcci�n**
        // Esto asegura que la animaci�n de ataque se dirija correctamente antes de activarse.
        animator.SetFloat("Horizontal", lastMoveDirection.x);
        animator.SetFloat("Vertical", lastMoveDirection.y);

        animator.SetTrigger("Golpe");

        // Esperar el tiempo de la animaci�n de ataque.
        yield return new WaitForSeconds(0.4f);

        // Restablecer el estado de ataque
        isAttacking = false;
    }

    private IEnumerator Dash()
    {
        // ... (Tu c�digo de Dash)
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