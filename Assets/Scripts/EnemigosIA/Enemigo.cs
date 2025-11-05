using UnityEngine;

public class Enemigo : MonoBehaviour
{
    private Room parentRoom;

    [SerializeField] private float vidaActual = 100f;
    [SerializeField] private float vidaMaxima = 200f;
    [SerializeField] private float tiempoParaDesaparecer = 1f;
    [SerializeField] private EnemyHealthBar EnemyHealthBar;
    private Animator anim;

    // --- CAMBIO 1: Añadimos la bandera para controlar el estado de muerte ---
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
    }

    public void SetParentRoom(Room room)
    {
        parentRoom = room;
    }

    public void TomarDaño(float daño)
    {
        // --- CAMBIO 2: Si el enemigo ya está muerto, no hacemos nada ---
        if (isDead) return;

        vidaActual -= daño;
        Debug.Log($"[Enemigo] {gameObject.name} ha recibido {daño} de daño. Vida restante: {vidaActual}");
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
        // ... (tu código de animación de muerte, etc.)

        if (parentRoom != null)
        {
            parentRoom.EnemyWasDefeated();
        }

        // --- ¡CAMBIO CLAVE AQUÍ! ---
        // Reemplazamos la llamada al ExperienceManager por la llamada al GameManager.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKilled();
        }

        // ... Desactivar IA, Colliders, etc.

        Destroy(gameObject, tiempoParaDesaparecer);
    }
}