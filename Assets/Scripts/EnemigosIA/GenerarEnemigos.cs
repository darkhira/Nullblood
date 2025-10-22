using UnityEngine;

public class GenerarEnemigos : MonoBehaviour
{
    [Header("Configuración de enemigos")]
    public GameObject enemigoPrefab;
    public int minEnemigos = 1;
    public int maxEnemigos = 3;

    // [QUITAMOS: public float radioSpawn = 4f;]

    private bool enemigosGenerados = false;
    private Room parentRoom;
    private Collider2D roomCollider; // Referencia al Collider2D de la sala

    private void Start()
    {
        // Obtiene la referencia a la Room padre.
        parentRoom = GetComponentInParent<Room>();
        if (parentRoom == null)
        {
            Debug.LogError("[GenerarEnemigos] No se encontró el script 'Room' en el padre o en el mismo objeto. ¡La lógica de combate no funcionará!");
            return;
        }

        // MODIFICACIÓN CLAVE: Obtener el Collider2D de la Room.
        roomCollider = parentRoom.GetComponent<Collider2D>();
        if (roomCollider == null)
        {
            Debug.LogError($"[GenerarEnemigos] El objeto {parentRoom.name} NO tiene un Collider2D. No se puede definir el área de aparición.");
        }
    }

    public void Generar()
    {
        if (enemigosGenerados || roomCollider == null)
        {
            if (enemigosGenerados) Debug.Log($"[GenerarEnemigos] Enemigos ya generados en {gameObject.name}");
            return;
        }

        if (enemigoPrefab == null)
        {
            Debug.LogWarning($"[GenerarEnemigos] No hay prefab asignado en {gameObject.name}");
            return;
        }

        int cantidad = Random.Range(minEnemigos, maxEnemigos + 1);
        Debug.Log($"[GenerarEnemigos] Generando {cantidad} enemigos en {gameObject.name}");

        // Obtener los límites del Collider2D de la sala
        Bounds bounds = roomCollider.bounds;

        for (int i = 0; i < cantidad; i++)
        {
            // MODIFICACIÓN CLAVE: Generar una posición aleatoria dentro de los límites del Collider
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            // Usamos Vector3 para la instanciación (Z=0 para 2D)
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            GameObject newEnemy = Instantiate(enemigoPrefab, spawnPos, Quaternion.identity, transform);

            if (parentRoom != null)
            {
                // Paso 1: Aumenta el contador en la Room
                parentRoom.EnemyWasSpawned();

                // Paso 2: Asigna la Room al script de salud del enemigo.
                if (newEnemy.TryGetComponent<Enemigo>(out Enemigo enemigoScript))
                {
                    enemigoScript.SetParentRoom(parentRoom);
                }
                else
                {
                    Debug.LogWarning($"El prefab {enemigoPrefab.name} no tiene el script 'Enemigo'. La Room no será notificada de la muerte.");
                }
            }
        }

        // Bloquea las puertas si se generaron enemigos
        if (cantidad > 0 && parentRoom != null)
        {
            parentRoom.CloseAllDoors();
            Debug.Log($"[GenerarEnemigos] {parentRoom.name}: Puertas cerradas para el combate.");
        }

        enemigosGenerados = true;
    }
}