using UnityEngine;
using System.Collections.Generic; // Necesario para Listas
using System.Linq; // Necesario para "OrderBy"

public class GenerarEnemigos : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject enemigoPrefab;
    public int minEnemigos = 1;
    public int maxEnemigos = 3;

    // --- CAMBIO CLAVE: Referencia a los puntos de spawn ---
    public List<Transform> puntosDeSpawn;

    private bool enemigosGenerados = false;
    private Room parentRoom;

    private void Start()
    {
        parentRoom = GetComponentInParent<Room>();
    }

    public void Generar()
    {
        if (enemigosGenerados || puntosDeSpawn.Count == 0) return;

        int cantidad = Random.Range(minEnemigos, maxEnemigos + 1);
        // Nos aseguramos de no generar más enemigos que puntos disponibles
        cantidad = Mathf.Min(cantidad, puntosDeSpawn.Count);

        // --- CAMBIO CLAVE: Mezclamos la lista de puntos para que el spawn sea aleatorio ---
        var puntosAleatorios = puntosDeSpawn.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < cantidad; i++)
        {
            // Obtenemos una posición de la lista aleatoria
            Vector3 spawnPos = puntosAleatorios[i].position;

            GameObject newEnemy = Instantiate(enemigoPrefab, spawnPos, Quaternion.identity, transform);

            // Asignar la Room al enemigo, como ya hacías
            if (parentRoom != null)
            {
                parentRoom.EnemyWasSpawned();
                if (newEnemy.TryGetComponent<Enemigo>(out Enemigo enemigoScript))
                {
                    enemigoScript.SetParentRoom(parentRoom);
                }
            }
        }

        if (cantidad > 0 && parentRoom != null)
        {
            parentRoom.CloseAllDoors();
        }

        enemigosGenerados = true;
    }
}