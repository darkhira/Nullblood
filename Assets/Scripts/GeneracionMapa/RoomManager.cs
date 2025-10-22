using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Configuración de generación")]
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;

    [Header("Tamaño de sala")]
    [SerializeField] private int roomWidth = 20;
    [SerializeField] private int roomHeight = 12;
    [SerializeField] private int gridSizeX = 10;
    [SerializeField] private int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY)); // izquierda
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY)); // derecha
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1)); // arriba
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1)); // abajo
        }
        else if (roomCount < minRooms && !generationComplete)
        {
            Debug.Log("No se alcanzó el número mínimo de salas, regenerando...");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generación completada, {roomCount} salas creadas.");
            generationComplete = true;
            ConnectAllDoors(); // conecta automáticamente los DoorTeleport
        }
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;

        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;
        if (roomCount >= maxRooms)
            return false;
        if (roomGrid[x, y] != 0)
            return false;
        if (Random.value < 0.5f)
            return false;
        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void ConnectAllDoors()
    {
        foreach (var roomObject in roomObjects)
        {
            Room roomScript = roomObject.GetComponent<Room>();
            Vector2Int index = roomScript.RoomIndex;
            int x = index.x;
            int y = index.y;

            TryConnect(roomScript, new Vector2Int(x - 1, y), Vector2Int.left);
            TryConnect(roomScript, new Vector2Int(x + 1, y), Vector2Int.right);
            TryConnect(roomScript, new Vector2Int(x, y - 1), Vector2Int.down);
            TryConnect(roomScript, new Vector2Int(x, y + 1), Vector2Int.up);
        }

        Debug.Log("Puertas conectadas y sincronizadas correctamente.");
    }

    private void TryConnect(Room currentRoom, Vector2Int neighborIndex, Vector2Int direction)
    {
        if (!IsInsideGrid(neighborIndex)) return;
        if (roomGrid[neighborIndex.x, neighborIndex.y] == 0) return;

        Room neighborRoom = GetRoomScriptAt(neighborIndex);
        if (neighborRoom == null) return;

        currentRoom.OpenDoor(direction);
        neighborRoom.OpenDoor(-direction);

        DoorTeleport doorA = currentRoom.GetDoorTeleport(direction);
        DoorTeleport doorB = neighborRoom.GetDoorTeleport(-direction);

        if (doorA != null && doorB != null)
        {
            doorA.targetDoor = doorB;
            doorB.targetDoor = doorA;

            doorA.parentRoom = currentRoom;
            doorB.parentRoom = neighborRoom;
        }
    }

    private bool IsInsideGrid(Vector2Int index)
    {
        return index.x >= 0 && index.x < gridSizeX && index.y >= 0 && index.y < gridSizeY;
    }

    private Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        return roomObject != null ? roomObject.GetComponent<Room>() : null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2),
                           roomHeight * (gridY - gridSizeY / 2),
                           0f);
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }
}
