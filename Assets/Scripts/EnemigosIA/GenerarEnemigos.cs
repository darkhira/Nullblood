using UnityEngine;
using System.Collections.Generic; // Necesario para Listas
using System.Linq; // Necesario para OrderBy

// --- �NUEVA CLASE DE DATOS! ---
// Esta clase agrupar� un prefab de enemigo con una cantidad m�nima y m�xima.
// [System.Serializable] permite que la veamos en el Inspector de Unity.
[System.Serializable]
public class EnemySpawnGroup
{
    public GameObject enemigoPrefab;
    [Tooltip("M�nimo de este enemigo a generar")]
    public int minCantidad = 1;
    [Tooltip("M�ximo de este enemigo a generar")]
    public int maxCantidad = 1;
}
// ---------------------------------


public class GenerarEnemigos : MonoBehaviour
{
    // --- CAMBIO: Reemplazamos la lista simple por la nueva lista de grupos ---
    [Header("Grupos de Enemigos")]
    [Tooltip("Define los tipos y cantidades de enemigos para esta sala.")]
    public List<EnemySpawnGroup> gruposDeEnemigos;

    [Header("Puntos de Spawn")]
    [Tooltip("Arrastra aqu� los GameObjects que marcan d�nde pueden aparecer enemigos.")]
    public List<Transform> puntosDeSpawn;

    private bool enemigosGenerados = false;
    private Room parentRoom;

    private void Start()
    {
        parentRoom = GetComponentInParent<Room>();
        if (parentRoom == null)
        {
            Debug.LogError("[GenerarEnemigos] �No se encontr� el script 'Room' en el padre!");
        }
    }

    public void Generar()
    {
        if (enemigosGenerados) return;
        enemigosGenerados = true;

        // --- �NUEVA L�GICA DE GENERACI�N! ---

        // 1. Crear una "lista de pedidos" de todos los prefabs a generar
        List<GameObject> listaDeSpawn = new List<GameObject>();
        foreach (EnemySpawnGroup grupo in gruposDeEnemigos)
        {
            // Decide cu�ntos generar de ESTE tipo (ej. entre 1 y 3 Vorrs)
            int cantidadEsteGrupo = Random.Range(grupo.minCantidad, grupo.maxCantidad + 1);

            for (int i = 0; i < cantidadEsteGrupo; i++)
            {
                if (grupo.enemigoPrefab != null)
                {
                    listaDeSpawn.Add(grupo.enemigoPrefab);
                }
            }
        }

        // 2. Comprobaciones de Puntos de Spawn
        int totalEnemigos = listaDeSpawn.Count;
        if (totalEnemigos == 0) return; // No hay nada que generar (sala vac�a)

        if (puntosDeSpawn == null || puntosDeSpawn.Count == 0)
        {
            Debug.LogError($"[GenerarEnemigos] {parentRoom.name} intenta generar {totalEnemigos} enemigos pero no tiene 'puntosDeSpawn' asignados.");
            return;
        }

        if (totalEnemigos > puntosDeSpawn.Count)
        {
            Debug.LogWarning($"[GenerarEnemigos] {parentRoom.name} quiere generar {totalEnemigos} enemigos pero solo tiene {puntosDeSpawn.Count} puntos de spawn. Se generar�n menos enemigos.");
            totalEnemigos = puntosDeSpawn.Count; // Limita al n�mero de puntos de spawn
        }

        Debug.Log($"[GenerarEnemigos] Generando {totalEnemigos} enemigos en {parentRoom.name}");

        // 3. Notificar al RoomManager
        if (RoomManager.Instance != null && !parentRoom.isBossRoom)
        {
            RoomManager.Instance.RegisterCombatRoom();
        }

        // 4. Mezclar los puntos de spawn
        var puntosAleatorios = puntosDeSpawn.OrderBy(x => Random.value).ToList();

        // 5. Generar los enemigos
        for (int i = 0; i < totalEnemigos; i++)
        {
            GameObject prefabDelEnemigo = listaDeSpawn[i];
            Vector3 spawnPos = puntosAleatorios[i].position;

            GameObject newEnemy = Instantiate(prefabDelEnemigo, spawnPos, Quaternion.identity, transform);

            if (parentRoom != null)
            {
                parentRoom.EnemyWasSpawned();
                if (newEnemy.TryGetComponent<Enemigo>(out Enemigo enemigoScript))
                {
                    enemigoScript.SetParentRoom(parentRoom);
                }
            }
        }

        if (parentRoom != null)
        {
            parentRoom.CloseAllDoors();
        }
    }
}