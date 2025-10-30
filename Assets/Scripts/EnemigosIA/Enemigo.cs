using UnityEngine;

public class Enemigo : MonoBehaviour
{
    private Room parentRoom;

    [SerializeField] private float vidaActual = 100f;
    [SerializeField] private float vidaMaxima = 200f;
    [SerializeField] private float tiempoParaDesaparecer = 1f;
    [SerializeField] private EnemyHealthBar EnemyHealthBar;

    [Header("Turret Settings")]
    private string turretTag = "EnemyTurret"; // 👈 tag to search for
    [SerializeField] private RangedTurret torreta;
    private Animator anim;

    // --- CAMBIO 1: A�adimos la bandera para controlar el estado de muerte ---
    private bool isDead = false;

    private void Start()
    {
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

    public void SetParentRoom(Room room)
    {
        parentRoom = room;
    }

    public void TomarDaño(float daño)
    {
        // --- CAMBIO 2: Si el enemigo ya est� muerto, no hacemos nada ---
        if (isDead) return;

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