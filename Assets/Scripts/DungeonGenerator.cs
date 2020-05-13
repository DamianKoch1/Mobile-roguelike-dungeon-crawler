using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    private static DungeonGenerator instance;

    public static DungeonGenerator Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<DungeonGenerator>();
            }
            return instance;
        }
    }

    [SerializeField]
    private Tile groundTile;

    public static Tile Ground => Instance.groundTile;

    [SerializeField]
    private Tile wallTile;

    public static Tile Wall => Instance.wallTile;

    [SerializeField]
    private Tile obstacleTile;

    public static Tile Obstacle => Instance.obstacleTile;

    public Tilemap tilemap;

    [SerializeField]
    private Goal goalPrefab;

    [SerializeField, Range(3, 20)]
    private int roomHalfSize;

    [SerializeField, Range(0, 10)]
    private int connectorHalfLength;

    [SerializeField, Range(2, 10)]
    private int connectorHalfWidth;


    [SerializeField, Range(0, 10)]
    private int maxDepth;

    [SerializeField, Range(0, 1)]
    private float roomExitChance = 0.8f;

    [SerializeField, Range(0, 1)]
    private float obstacleChance = 0.1f;

    [SerializeField]
    private bool useRandomSeed;

    public int seed;



    private Dictionary<Vector2Int, DungeonRoom> rooms;
    private List<DungeonRoom> roomsToExpandAt;

    public List<DungeonRoom> roomList;

    private void Start()
    {
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(0, 10000);
        }
        Random.InitState(seed);
        rooms = new Dictionary<Vector2Int, DungeonRoom>();
        roomList = new List<DungeonRoom>();
        roomsToExpandAt = new List<DungeonRoom>();
        tilemap.ClearAllTiles();
        var startRoom = new DungeonRoom(roomHalfSize, roomExitChance, 0, 0, Vector2.zero);
        roomList.Add(startRoom);
        RegisterRoom(startRoom);
        int maxGenerationTries = 500;
        while (maxGenerationTries > 0 && roomsToExpandAt.Count > 0)
        {
            ExpandFromRoom(roomsToExpandAt[0]);
            roomsToExpandAt.RemoveAt(0);
            maxGenerationTries--;
        }

        foreach (var room in rooms.Values)
        {
            room.SetTiles(obstacleChance);
        }
        startRoom.SetTiles();
        SpawnGoal();
    }

    private void RegisterRoom(DungeonRoom room)
    {
        rooms[room.idx] = room;
        roomsToExpandAt.Add(room);
    }

    private void ExpandFromRoom(DungeonRoom room)
    {
        ExpandToDir(room, Vector2Int.up);
        ExpandToDir(room, Vector2Int.down);
        ExpandToDir(room, Vector2Int.left);
        ExpandToDir(room, Vector2Int.right);
    }

    private void ExpandToDir(DungeonRoom room, Vector2Int dir)
    {
        if (Mathf.Abs(room.idx.x + dir.x) > maxDepth)
        {
            room.exits[dir] = false;
            return;
        }
        if (Mathf.Abs(room.idx.y + dir.y) > maxDepth)
        {
            room.exits[dir] = false;
            return;
        }

        if (!room.exits[dir])
        {
            if (rooms.ContainsKey(room.idx + dir))
            {
                rooms[room.idx + dir].exits[dir * -1] = false;
            }
            return;
        }

        if (rooms.ContainsKey(room.idx + dir))
        {
            if (!rooms[room.idx + dir].exits[dir * -1])
            {
                room.exits[dir] = false;
                return;
            }

            bool vertical = dir.y != 0;
            var roomPos = room.position + dir * (roomHalfSize + connectorHalfLength);
            var connector = new DungeonRoom(connectorHalfWidth, connectorHalfLength, roomPos, vertical);
            connector.SetTiles();
            return;
        }
        var newRoom = room.AddNeighbour(dir, roomHalfSize, connectorHalfWidth, connectorHalfLength, roomExitChance);
        newRoom.exits[dir * -1] = true;
        RegisterRoom(newRoom);
        roomList.Add(newRoom);
    }

    private void SpawnGoal()
    {
        var goal = FindObjectOfType<Goal>();
        var outerRooms = roomList.FindAll(r => Mathf.Abs(r.idx.x) == maxDepth || Mathf.Abs(r.idx.y) == maxDepth);
        DungeonRoom goalRoom = null;
        if (outerRooms.Count > 0)
        {
            goalRoom = outerRooms.Find(r => r.IsDeadEnd());
            if (goalRoom == null)
            {
                goalRoom = outerRooms[Random.Range(0, outerRooms.Count)];
            }
        }
        else
        {
            goalRoom = roomList.Find(r => r.IsDeadEnd());
            if (goalRoom == null)
            {
                goalRoom = roomList[Random.Range(0, roomList.Count)];
            }
        }
        goalRoom.SetTiles();
        if (goal)
        {
            goal.transform.position = goalRoom.position;
        }
        else
        {
            Instantiate(goalPrefab, goalRoom.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomHalfSize * 2, roomHalfSize * 2, 0));
        Gizmos.DrawWireCube(transform.position + Vector3.up * (roomHalfSize + connectorHalfLength), new Vector3(connectorHalfWidth * 2, connectorHalfLength * 2, 0));
    }
}

