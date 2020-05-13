using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class DungeonRoom
{
    public Vector2Int halfSize;

    public Dictionary<Vector2Int, bool> exits;

    public Vector2Int idx;
    public Vector2 position;


    public void SetTiles(float obstacleChance = 0)
    {
        var centerTilePos = DungeonGenerator.Instance.tilemap.WorldToCell(position);
        for (int x = -halfSize.x; x < halfSize.x; x++)
        {
            for (int y = -halfSize.y; y < halfSize.y; y++)
            {
                Tile tileToPlace = DungeonGenerator.Ground;
                if (x == -halfSize.x)
                {
                    if (!exits[Vector2Int.left])
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                    else if (y > 0 || y < -1)
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                }
                else if (x == halfSize.x - 1)
                {
                    if (!exits[Vector2Int.right])
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                    else if (y > 0 || y < -1)
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                }
                else if (y == -halfSize.y)
                {
                    if (!exits[Vector2Int.down])
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                    else if (x > 0 || x < -1)
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                }
                else if (y == halfSize.y - 1)
                {
                    if (!exits[Vector2Int.up])
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                    else if (x > 0 || x < -1)
                    {
                        tileToPlace = DungeonGenerator.Wall;
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < obstacleChance)
                    {
                        tileToPlace = DungeonGenerator.Obstacle;
                    }
                }
                DungeonGenerator.Instance.tilemap.SetTile(new Vector3Int((int)position.x + x, (int)position.y + y, 0), tileToPlace);
            }
        }
    }

    /// <summary>
    /// Use this for rooms
    /// </summary>
    /// <param name="_halfSize"></param>
    /// <param name="exitChance"></param>
    public DungeonRoom(int _halfSize, float exitChance, int x, int y, Vector2 _position)
    {
        exits = new Dictionary<Vector2Int, bool>();
        halfSize = new Vector2Int(_halfSize, _halfSize);
        exits[Vector2Int.up] = Random.Range(0f, 1f) < exitChance;
        exits[Vector2Int.down] = Random.Range(0f, 1f) < exitChance;
        exits[Vector2Int.right] = Random.Range(0f, 1f) < exitChance;
        exits[Vector2Int.left] = Random.Range(0f, 1f) < exitChance;
        idx = new Vector2Int(x, y);
        position = _position;
    }

    /// <summary>
    /// Use this for connectors
    /// </summary>
    /// <param name="halfWidth"></param>
    /// <param name="halfLength"></param>
    /// <param name="vertical"></param>
    public DungeonRoom(int halfWidth, int halfLength, Vector2 _position, bool vertical)
    {
        exits = new Dictionary<Vector2Int, bool>();
        if (vertical)
        {
            halfSize = new Vector2Int(halfWidth, halfLength);
        }
        else halfSize = new Vector2Int(halfLength, halfWidth);
        exits[Vector2Int.up] = vertical;
        exits[Vector2Int.down] = vertical;
        exits[Vector2Int.right] = !vertical;
        exits[Vector2Int.left] = !vertical;
        position = _position;
    }

    public DungeonRoom AddNeighbour(Vector2Int dir, int roomHalfSize, int connectorHalfWidth, int connectorHalfLength, float roomExitChance)
    {
        bool vertical = dir.y != 0;
        var roomPos = position + dir * (roomHalfSize + connectorHalfLength);
        var connector = new DungeonRoom(connectorHalfWidth, connectorHalfLength, roomPos, vertical);
        connector.SetTiles();
        roomPos += dir * (roomHalfSize + connectorHalfLength);
        return new DungeonRoom(roomHalfSize, roomExitChance, idx.x + dir.x, idx.y + dir.y, roomPos);
    }

    public bool IsDeadEnd()
    {
        int numExits = 0;
        foreach (var exit in exits.Values)
        {
            if (exit)
            {
                numExits++;
            }
        }
        return numExits == 1;
    }
}