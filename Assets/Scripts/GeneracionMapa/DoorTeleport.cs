using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class DoorTeleport : MonoBehaviour
{
    [HideInInspector] public Room parentRoom;
    [HideInInspector] public DoorTeleport targetDoor;
    public float blockTime = 0.25f;

    private bool isTeleporting = false;
    private CameraFollow camFollow;

    private void Start()
    {
#if UNITY_2023_1_OR_NEWER
        camFollow = FindFirstObjectByType<CameraFollow>();
#else
        camFollow = FindObjectOfType<CameraFollow>();
#endif

        if (parentRoom == null)
        {
            parentRoom = GetComponentInParent<Room>();

            if (parentRoom == null)
            {
#if UNITY_2023_1_OR_NEWER
                Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
#else
                Room[] allRooms = FindObjectsOfType<Room>();
#endif
                float minDist = Mathf.Infinity;
                foreach (Room r in allRooms)
                {
                    float d = Vector2.Distance(transform.position, r.transform.position);
                    if (d < minDist)
                    {
                        parentRoom = r;
                        minDist = d;
                    }
                }
            }

            if (parentRoom != null)
                Debug.Log($"[DoorTeleport] {name} asignó Room = {parentRoom.name}");
            else
                Debug.LogWarning($"[DoorTeleport] {name} no encontró ningún Room cercano.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isTeleporting || targetDoor == null) return;

        // AÑADIDO: Comprobación de enemigos en la habitación actual.
        // **ASUMIMOS** que la clase 'Room' tiene un método o propiedad
        // llamado 'HasActiveEnemies()' que devuelve true si hay enemigos vivos.
        if (parentRoom != null && parentRoom.HasActiveEnemies())
        {
            Debug.Log($"[DoorTeleport] {name}: ¡Hay enemigos activos en {parentRoom.name}! Teletransporte bloqueado.");
            // Opcional: Podrías añadir lógica aquí para notificar al jugador (e.g., sonido, mensaje UI).
            return;
        }
        // FIN AÑADIDO

        StartCoroutine(TeleportRoutine(other));
    }

    private IEnumerator TeleportRoutine(Collider2D player)
    {
        isTeleporting = true;
        targetDoor.isTeleporting = true;

        yield return null;

        Vector2Int dir = Vector2Int.zero;
        // La lógica para determinar la dirección de la teletransportación puede ser simplificada,
        // pero mantendremos la tuya por ahora:
        if (targetDoor.transform.position.x < transform.position.x) dir = Vector2Int.left;
        else if (targetDoor.transform.position.x > transform.position.x) dir = Vector2Int.right;
        else if (targetDoor.transform.position.y > transform.position.y) dir = Vector2Int.up;
        else if (targetDoor.transform.position.y < transform.position.y) dir = Vector2Int.down;

        targetDoor.parentRoom.TeleportPlayerHere(dir);

        if (camFollow != null)
            camFollow.SnapToTarget();

        yield return new WaitForSeconds(blockTime);

        isTeleporting = false;
        targetDoor.isTeleporting = false;
    }
}