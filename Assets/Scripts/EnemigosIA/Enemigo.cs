using UnityEngine;

public class Enemigo : MonoBehaviour
{
    private Room parentRoom;
    private float vidaOriginal= 0;
    private bool tieneEscudo = false;

    [SerializeField] private float vidaActual = 100f;
    [SerializeField] private float vidaMaxima = 200f;
    [SerializeField] private float tiempoParaDesaparecer = 1f;
    [SerializeField] private EnemyHealthBar EnemyHealthBar;
    

    [Header("Turret Settings")]
    [SerializeField] private string turretTag = "EnemyTurret"; // 👈 tag to search for
    private RangedTurret torreta;
    private Animator anim;

    // --- CAMBIO 1: A�adimos la bandera para controlar el estado de muerte ---
    private bool isDead = false;

    private void Start()
    {
        vidaOriginal = vidaActual;
        // ... (el resto del Start sigue igual)
        EnemyHealthBar.UpdateHealthBar(vidaMaxima, vidaActual);
        anim = GetComponent<Animator>();
        if (parentRoom == null)
        {
            parentRoom = GetComponentInParent<Room>();
        }
        GameObject turretObj = GameObject.FindGameObjectWithTag(turretTag);
        if (turretObj != null)
        {
            torreta = turretObj.GetComponent<RangedTurret>();
        }
        else
        {
            Debug.LogWarning($"[RangedEnemy] No RangedTurret found with tag '{turretTag}'");
        }
    }
    public void ApplyShield(float amount, bool shield)
    {
        if (shield)
        {
            tieneEscudo = true;
            // Aquí podrías activar un sprite o animación de escudo
        }
        else
        {
            vidaActual += amount;
        }
    }
    public void RemoveShield(float amount, bool shield)
    {
        if (shield)
        {
            tieneEscudo = false;
            // Desactivar visual del escudo si existía
        }
        else
        {
            // Evitar que baje por debajo del original
            vidaActual = Mathf.Max(vidaOriginal, vidaActual - amount);
        }
    }


    public void SetParentRoom(Room room)
    {
        parentRoom = room;
    }

    public void TomarDaño(float daño)
    {
        // --- CAMBIO 2: Si el enemigo ya est� muerto, no hacemos nada ---
        if (isDead) return;
        if (tieneEscudo)
        {
            // Escudo absorbe el daño
            daño *= 0.5f; // por ejemplo, reduce a la mitad
        }

        vidaActual -= daño;
        Debug.Log($"[Enemigo] {gameObject.name} ha recibido {daño} de da�o. Vida restante: {vidaActual}");
        EnemyHealthBar.UpdateHealthBar(vidaMaxima, vidaActual);

        if (vidaActual <= 0)
        {
            // --- CAMBIO 3: Marcamos al enemigo como muerto ANTES de llamar a Muerte() ---
            isDead = true;
            Muerte();
        }
    }

    private void Muerte()
    {
        // Si este enemigo es un SupportEnemy, delegamos la muerte a su lógica especial
        SupportEnemy support = GetComponent<SupportEnemy>();
        if (support != null)
        {
            support.OnDeath();
            parentRoom.EnemyWasDefeated();
            return; // No ejecutar la muerte estándar aquí
        }
        RangedEnemy movement = GetComponent<RangedEnemy>();
        if (movement != null)
            movement.enabled = false;
        RangedTurret turret = GetComponent<RangedTurret>();
        if (turret != null)
            torreta.SetPuedeDisparar(false);
        // ... (el resto de la funci�n Muerte sigue igual)
        if (anim != null)
        {
            anim.SetTrigger("Muerte");
        }
        if (parentRoom != null)
        {
            parentRoom.EnemyWasDefeated();
        }

        // ... Desactivar IA, Colliders, etc.
        if (transform.parent != null && transform.parent.gameObject.scene.IsValid() && gameObject.tag != "Player")
        {
            Destroy(transform.parent.gameObject, tiempoParaDesaparecer);
        }
        Destroy(gameObject, tiempoParaDesaparecer);
        
    }
}