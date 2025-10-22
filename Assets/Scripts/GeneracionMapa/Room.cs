// Script: Room.cs
using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Puertas")]
    public GameObject TopDoor;
    public GameObject BottomDoor;
    public GameObject LeftDoor;
    public GameObject RightDoor;

    [HideInInspector] public Vector2Int RoomIndex;

    private GenerarEnemigos generadorEnemigos;
    private bool playerHasEntered = false;

    private int activeEnemiesCount = 0;

    // Lista para almacenar SOLO las referencias de las puertas conectadas.
    private List<GameObject> connectedDoors = new List<GameObject>();

    private void Start()
    {
        generadorEnemigos = GetComponent<GenerarEnemigos>();

        // ⭐ CAMBIO: Quitamos CloseAllDoors() de Start(). 
        // Las puertas DEBEN estar cerradas por defecto en el prefab, 
        // y el RoomManager abrirá (SetActive(true)) solo las conectadas.
    }

    //
    // --- LÓGICA DE COMBATE Y ENEMIGOS ---
    //

    public bool HasActiveEnemies()
    {
        return activeEnemiesCount > 0;
    }

    public void EnemyWasSpawned()
    {
        activeEnemiesCount++;
        Debug.Log($"[Room {name}] Enemigo Generado. Total: {activeEnemiesCount}");
    }

    public void EnemyWasDefeated()
    {
        activeEnemiesCount--;
        Debug.Log($"[Room {name}] Enemigo Derrotado. Restantes: {activeEnemiesCount}");

        if (activeEnemiesCount <= 0)
        {
            Debug.Log($"[Room {name}] ¡Habitación despejada! Abriendo puertas CONECTADAS.");
            // Llama a la nueva función que solo abre las puertas válidas.
            OpenConnectedDoors();
        }
    }

    //
    // --- LÓGICA DE PUERTAS ---
    //

    // ⭐ CAMBIO CLAVE: Esta función ahora hace 2 cosas:
    // 1. Abre visualmente la puerta para permitir el paso inicial (si está conectada).
    // 2. Registra la puerta en la lista 'connectedDoors' para el bloqueo/desbloqueo de combate.
    public void OpenDoor(Vector2Int direction)
    {
        GameObject doorObject = null;

        if (direction == Vector2Int.up && TopDoor != null)
            doorObject = TopDoor;
        else if (direction == Vector2Int.down && BottomDoor != null)
            doorObject = BottomDoor;
        else if (direction == Vector2Int.left && LeftDoor != null)
            doorObject = LeftDoor;
        else if (direction == Vector2Int.right && RightDoor != null)
            doorObject = RightDoor;

        if (doorObject != null)
        {
            // 1. Apertura Visual Inmediata (para la navegación inicial)
            doorObject.SetActive(true); // ⭐ RESTAURADO: Abre la puerta al ser conectada.

            // 2. Registro para bloqueo de combate
            if (!connectedDoors.Contains(doorObject))
            {
                connectedDoors.Add(doorObject);
            }
        }
    }

    // ⭐ NUEVO MÉTODO: Abre solo las puertas que fueron registradas.
    public void OpenConnectedDoors()
    {
        foreach (GameObject door in connectedDoors)
        {
            if (door != null)
            {
                door.SetActive(true);
            }
        }
    }

    // ⭐ CAMBIO: Modificado para cerrar SÓLO las puertas que están conectadas (para el bloqueo de combate)
    public void CloseConnectedDoors()
    {
        foreach (GameObject door in connectedDoors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }

    // El método CloseAllDoors() anterior ya no se usa, pero lo puedes dejar o renombrar.
    public void CloseAllDoors()
    {
        // Esta función ahora solo cierra las conectadas para el bloqueo de combate.
        CloseConnectedDoors();
    }


    public DoorTeleport GetDoorTeleport(Vector2Int direction)
    {
        if (direction == Vector2Int.up && TopDoor != null)
            return TopDoor.GetComponentInChildren<DoorTeleport>();
        if (direction == Vector2Int.down && BottomDoor != null)
            return BottomDoor.GetComponentInChildren<DoorTeleport>();
        if (direction == Vector2Int.left && LeftDoor != null)
            return LeftDoor.GetComponentInChildren<DoorTeleport>();
        if (direction == Vector2Int.right && RightDoor != null)
            return RightDoor.GetComponentInChildren<DoorTeleport>();

        return null;
    }

    //
    // --- LÓGICA DE TELETRANSPORTE Y ENTRADA ---
    //

    public void TeleportPlayerHere(Vector2Int fromDirection)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 offset = Vector3.zero;
        float distance = 2f;

        if (fromDirection == Vector2Int.up) offset = Vector3.down * distance;
        else if (fromDirection == Vector2Int.down) offset = Vector3.up * distance;
        else if (fromDirection == Vector2Int.left) offset = Vector3.right * distance;
        else if (fromDirection == Vector2Int.right) offset = Vector3.left * distance;

        player.transform.position = transform.position + offset;

        OnPlayerEnter();
    }

    public void OnPlayerEnter()
    {
        if (playerHasEntered) return;

        Debug.Log($"[Room] Jugador entró a {gameObject.name}");

        playerHasEntered = true;

        if (generadorEnemigos != null)
        {
            generadorEnemigos.Generar();

            // ⭐ IMPORTANTE: Si se generan enemigos, debes bloquear las puertas para iniciar el combate.
            if (activeEnemiesCount > 0)
            {
                CloseConnectedDoors();
            }
        }
        else
        {
            Debug.LogWarning($"[Room] No hay GenerarEnemigos en {gameObject.name}");
        }
    }
}