using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Puertas")]
    public GameObject TopDoor;
    public GameObject BottomDoor;
    public GameObject LeftDoor;
    public GameObject RightDoor;

    [Header("Tipo de Sala")]
    [Tooltip("Marca esto si es la Sala del Jefe. Sus puertas no se abrirán hasta que se desbloquee.")]
    public bool isBossRoom = false;

    [HideInInspector] public Vector2Int RoomIndex;

    private GenerarEnemigos generadorEnemigos;
    private bool playerHasEntered = false;

    private int activeEnemiesCount = 0;
    private List<GameObject> connectedDoors = new List<GameObject>();

    private void Start()
    {
        generadorEnemigos = GetComponent<GenerarEnemigos>();
    }

    public bool HasActiveEnemies()
    {
        return activeEnemiesCount > 0;
    }

    public void EnemyWasSpawned()
    {
        activeEnemiesCount++;
    }

    public void EnemyWasDefeated()
    {
        activeEnemiesCount--;

        if (activeEnemiesCount <= 0)
        {
            if (!isBossRoom)
            {
                if (RoomManager.Instance != null)
                {
                    RoomManager.Instance.OnRoomCleared();
                }
            }
            OpenConnectedDoors();
        }
    }

    public void UnlockRoom()
    {
        if (isBossRoom)
        {
            Debug.LogWarning($"¡SALA FINAL {name} DESBLOQUEADA! Abriendo puertas...");
            OpenConnectedDoors();
        }
    }

    public void OpenDoor(Vector2Int direction)
    {
        GameObject doorObject = GetDoorObject(direction);
        if (doorObject != null)
        {
            if (!isBossRoom)
            {
                doorObject.SetActive(true);
            }
            if (!connectedDoors.Contains(doorObject))
            {
                connectedDoors.Add(doorObject);
            }
        }
    }

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

    // --- ¡ESTE ES EL MÉTODO QUE FALTABA! ---
    public void CloseAllDoors()
    {
        CloseConnectedDoors();
    }
    // ----------------------------------------

    private void CloseConnectedDoors()
    {
        foreach (GameObject door in connectedDoors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }

    public DoorTeleport GetDoorTeleport(Vector2Int direction)
    {
        GameObject doorObject = GetDoorObject(direction);
        if (doorObject != null)
            return doorObject.GetComponentInChildren<DoorTeleport>();
        return null;
    }

    private GameObject GetDoorObject(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return TopDoor;
        if (direction == Vector2Int.down) return BottomDoor;
        if (direction == Vector2Int.left) return LeftDoor;
        if (direction == Vector2Int.right) return RightDoor;
        return null;
    }

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
        playerHasEntered = true;

        if (generadorEnemigos != null)
        {
            generadorEnemigos.Generar();
            if (activeEnemiesCount > 0)
            {
                CloseAllDoors(); // <- Ahora esta llamada funcionará
            }
        }
    }
}