using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    // Global objects
    EntityManager entityManager;

    // Room Data
    public Vector3Int size { get; private set; } // Room size
    List<Vector3Int> groundTiles; // Positions of any tile that entites can stand on
    Dungeon map;
    Tilemap groundTileMap;
    public List<Door> doors { get; private set; }

    // Room properties
    int enemiesAlive;
    [SerializeField] int enemyNo = 2;

    // Room state
    bool wasEntered = false;
    bool wasCleared = false;

    // Temporary variables
    NotPlayer currentNPC;
    int currentEntityID;

    // Update is called once per frame
    void Update()
    {
        
    }


    // Room constructor
    public void Initialise(Vector3Int roomSize, Dungeon thisMap, int enemyNumber)
    {
        size = roomSize;
        doors = new List<Door>();
        groundTiles = new List<Vector3Int>();
        entityManager = FindObjectOfType<EntityManager>();
        map = thisMap;
        groundTileMap = map.GetGroundTileMap();
        enemyNo = enemyNumber;
    }

    public int GetEnemyNo()
    {
        return enemyNo;
    }

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }

    public void AddGroundTile(Vector3Int tile)
    {
        groundTiles.Add(tile);
    }

    // Returns a random position in the room
    public Vector3 GetRandomGroundPosition()
    {
        return groundTileMap.GetCellCenterWorld(groundTiles[Random.Range(0, groundTiles.Count)]);
    }
    
    // Close all the doors and spawn enemies
    public IEnumerator EnterRoom()
    {
        yield return new WaitForFixedUpdate();
        if (!wasEntered)
        {
            CloseDoors();
            for (int i = 0; i < enemyNo; ++i)
            {
                CreateEntity();
            }
            wasEntered = true;
        }
        yield return null;
    }

    public void ExitRoom()
    {

    }

    void CreateEntity()
    {
        currentEntityID = entityManager.TryCreateListedNPC(0, GetRandomGroundPosition());
        if (currentEntityID != -1)
        {
            currentNPC = entityManager.GetEntity(currentEntityID) as NotPlayer;
            currentNPC.AddRoom(this);
            ++enemiesAlive;
        }   
    }

    public void OpenDoors()
    {
        foreach (Door door in doors)
        {
            door.OpenDoor();
        }
    }

    public void CloseDoors()
    {
        foreach (Door door in doors)
        {
            door.CloseDoor();
        }
    }

    public void EntityRemoved()
    {
        --enemiesAlive;
        if (enemiesAlive <= 0)
        {
            OpenDoors();
            wasCleared = true;
        }
    }
}
