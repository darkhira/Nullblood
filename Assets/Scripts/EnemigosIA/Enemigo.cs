using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public PlayerSoundController playerSoundController;

    private Room parentRoom;
    public bool esEnemigoEscudo = false;

    [SerializeField] private float vidaActual = 100f;
    [SerializeField] private float vidaMaxima = 200f;
    [SerializeField] private float tiempoParaDesaparecer = 1f;
    [SerializeField] private EnemyHealthBar EnemyHealthBar;
    private Animator anim;

    // --- CAMBIO 1: A�adimos la bandera para controlar el estado de muerte ---
    private bool isDead = false;

    private void Start()
    {
        // ... (el resto del Start sigue igual)
        EnemyHealthBar.UpdateHealthBar(vidaMaxima, vidaActual);
        anim = GetComponent<Animator>();
        playerSoundController = GetComponent<PlayerSoundController>();
        if (parentRoom == null)
        {
            parentRoom = GetComponentInParent<Room>();
        }
    }

    public void SetParentRoom(Room room)
    {
        parentRoom = room;
    }

    public virtual void TomarDaño(float daño)
    {
        // --- CAMBIO 2: Si el enemigo ya est� muerto, no hacemos nada ---
        if (isDead) return;
        if (GameManager.Instance.escudoActivo && !esEnemigoEscudo)
        {
            Debug.Log($"[Enemigo] {gameObject.name} está protegido por el enemigo de escudo.");
            return;
        }

        vidaActual -= daño;
        if (EnemyHealthBar != null)
{
    EnemyHealthBar.UpdateHealthBar(vidaMaxima, vidaActual);
}
        if (vidaActual <= 0)
        {
            // --- CAMBIO 3: Marcamos al enemigo como muerto ANTES de llamar a Muerte() ---
            isDead = true;
            if (playerSoundController != null)
    {
        playerSoundController.playsonidoMuerteVorr();
        playerSoundController.playsonidoMuerteMono();
    }
            Muerte();
        }
    }

    private void Muerte()
    {
        // ... (tu c�digo de animaci�n de muerte, etc.)

        if (parentRoom != null)
        {
            parentRoom.EnemyWasDefeated();
        }

        // --- �CAMBIO CLAVE AQU�! ---
        // Reemplazamos la llamada al ExperienceManager por la llamada al GameManager.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKilled();
        }

        // ... Desactivar IA, Colliders, etc.

        Destroy(gameObject, tiempoParaDesaparecer);
    }
}