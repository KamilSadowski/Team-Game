using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    // Global objects
    protected EntityManager entityManager;

    // Room Data
    public Vector3Int size { get; private set; } // Room size
    public List<Door> doors { get; private set; }
    protected Vector3 center = new Vector3(); // Room position on the map
    protected List<Vector3Int> groundTiles; // Positions of any tile that entites can stand on
    protected Dungeon map;
    protected Tilemap groundTileMap;

    // Room properties
    [SerializeField] protected int enemyNo = 2;
    protected int enemiesAlive;

    // Room state
    protected bool wasEntered = false;
    protected bool wasCleared = false;

    // Temporary variables
    protected NotPlayer currentNPC;
    protected int currentEntityID;

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

    public void SetRoomCenter(Vector3 roomCenter)
    {
        center = roomCenter;
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
    public virtual void EnterRoom()
    {
        map.LightSwitch(true);
        map.SetLightPosition(center);
        if (!wasEntered)
        {
            CloseDoors();
            for (int i = 0; i < enemyNo; ++i)
            {
                CreateEntity();
            }

            wasEntered = true;

            // Minimap Code
            // Disable hiding image
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas)
            {
                var img = canvas.GetComponentInChildren<Image>();
                if (img)
                {
                    img.color = Color.clear;
                }
            }
        }
    }

    public virtual void ExitRoom()
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

    public virtual void EntityRemoved()
    {
        --enemiesAlive;
        if (enemiesAlive <= 0)
        {
            OpenDoors();
            wasCleared = true;
        }
    }
}