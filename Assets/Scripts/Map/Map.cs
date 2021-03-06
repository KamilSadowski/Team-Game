using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Which tile is currently held in the given position
enum TileState
{
    empty,
    ground,
    wall,
    noTile
} // No tile means null

enum WallType
{
    center,
    topLeft,
    topRight,
    topTop,
    left,
    right,
    bottom,
    bottomLeft,
    bottomRight,
    bottomRightCorner,
    topRightCorner,
    bottomLeftCorner,
    topLeftCorner,
    topU,
    botU,
    leftU,
    rightU,
    none
}

class WallData
{
    public WallType   type;
    public Vector3Int position;

    public WallData()
    {
    }

    public WallData(WallType newType, Vector3Int pos)
    {
        type     = newType;
        position = pos;
    }
}

// Holds the data of surrounding tiles to use in map generation
class WallGenData
{
    public WallData topLeft      = new WallData();
    public WallData top          = new WallData();
    public WallData topRight     = new WallData();
    public WallData left         = new WallData();
    public WallData currentTile  = new WallData();
    public WallData right        = new WallData();
    public WallData bottomLeft   = new WallData();
    public WallData bottom       = new WallData(); // y - 1
    public WallData bottomRight  = new WallData();
    public WallData bottomBottom = new WallData(); // y - 2
}

class TileStateData
{
    public TileState  state    = new TileState();
    public Vector3Int position = new Vector3Int();
}

class TileGenData
{
    public TileStateData topLeft      = new TileStateData();
    public TileStateData top          = new TileStateData();
    public TileStateData topRight     = new TileStateData();
    public TileStateData left         = new TileStateData();
    public TileStateData currentTile  = new TileStateData();
    public TileStateData right        = new TileStateData();
    public TileStateData bottomLeft   = new TileStateData();
    public TileStateData bottom       = new TileStateData(); // y - 1
    public TileStateData bottomRight  = new TileStateData();
    public TileStateData bottomBottom = new TileStateData(); // y - 2
}

// Holds the door position and the room it belongs to
struct DoorData
{
    public Vector3Int position; // Position of the door

    public Vector3Int
        corridorStart; // Position to link a corridor to (has to be offset to avoid it clipping with the room)

    public int               roomNumber; // Room the door belongs to
    public bool              exists; // Used as a flag to check if the door has been created
    public Globals.Direction direction; // Which direction is the door facing

    public DoorData(Vector3Int position, int roomNo, Globals.Direction doorDirection)
    {
        this.position = position;
        roomNumber    = roomNo;
        exists        = false;
        direction     = doorDirection;

        // Find out where to place the corridor start based on direction
        corridorStart = position;
        switch (direction)
        {
            case Globals.Direction.north:
            {
                corridorStart.y += Globals.MIN_CORRIDOR_DIST;
                break;
            }
            case Globals.Direction.south:
            {
                corridorStart.y -= Globals.MIN_CORRIDOR_DIST;
                break;
            }
            case Globals.Direction.east:
            {
                corridorStart.x += Globals.MIN_CORRIDOR_DIST;
                break;
            }
            case Globals.Direction.west:
            {
                corridorStart.x -= Globals.MIN_CORRIDOR_DIST;
                break;
            }
        }
    }

    public void CreateDoor()
    {
        exists = true;
    }
}

public struct MapGridData
{
    int? prefabNoNullable;

    public int prefabNo
    {
        get { return prefabNoNullable ?? -1; }
        set { prefabNoNullable = value; }
    } // Index of the prefab to use (-1 means no room)

    int? roomIndexNullable;

    public int roomIndex
    {
        get { return prefabNoNullable ?? -1; }
        set { prefabNoNullable = value; }
    } // Index in the created rooms array

    public bool    isBossRoom;
    public Vector3 roomPosition;
}

public class Dungeon : MonoBehaviour
{
    GameManager   gameManager;
    EntityManager entityManager;


    // Tilemap loader variables
    [SerializeField] Tilemap wallTileMap;
    [SerializeField] Tilemap groundTileMap;

    [SerializeField] TileBase   editorGroundTile;
    [SerializeField] TileBase   editorWallTile;
    [SerializeField] TileBase   editorPropTile;
    [SerializeField] TileBase   editorDoorTile;
    [SerializeField] List<Tile> groundTiles;

    [SerializeField] List<Tile> centerWallTiles;
    [SerializeField] List<Tile> topLeftWallTiles;
    [SerializeField] List<Tile> topRightWallTiles;
    [SerializeField] List<Tile> topTopWallTiles;
    [SerializeField] List<Tile> leftWallTiles;
    [SerializeField] List<Tile> rightWallTiles;
    [SerializeField] List<Tile> bottomWallTiles;
    [SerializeField] List<Tile> bottomLeftWallTiles;
    [SerializeField] List<Tile> bottomRightWallTiles;
    [SerializeField] List<Tile> bottomRightCornerWallTiles;
    [SerializeField] List<Tile> topRightCornerWallTiles;
    [SerializeField] List<Tile> bottomLeftCornerWallTiles;
    [SerializeField] List<Tile> topLeftCornerWallTiles;
    [SerializeField] List<Tile> topUWallTiles;
    [SerializeField] List<Tile> botUWallTiles;
    [SerializeField] List<Tile> leftUWallTiles;
    [SerializeField] List<Tile> rightUWallTiles;

    [SerializeField] List<Prop> commonNearTopWallProps;
    [SerializeField] List<Prop> uncommonNearTopWallProps;
    [SerializeField] List<Prop> rareNearTopWallProps;
    [SerializeField] List<Prop> commonRoomProps;
    [SerializeField] List<Prop> uncommonRoomProps;
    [SerializeField] List<Prop> rareRoomProps;


    [SerializeField] BossRoom emptyBossRoomPrefab;
    [SerializeField] Room     emptyRoomPrefab;
    [SerializeField] Light    lightPrefab;

    // Door prefabs
    [SerializeField] List<Door> northDoors;
    [SerializeField] List<Door> southDoors;
    [SerializeField] List<Door> eastDoors;
    [SerializeField] List<Door> westDoors;

    List<Vector3Int> wallLocations = new List<Vector3Int>();

    // A list of room prefabs that can be used
    [SerializeField] List<Room> roomPrefabs     = new List<Room>();
    [SerializeField] List<Room> bossRoomPrefabs = new List<Room>();

    [SerializeField] List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] List<GameObject> bossList = new List<GameObject>();


    // Represents generated rooms on a 2d grid
    MapGridData[,] mapGrid = new MapGridData[Globals.MAP_GRID_SIZE, Globals.MAP_GRID_SIZE];

    // Room positions on a 2D grid
    Globals.Grid2D currentRoomPosition = new Globals.Grid2D(Globals.MAP_GRID_SIZE / 2, Globals.MAP_GRID_SIZE / 2); // Starting in the middle of the grid
    Globals.Grid2D tmpRoomPosition     = new Globals.Grid2D();

    Globals.Direction       newDirection;
    List<Globals.Direction> directionsAvailable = new List<Globals.Direction>();

    List<List<DoorData>> doorsCreationData = new List<List<DoorData>>(); // Index 1 is room number, index 2 is door number
    List<Room>           roomsCreated      = new List<Room>();
    List<Globals.Grid2D> roomGridPositions = new List<Globals.Grid2D>();
    Light                mapLight;

    // Room creation variables
    Vector3 roomPosition = Vector3.zero;
    Vector3 roomRotation = new Vector3(0.0f, 0.0f, 90.0f);

    [SerializeField] private GameObject HideFromMinimapPrefab;

    public GameObject GetRandomEnemy()
    {
        return enemyList[Random.Range(0, enemyList.Count)];
    }

    public GameObject GetRandomBoss()
    {
        return bossList[Random.Range(0, bossList.Count)];
    }

    private void Start()
    {
        gameManager   = FindObjectOfType<GameManager>();
        entityManager = FindObjectOfType<EntityManager>();
        mapLight      = Instantiate(lightPrefab);

        // In a rare event that the map creation crashes
        // Reload the map scene and generate it again
        // Crashes are so rare that this should fix the problem at a small cost of generating the map twice
        try
        {
            CreateDungeon();
        }
        catch
        {
            Portal.TeleportTo(Globals.Scenes.Dungeon);
        }
    }

    public Tilemap GetGroundTileMap()
    {
        return groundTileMap;
    }

    // Checks if the editor tile is meant to be ground
    // This includes prop tiles which would otherwise be ignored
    bool IsGround(TileBase tile)
    {
        return (tile == editorGroundTile || tile == editorPropTile);
    }

    public Tilemap GetWallTileMap()
    {
        return wallTileMap;
    }

    public void LightSwitch(bool on)
    {
        mapLight.enabled = on;
    }

    // Does not update the z position
    public void SetLightPosition(Vector3 position)
    {
        position.z                  = mapLight.transform.position.z;
        mapLight.transform.position = position;
    }

    // Reads the prefab and writes it to the map array, adds a room instance with no doors, door data for the room is added to the doors list
    void WriteRoom(ref TileBase[,] map, GameObject roomPrefab, Vector3Int roomOffset,
        ref List<Room> rooms, ref List<List<DoorData>> doors, int roomNo,
        ref List<Vector3Int> prefabs, bool bossRoom)
    {
        Vector3Int prefabPosition = new Vector3Int(); // Position within the prefab
        Vector3Int globalPosition = new Vector3Int(); // Position on the map

        // Add a new room to the list
        if (bossRoom)
        {
            rooms.Add(Instantiate<BossRoom>(emptyBossRoomPrefab));
        }
        else
        {
            rooms.Add(Instantiate<Room>(emptyRoomPrefab));
        }

        // Fill in the room that has just been added with data from the prefab
        Room    roomPrefabScript  = roomPrefab.GetComponent<Room>();
        Tilemap roomPrefabTilemap = roomPrefab.GetComponent<Tilemap>();
        roomPrefabTilemap.CompressBounds();
        Vector3 roomCenter = new Vector3();
        int     doorsFound = 0;
        //roomCenter += roomPrefabTilemap.cellBounds.size / 2;
        rooms[rooms.Count - 1].Initialise(roomPrefabTilemap.cellBounds.size, this, roomPrefabScript.minEnemyNo, roomPrefabScript.maxEnemyNo);

        doors.Add(new List<DoorData>());

        globalPosition.y = roomOffset.x;

        for (int x = (int)roomPrefabTilemap.cellBounds.min.x; x < roomPrefabTilemap.cellBounds.max.x; ++x)
        {
            globalPosition.x = roomOffset.y + roomPrefabTilemap.cellBounds.size.y;
            for (int y = (int)roomPrefabTilemap.cellBounds.min.y; y < roomPrefabTilemap.cellBounds.max.y; ++y)
            {
                prefabPosition.y = y;
                prefabPosition.x = x;
                TileBase tile = roomPrefabTilemap.GetTile(prefabPosition);
                if (tile != null)
                {
                    map[globalPosition.x, globalPosition.y] = roomPrefabTilemap.GetTile(prefabPosition);

                    // Check if the tile can be spawned on
                    if (tile == editorGroundTile)
                    {
                        rooms[rooms.Count - 1].AddGroundTile(globalPosition);
                    }

                    // Is the tile a door
                    else if (tile == editorDoorTile)
                    {
                        // Find the door direction
                        // North
                        if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x - 1, prefabPosition.y, prefabPosition.z))))
                        {
                            doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.north));
                            roomCenter += globalPosition;
                            ++doorsFound;
                        }
                        // South
                        else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x + 1, prefabPosition.y, prefabPosition.z))))
                        {
                            doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.south));
                            roomCenter += globalPosition;
                            ++doorsFound;
                        }
                        // East
                        else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x, prefabPosition.y + 1, prefabPosition.z))))
                        {
                            doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.east));
                            roomCenter += globalPosition;
                            ++doorsFound;
                        }
                        // West
                        else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x, prefabPosition.y - 1, prefabPosition.z))))
                        {
                            doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.west));
                            roomCenter += globalPosition;
                            ++doorsFound;
                        }
                    }

                    // Is the tile a prop
                    else if (tile == editorPropTile)
                    {
                        rooms[rooms.Count - 1].AddGroundTile(globalPosition);
                        prefabs.Add(globalPosition);
                    }
                }

                globalPosition.x--;
            }

            globalPosition.y++;
        }

        roomCenter /= doorsFound;
        roomCenter =  groundTileMap.CellToWorld(new Vector3Int((int)roomCenter.x, (int)roomCenter.y, (int)roomCenter.z));
        rooms[rooms.Count - 1].center = roomCenter;


        // Minimap Code
        // Initialize HideFromMinimap image

        rooms[^1].transform.position = roomCenter;

        var c = roomPrefab.GetComponentInChildren<Canvas>();
        if (c)
        {
            var newC = rooms[^1].GetComponentInChildren<Canvas>();
            if (newC)
            {
                var T       = newC.transform as RectTransform;
                var prefabT = roomPrefab.GetComponentInChildren<Canvas>().transform as RectTransform;
                if (T && prefabT)
                {
                    var prefabTRect = prefabT.rect;
                    var rect        = T.rect;
                    rect.center = roomCenter;
                    rect.height = prefabTRect.height;
                    rect.width  = prefabTRect.width;
                    T.sizeDelta = prefabT.sizeDelta;
                }
            }
        }
    }

    // Writes a corridor of the specified length in a specified direction to the specified map
    void WriteCorridor(ref TileBase[,] map, Vector3Int startPosition, Globals.Direction direction, int length)
    {
        switch (direction)
        {
            case Globals.Direction.north:
            {
                for (int i = 0; i < length; ++i)
                {
                    map[startPosition.x, startPosition.y + i] = editorGroundTile;
                }

                break;
            }
            case Globals.Direction.south:
            {
                for (int i = 0; i < length; ++i)
                {
                    map[startPosition.x, startPosition.y - i] = editorGroundTile;
                }

                break;
            }
            case Globals.Direction.east:
            {
                for (int i = 0; i < length; ++i)
                {
                    map[startPosition.x + i, startPosition.y] = editorGroundTile;
                }

                break;
            }
            case Globals.Direction.west:
            {
                for (int i = 0; i < length; ++i)
                {
                    map[startPosition.x - i, startPosition.y] = editorGroundTile;
                }

                break;
            }
        }
    }

    // Connects the 2 selected rooms using the closest doors from door data, doors needed are added to the instance of the room
    void ConnectRooms(ref TileBase[,] map, ref List<Room> rooms, ref List<List<DoorData>> roomDoors, int room1,
                      int room2, Globals.Direction door1Direction, Globals.Direction door2Direction)
    {
        // If no doors, return
        if (roomDoors[room1].Count == 0 || roomDoors[room2].Count == 0)
        {
            return;
        }

        // Find out which of the doors are the closest
        int door1 = FindDoorWithDirection(roomDoors[room1], door1Direction);
        int door2 = FindDoorWithDirection(roomDoors[room2], door2Direction);

        // Returns the index of the door with the matching direction
        int FindDoorWithDirection(List<DoorData> roomDoors, Globals.Direction direction)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (roomDoors[i].direction == direction)
                {
                    return i;
                }
            }
            return -1;
        }

        // Create a door if it does not exist
        if (!roomDoors[room1][door1].exists)
        {
            Vector3 pos = groundTileMap.GetCellCenterWorld(roomDoors[room1][door1].position);
            roomDoors[room1][door1].CreateDoor();

            // Choose the door prefab to instantiate based on the door direction
            switch (roomDoors[room1][door1].direction)
            {
                case Globals.Direction.north:
                {
                    int index = Random.Range(0, northDoors.Count - 1);
                    rooms[room1].AddDoor(Instantiate<Door>(northDoors[index],
                                    pos + northDoors[index].offset,
                                    Quaternion.identity));
                    break;
                }
                case Globals.Direction.south:
                {
                    int index = Random.Range(0, southDoors.Count - 1);
                    rooms[room1].AddDoor(Instantiate<Door>(southDoors[index],
                                    pos + southDoors[index].offset,
                                    Quaternion.identity));
                    break;
                }
                case Globals.Direction.east:
                {
                    int index = Random.Range(0, eastDoors.Count - 1);
                    rooms[room1].AddDoor(Instantiate<Door>(eastDoors[index],
                                    pos + eastDoors[index].offset,
                                    Quaternion.identity));
                    break;
                }
                case Globals.Direction.west:
                {
                    int index = Random.Range(0, westDoors.Count - 1);
                    rooms[room1].AddDoor(Instantiate<Door>(westDoors[index],
                                    pos + westDoors[index].offset,
                                    Quaternion.identity));
                    break;
                }
            }

            rooms[room1].doors[rooms[room1].doors.Count - 1].SetRoom(rooms[room1]);

            // Door needs a small corridor, rooms will link to the end of it
            WriteCorridor(ref map,
                            roomDoors[room1][door1].position, 
                            roomDoors[room1][door1].direction, 
                            (int)Vector3Int.Distance(roomDoors[room1][door1].corridorStart, 
                                            roomDoors[room1][door1].position) + 1);
        }

        if (!roomDoors[room2][door2].exists)
        {
            Vector3 pos = groundTileMap.GetCellCenterWorld(roomDoors[room2][door2].position);
            roomDoors[room2][door2].CreateDoor();

            switch (roomDoors[room2][door2].direction)
            {
                case Globals.Direction.north:
                {
                    int index = Random.Range(0, northDoors.Count - 1);
                    rooms[room2].AddDoor(Instantiate<Door>(northDoors[index], pos + northDoors[index].offset,
                        Quaternion.identity));
                    break;
                }
                case Globals.Direction.south:
                {
                    int index = Random.Range(0, southDoors.Count - 1);
                    rooms[room2].AddDoor(Instantiate<Door>(southDoors[index], pos + southDoors[index].offset,
                        Quaternion.identity));
                    break;
                }
                case Globals.Direction.east:
                {
                    int index = Random.Range(0, eastDoors.Count - 1);
                    rooms[room2].AddDoor(Instantiate<Door>(eastDoors[index], pos + eastDoors[index].offset,
                        Quaternion.identity));
                    break;
                }
                case Globals.Direction.west:
                {
                    int index = Random.Range(0, westDoors.Count - 1);
                    rooms[room2].AddDoor(Instantiate<Door>(westDoors[index], pos + westDoors[index].offset,
                        Quaternion.identity));
                    break;
                }
            }

            rooms[room2].doors[rooms[room2].doors.Count - 1].SetRoom(rooms[room2]);

            // Door needs a small corridor, rooms will link to the end of it
            WriteCorridor(
                ref map,
                roomDoors[room2][door2].position,
                roomDoors[room2][door2].direction,
                (int)Vector3Int.Distance(
                    roomDoors[room2][door2].corridorStart,
                    roomDoors[room2][door2].position) + 1);
        }



        // Connect the rooms together
        Vector3Int currentPosition = roomDoors[room1][door1].corridorStart;
        while (true)
        {
            // Right
            if (currentPosition.x < roomDoors[room2][door2].corridorStart.x)
            {
                int distance = roomDoors[room2][door2].corridorStart.x - currentPosition.x;
                WriteCorridor(ref map, currentPosition, Globals.Direction.east, distance);
                currentPosition.x += distance;
            }

            // Left
            else if (currentPosition.x > roomDoors[room2][door2].corridorStart.x)
            {
                int distance = currentPosition.x - roomDoors[room2][door2].corridorStart.x;
                WriteCorridor(ref map, currentPosition, Globals.Direction.west, distance);
                currentPosition.x -= distance;
            }

            // Up
            else if (currentPosition.y < roomDoors[room2][door2].corridorStart.y)
            {
                int distance = roomDoors[room2][door2].corridorStart.y - currentPosition.y;
                WriteCorridor(ref map, currentPosition, Globals.Direction.north, distance);
                currentPosition.y += distance;
            }

            // Down
            else if (currentPosition.y > roomDoors[room2][door2].corridorStart.y)
            {
                int distance = currentPosition.y - roomDoors[room2][door2].corridorStart.y;
                WriteCorridor(ref map, currentPosition, Globals.Direction.south, distance);
                currentPosition.y -= distance;
            }
            else
            {
                break;
            }
        }


        // Minimap code
        // Put image between the two rooms


        // Get the position between the rooms
        var startPos = roomDoors[room1][door1].corridorStart;
        var endPos = currentPosition;
        var dist = startPos - endPos;
        var middle = startPos + dist;
                Debug.Log("Distance :");
                Debug.Log(dist);

        //var o = Instantiate(HideFromMinimapPrefab, middle , Quaternion.identity);
        //if (o)
        //{
        //    var canvasTransform = o.GetComponentInChildren<Canvas>().transform as RectTransform;
        //    if (canvasTransform)
        //    {
        //    }
        //}

    }

    // Returns the tile type for the given tile
    TileState GetTileState(TileBase tile)
    {
        if (tile == null)
        {
            return TileState.noTile;
        }

        if (tile == editorWallTile) return TileState.wall;
        else if (tile == editorGroundTile) return TileState.ground;
        else return TileState.empty;
    }

    // This overload supports arrays and boundary checks
    TileState GetTileState(TileBase[,] tileMap, int x, int y)
    {
        if (x < 0 || y < 0 || x >= tileMap.GetLength(0) || y >= tileMap.GetLength(1))
        {
            return TileState.noTile;
        }

        if (tileMap[x, y] == editorWallTile) return TileState.wall;
        else if (IsGround(tileMap[x, y])) return TileState.ground;
        else return TileState.empty;
    }


    // Returns true if finished creating a dungeon
    bool CreateDungeon()
    {
        // Create a room
        Vector3Int  roomPosition = new Vector3Int(500, 500, 0);
        TileBase[,] readMap      = new TileBase[1000, 1000];

        List<Vector3Int> props = new List<Vector3Int>();

        // Used for checking the biggest room on each row and column
        int[] biggestX = new int[Globals.MAP_GRID_SIZE];
        int[] biggestY = new int[Globals.MAP_GRID_SIZE];

        // Map grid creation
        // Create the first room
        mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo = Random.Range(0, roomPrefabs.Count);
        Tilemap chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
        if (chosenPrefab.cellBounds.size.x > biggestX[currentRoomPosition.y])
        {
            biggestX[currentRoomPosition.y] = chosenPrefab.cellBounds.size.x;
        }

        chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
        if (chosenPrefab.cellBounds.size.y > biggestY[currentRoomPosition.x])
        {
            biggestY[currentRoomPosition.x] = chosenPrefab.cellBounds.size.y;
        }

        roomGridPositions.Add(currentRoomPosition);

        // Create the other rooms
        while (roomGridPositions.Count < Globals.MAX_ROOM_NO)
        {
            currentRoomPosition = roomGridPositions[Random.Range(0, roomGridPositions.Count)];

            UpdateDirectionsAvailable(currentRoomPosition);
            if (RandomiseAndUpdatePosition(ref currentRoomPosition) && CheckRoomSpace(currentRoomPosition))
            {
                // Last room is always the boss room
                if (roomGridPositions.Count < Globals.MAX_ROOM_NO - 1)
                {
                    // Randomise the room to place
                    mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo = Random.Range(0, roomPrefabs.Count);

                    // Update the biggest size
                    chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
                }
                else
                {
                    // Boss room
                    mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo   = Random.Range(0, bossRoomPrefabs.Count);
                    mapGrid[currentRoomPosition.x, currentRoomPosition.y].isBossRoom = true;
                    chosenPrefab                                                     = bossRoomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
                }


                //chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
                if (chosenPrefab.cellBounds.size.x > biggestX[currentRoomPosition.y])
                {
                    biggestX[currentRoomPosition.y] = chosenPrefab.cellBounds.size.x;
                }

                if (chosenPrefab.cellBounds.size.y > biggestY[currentRoomPosition.x])
                {
                    biggestY[currentRoomPosition.x] = chosenPrefab.cellBounds.size.y;
                }


                roomGridPositions.Add(currentRoomPosition);
            }
        }

        Debug.Log("X:");
        foreach (int x in biggestX)
        {
            Debug.Log(x + " ");
        }

        Debug.Log("Y:");
        foreach (int y in biggestY)
        {
            Debug.Log(y + " ");
        }

        // Find the position for each row and column
        int[] posX = new int[Globals.MAP_GRID_SIZE];
        int[] posY = new int[Globals.MAP_GRID_SIZE];

        for (int i = 0; i < Globals.MAP_GRID_SIZE; ++i)
        {
            if (biggestX[i] > 0)
            {
                if (i - 1 > 0)
                {
                    posX[i] = biggestX[i - 1];
                    posX[i] += posX[i - 1] + Globals.ROOM_PLACEMENT_OFFSET; // Add some offset for wall and corridor generation
                }
            }

            if (biggestY[i] > 0)
            {
                if (i - 1 > 0)
                {
                    posY[i] =  biggestY[i - 1];
                    posY[i] += posY[i - 1] + Globals.ROOM_PLACEMENT_OFFSET;
                }
            }
        }

        GameObject currentRoomGameObject;

        // Tile map creation based on grid map
        // Read random rooms and write them to readMap
        for (int i = 0; i < Globals.MAX_ROOM_NO; ++i)
        {
            // Choose the room
            if (mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].isBossRoom)
            {
                currentRoomGameObject = bossRoomPrefabs[mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].prefabNo].gameObject;
            }
            else
            {
                currentRoomGameObject = roomPrefabs[mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].prefabNo].gameObject;
            }

            Room    roomPrefabScript  = currentRoomGameObject.GetComponent<Room>();
            Tilemap roomPrefabTilemap = currentRoomGameObject.GetComponent<Tilemap>();

            roomPosition.x = posX[roomGridPositions[i].y];

            roomPosition.y = posY[roomGridPositions[i].x];

            WriteRoom(ref readMap, currentRoomGameObject, roomPosition,
                ref roomsCreated, ref doorsCreationData, i, ref props,
                mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].isBossRoom);

            // Update the map grid with the index of the created room
            mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].roomIndex = i;
        }

        // Go through the entire map
        for (int y = 0; y < Globals.MAP_GRID_SIZE; ++y)
        {
            for (int x = 0; x < Globals.MAP_GRID_SIZE; ++x)
            {
                // Linking every room to the one above and to the left will connect all of the rooms together
                if (x < Globals.MAP_GRID_SIZE - 1)
                {
                    if (mapGrid[x + 1, y].roomIndex != -1 && mapGrid[x, y].roomIndex != -1)
                    {
                        ConnectRooms(ref readMap, ref roomsCreated, ref doorsCreationData,
                            mapGrid[x, y].roomIndex,
                            mapGrid[x + 1, y].roomIndex, 
                            Globals.Direction.east, Globals.Direction.west);
                    }
                }

                if (y < Globals.MAP_GRID_SIZE - 1)
                {
                    if (mapGrid[x, y].roomIndex != -1 && mapGrid[x, y + 1].roomIndex != -1)
                    {
                        ConnectRooms(ref readMap, ref roomsCreated, ref doorsCreationData,
                            mapGrid[x, y].roomIndex,
                            mapGrid[x, y + 1].roomIndex,
                            Globals.Direction.north, Globals.Direction.south);
                    }
                }
            }
        }

        PlaceGround(ref readMap);

        FindWalls(readMap);

        FillInWalls(readMap);

        PlaceProps(readMap, props);

        // Teleport the player to the starting location
        Vector3 startPosition = roomsCreated[0].GetRandomGroundPosition();
        if (!gameManager.TeleportPlayer(startPosition))
        {
            entityManager.TryCreatePlayer(startPosition);
            gameManager.TeleportPlayer(startPosition);
        }

        // First room is closed and the rest will only close when entered
        roomsCreated[0].EnterRoom();

        for (int i = 1; i < roomsCreated.Count; ++i)
        {
            roomsCreated[i].OpenDoors();
        }

        // Teleport the camera to avoid a delay of it having to move towards the player

        FollowingCamera.instance.Teleport(startPosition);

        gameManager.GivePlayerEquipment();

        return true;
    }

    // Finds places where walls should be (call fill in walls to actually give them tiles)
    void FindWalls(TileBase[,] readMap)
    {
        TileGenData currentWall = new TileGenData();

        // Check all of the ground edges and see if a wall needs to be inserted
        Vector3Int index        = new Vector3Int(0, 0, 0);
        Vector3Int newWallIndex = new Vector3Int(0, 0, 0);
        for (int y = 0; y < readMap.GetLength(1); y++)
        {
            for (int x = 0; x < readMap.GetLength(0); x++)
            {
                index.x = x;
                index.y = y;

                // Check the ground tile's surrounding tiles for future wall locations
                currentWall.currentTile.state = GetTileState(readMap, index.x, index.y);
                if (currentWall.currentTile.state == TileState.ground)
                {
                    currentWall.topLeft.position = new Vector3Int(index.x - 1, index.y + 1);
                    currentWall.topLeft.state    = GetTileState(readMap, index.x - 1, index.y + 1);

                    currentWall.top.position = new Vector3Int(index.x, index.y + 1, index.z);
                    currentWall.top.state    = GetTileState(readMap, index.x, index.y + 1);

                    currentWall.topRight.position = new Vector3Int(index.x + 1, index.y + 1, index.z);
                    currentWall.topRight.state    = GetTileState(readMap, index.x + 1, index.y + 1);

                    currentWall.left.position = new Vector3Int(index.x - 1, index.y, index.z);
                    currentWall.left.state    = GetTileState(readMap, index.x - 1, index.y);

                    currentWall.right.position = new Vector3Int(index.x + 1, index.y, index.z);
                    currentWall.right.state    = GetTileState(readMap, index.x + 1, index.y);

                    currentWall.bottomLeft.position = new Vector3Int(index.x - 1, index.y - 1, index.z);
                    currentWall.bottomLeft.state    = GetTileState(readMap, index.x - 1, index.y - 1);

                    currentWall.bottom.position = new Vector3Int(index.x, index.y - 1, index.z);
                    currentWall.bottom.state    = GetTileState(readMap, index.x, index.y - 1);

                    currentWall.bottomRight.position = new Vector3Int(index.x + 1, index.y - 1, index.z);
                    currentWall.bottomRight.state    = GetTileState(readMap, index.x + 1, index.y - 1);

                    // Top left
                    if (currentWall.topLeft.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          -= 1;
                        newWallIndex.y                          += 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Top
                    if (currentWall.top.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.y                          += 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Top right
                    if (currentWall.topRight.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          += 1;
                        newWallIndex.y                          += 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Left
                    if (currentWall.left.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          -= 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Right
                    if (currentWall.right.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          += 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Bottom left
                    if (currentWall.bottomLeft.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          -= 1;
                        newWallIndex.y                          -= 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Bottom
                    if (currentWall.bottom.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.y                          -= 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }

                    // Bottom right
                    if (currentWall.bottomRight.state == TileState.empty)
                    {
                        newWallIndex                            =  index;
                        newWallIndex.x                          += 1;
                        newWallIndex.y                          -= 1;
                        readMap[newWallIndex.x, newWallIndex.y] =  editorWallTile;
                        wallLocations.Add(newWallIndex);
                    }
                }
            }
        }
    }

    // The function returns which wall should be placed at the given position based on the tiles surrounding it
    WallData CheckWall(TileBase[,] readMap, Vector3Int position)
    {
        // Update the tile
        TileGenData tile = FillInSurroundingTiles(readMap, position);

        if (tile.top.state == TileState.ground)
        {
            if (tile.left.state == TileState.ground)
            {
                if (tile.right.state == TileState.ground)
                {
                    // Bottom U
                    return new WallData(WallType.botU, position);
                }
                else if (tile.bottom.state == TileState.ground)
                {
                    // Center
                    return new WallData(WallType.center, position);
                }
                else if (tile.bottomRight.state != TileState.empty)
                {
                    // Right U
                    return new WallData(WallType.rightU, position);
                }
                else
                {
                    // Bottom right corner
                    return new WallData(WallType.bottomRightCorner, position);
                }
            }

            if (tile.right.state == TileState.ground)
            {
                if (tile.left.state == TileState.ground)
                {
                    // Bottom U
                    return new WallData(WallType.botU, position);
                }
                else if (tile.bottom.state == TileState.ground)
                {
                    // Center
                    return new WallData(WallType.center, position);
                }
                else if (tile.bottomLeft.state != TileState.empty)
                {
                    // Left U
                    return new WallData(WallType.leftU, position);
                }
                else
                {
                    // Bottom left corner
                    return new WallData(WallType.bottomLeftCorner, position);
                }
            }

            // Bottom
            return new WallData(WallType.bottom, position);
        }

        if (tile.right.state != TileState.empty)
        {
            if (tile.left.state == TileState.empty)
            {
                // Top left corner
                if (tile.bottom.state == TileState.ground ||
                    tile.bottomBottom.state == TileState.ground)
                {
                    if (tile.topRight.state == TileState.ground &&
                        tile.right.state == TileState.wall)
                    {
                        return new WallData(WallType.topTop, position);
                    }
                    return new WallData(WallType.topLeftCorner, position);
                }
                // Left
                else
                {
                    if (tile.right.state == TileState.ground ||
                        IsCenterWall(readMap, tile.right.position))
                    {
                        return new WallData(WallType.left, position);
                    }
                }
            }
            if (tile.right.state != TileState.empty &&
                tile.bottom.state != TileState.ground &&
                tile.bottomBottom.state != TileState.ground &&
                IsCenterWall(readMap, tile.right.position))
            {
                return new WallData(WallType.left, position);
            }
        }

        if (tile.left.state != TileState.empty)
        {
            if (tile.right.state == TileState.empty)
            {
                // Top right corner
                if (tile.bottom.state == TileState.ground ||
                    tile.bottomBottom.state == TileState.ground)
                {
                    if (tile.topLeft.state == TileState.ground &&
                        tile.left.state == TileState.wall)
                    {
                        return new WallData(WallType.topTop, position);
                    }
                    return new WallData(WallType.topRightCorner, position);
                }
                // Right
                else
                {
                    if (tile.left.state == TileState.ground ||
                        IsCenterWall(readMap, tile.left.position))
                    {
                        return new WallData(WallType.right, position);
                    }
                }
            }
            if (tile.left.state != TileState.empty &&            
                tile.bottom.state != TileState.ground && 
                tile.bottomBottom.state != TileState.ground && 
                IsCenterWall(readMap, tile.left.position))
            {
                return new WallData(WallType.right, position);
            }
        }

        if (tile.bottom.state == TileState.ground)
        {
            // Center
            return new WallData(WallType.center, position);
        }

        if (tile.top.state == TileState.empty)
        {
            if (tile.bottomRight.state == TileState.wall &&
                tile.bottomLeft.state != TileState.ground)
            {
                // Top right corner
                return new WallData(WallType.topRightCorner, position);
            }
            else if (tile.bottomLeft.state == TileState.wall &&
                     tile.bottomRight.state != TileState.ground)
            {
                // Top left corner
                return new WallData(WallType.topLeftCorner, position);
            }
            else if ((tile.bottomLeft.state != TileState.ground &&
                     tile.bottomRight.state != TileState.ground))
            {
                // Top U
                return new WallData(WallType.topU, position);
            }
            else if (tile.bottom.state != TileState.wall)
            {
                if (tile.right.state == TileState.ground ||
                    IsCenterWall(readMap, tile.right.position))
                {
                    return new WallData(WallType.left, position);
                }
                else if (tile.left.state == TileState.ground ||
                         IsCenterWall(readMap, tile.left.position))
                {
                    return new WallData(WallType.right, position);
                }
            }

        }
        else
        {
            if (tile.bottomBottom.state != TileState.ground)
            {
                if (tile.right.state == TileState.ground ||
                    IsCenterWall(readMap, tile.right.position))
                {
                    return new WallData(WallType.left, position);
                }
                else if (tile.left.state == TileState.ground ||
                         IsCenterWall(readMap, tile.left.position))
                {
                    return new WallData(WallType.right, position);
                }
            }
            else
            {
                if (tile.bottomRight.state == TileState.wall &&
                    tile.bottomLeft.state != TileState.wall &&
                    tile.topRight.state != TileState.ground)
                {
                    // Top right corner
                    return new WallData(WallType.topRightCorner, position);
                }
                else if (tile.bottomLeft.state == TileState.wall && 
                         tile.bottomRight.state != TileState.wall &&
                         tile.topLeft.state != TileState.ground)
                {
                    // Top left corner
                    return new WallData(WallType.topLeftCorner, position);
                }
            }

        }

        return new WallData(WallType.none, position);
    }

    bool IsCenterWall(TileBase[,] readMap, Vector3Int position)
    {
        TileGenData tile = FillInSurroundingTiles(readMap, position);
        return tile.bottom.state == TileState.ground; // && tile.top.state != TileState.ground;
    }

    TileGenData FillInSurroundingTiles(TileBase[,] readMap, Vector3Int position)
    {
        TileGenData tile = new TileGenData();

        tile.topLeft.state    = GetTileState(readMap, position.x - 1, position.y + 1);
        tile.topLeft.position = new Vector3Int(position.x - 1, position.y + 1, position.z);

        tile.top.state    = GetTileState(readMap, position.x, position.y + 1);
        tile.top.position = new Vector3Int(position.x, position.y + 1, position.z);

        tile.topRight.state    = GetTileState(readMap, position.x + 1, position.y + 1);
        tile.topRight.position = new Vector3Int(position.x + 1, position.y + 1, position.z);

        tile.left.state    = GetTileState(readMap, position.x - 1, position.y);
        tile.left.position = new Vector3Int(position.x - 1, position.y, position.z);

        tile.currentTile.state    = GetTileState(readMap, position.x, position.y);
        tile.currentTile.position = new Vector3Int(position.x, position.y, position.z);

        tile.right.state    = GetTileState(readMap, position.x + 1, position.y);
        tile.right.position = new Vector3Int(position.x + 1, position.y, position.z);

        tile.bottomLeft.state    = GetTileState(readMap, position.x - 1, position.y - 1);
        tile.bottomLeft.position = new Vector3Int(position.x - 1, position.y - 1, position.z);

        tile.bottom.state    = GetTileState(readMap, position.x, position.y - 1);
        tile.bottom.position = new Vector3Int(position.x, position.y - 1, position.z);

        tile.bottomRight.state    = GetTileState(readMap, position.x + 1, position.y - 1);
        tile.bottomRight.position = new Vector3Int(position.x + 1, position.y - 1, position.z);

        tile.bottomBottom.state    = GetTileState(readMap, position.x, position.y - 2);
        tile.bottomBottom.position = new Vector3Int(position.x, position.y - 2, position.z);

        return tile;
    }

    void FillInSurroundingWalls(TileBase[,] readMap, ref WallGenData wall, Vector3Int position)
    {
        wall.topLeft      = CheckWall(readMap, new Vector3Int(position.x - 1, position.y + 1, position.z));
        wall.top          = CheckWall(readMap, new Vector3Int(position.x,     position.y + 1, position.z));
        wall.topRight     = CheckWall(readMap, new Vector3Int(position.x + 1, position.y + 1, position.z));
        wall.left         = CheckWall(readMap, new Vector3Int(position.x - 1, position.y,     position.z));
        wall.currentTile  = CheckWall(readMap, new Vector3Int(position.x,     position.y,     position.z));
        wall.right        = CheckWall(readMap, new Vector3Int(position.x + 1, position.y,     position.z));
        wall.bottomLeft   = CheckWall(readMap, new Vector3Int(position.x - 1, position.y - 1, position.z));
        wall.bottom       = CheckWall(readMap, new Vector3Int(position.x,     position.y - 1, position.z));
        wall.bottomRight  = CheckWall(readMap, new Vector3Int(position.x + 1, position.y - 1, position.z));
        wall.bottomBottom = CheckWall(readMap, new Vector3Int(position.x,     position.y - 2, position.z));
    }

    void SetTile(ref Tilemap tileMap, Vector3Int position, List<Tile> list)
    {
        tileMap.SetTile(position, list[Random.Range(0, list.Count)]);
    }


    // This function can only fill in basic walls and will ignore wall extensions
    // Those will need to be added manually
    void FillInWall(WallType type, Vector3Int position)
    {
        switch (type)
        {
            case WallType.center:
            {
                SetTile(ref groundTileMap, position, centerWallTiles);
                break;
            }
            case WallType.topLeft:
            {
                SetTile(ref wallTileMap, position, topLeftWallTiles);
                break;
            }
            case WallType.topRight:
            {
                SetTile(ref wallTileMap, position, topRightWallTiles);
                break;
            }
            case WallType.topTop:
            {
                SetTile(ref wallTileMap, position, topTopWallTiles);
                break;
            }
            case WallType.left:
            {
                SetTile(ref wallTileMap, position, leftWallTiles);
                break;
            }
            case WallType.right:
            {
                SetTile(ref wallTileMap, position, rightWallTiles);
                break;
            }
            case WallType.bottom:
            {
                SetTile(ref wallTileMap, position, bottomWallTiles);
                break;
            }
            case WallType.bottomLeft:
            {
                SetTile(ref wallTileMap, position, bottomLeftWallTiles);
                break;
            }
            case WallType.bottomRight:
            {
                SetTile(ref wallTileMap, position, bottomRightWallTiles);
                break;
            }
            case WallType.bottomRightCorner:
            {
                SetTile(ref wallTileMap, position, bottomRightCornerWallTiles);
                break;
            }
            case WallType.topRightCorner:
            {
                SetTile(ref wallTileMap, position, topRightCornerWallTiles);
                break;
            }
            case WallType.bottomLeftCorner:
            {
                SetTile(ref wallTileMap, position, bottomLeftCornerWallTiles);
                break;
            }
            case WallType.topLeftCorner:
            {
                SetTile(ref wallTileMap, position, topLeftCornerWallTiles);
                break;
            }
            case WallType.topU:
            {
                SetTile(ref wallTileMap, position, topUWallTiles);
                break;
            }
            case WallType.botU:
            {
                SetTile(ref wallTileMap, position, botUWallTiles);
                break;
            }
            case WallType.leftU:
            {
                SetTile(ref wallTileMap, position, leftUWallTiles);
                break;
            }
            case WallType.rightU:
            {
                SetTile(ref wallTileMap, position, rightUWallTiles);
                break;
            }
        }
    }

    // Goes through found walls and fills them in with the corresponding tiles based on their surrounding tiles
    void FillInWalls(TileBase[,] readMap)
    {
        // Fill in the wall tiles based on ground tiles
        for (int i = 0; i < wallLocations.Count; i++)
        {
            WallData    wallData = CheckWall(readMap, wallLocations[i]);
            TileGenData tileData = FillInSurroundingTiles(readMap, wallData.position);
            FillInWall(wallData.type, wallLocations[i]);

            // Fill in the wall extensions

            // Top left
            if ((wallData.type == WallType.left ||
                 wallData.type == WallType.topLeftCorner) &&
                 tileData.top.state == TileState.empty)
            {
                FillInWall(WallType.topLeft, tileData.top.position);
            }

            // Top right
            else if ((wallData.type == WallType.right ||
                     wallData.type == WallType.topRightCorner) &&
                     tileData.top.state == TileState.empty)
            {
                FillInWall(WallType.topRight, tileData.top.position);
            }

            // Top top
            else if(wallData.type == WallType.center)
            {
                TileGenData topTileData = FillInSurroundingTiles(readMap, tileData.top.position);
                if (topTileData.currentTile.state != TileState.ground)
                {
                    if (topTileData.left.state != TileState.empty &&
                        topTileData.bottomLeft.state != TileState.wall &&
                        topTileData.bottomRight.state != TileState.wall)
                    {
                        FillInWall(WallType.topU, tileData.top.position);
                        continue;
                    }
                    else if (topTileData.bottomLeft.state != TileState.wall)
                    {
                        FillInWall(WallType.topRightCorner, tileData.top.position);
                        continue;
                    }

                    if (topTileData.left.state != TileState.empty &&
                        topTileData.bottomRight.state != TileState.wall)
                    {
                        FillInWall(WallType.topLeftCorner, tileData.top.position);
                        continue;
                    }
                }
                FillInWall(WallType.topTop, tileData.top.position);

            }

            // Bottom right
            else if((wallData.type == WallType.left ||
                    wallData.type == WallType.bottomLeftCorner))
            {
                if (tileData.bottomBottom.state == TileState.empty)
                {
                    FillInWall(WallType.bottomRight, tileData.bottom.position);
                }
            }

            // Bottom Left
            else if((wallData.type == WallType.right ||
                    wallData.type == WallType.bottomRightCorner))
            {
                if (tileData.bottomBottom.state == TileState.empty)     
                {
                    FillInWall(WallType.bottomLeft, tileData.bottom.position);
                }
            }
        }
    }

    // Places random props in the specified positions
    void PlaceProps(TileBase[,] readMap, List<Vector3Int> propPositons)
    {
        const int totalChance = Globals.COMMON_CHANCE + Globals.UNCOMMON_CHANCE + Globals.RARE_CHANCE;

        int rarity = 0;
        Prop prop;

        foreach (Vector3Int propPos in propPositons)
        {
            rarity = Random.Range(0, totalChance);

            // Check if to place a top wall prop
            if (IsGround(readMap[propPos.x, propPos.y + 1]))
            {
                // Check how rare of a prop type to use
                if (rarity <= Globals.COMMON_CHANCE)
                {
                    int propIndex = Random.Range(0, commonRoomProps.Count);
                    prop = commonRoomProps[propIndex];
                }
                else if (rarity < Globals.UNCOMMON_CHANCE + Globals.COMMON_CHANCE)
                {
                    int propIndex = Random.Range(0, uncommonRoomProps.Count);
                    prop = uncommonRoomProps[propIndex];
                }
                else
                {
                    int propIndex = Random.Range(0, rareRoomProps.Count);
                    prop = rareRoomProps[propIndex];
                }
            }
            else
            {
                if (rarity <= Globals.COMMON_CHANCE)
                {
                    int propIndex = Random.Range(0, commonNearTopWallProps.Count);
                    prop = commonNearTopWallProps[propIndex];
                }
                else if (rarity < Globals.UNCOMMON_CHANCE + Globals.COMMON_CHANCE)
                {
                    int propIndex = Random.Range(0, uncommonNearTopWallProps.Count);
                    prop = uncommonNearTopWallProps[propIndex];
                }
                else
                {
                    int propIndex = Random.Range(0, rareNearTopWallProps.Count);
                    prop = rareNearTopWallProps[propIndex];
                }
            }
            // Create the selected prop
            entityManager.TryCreateEntity(prop.gameObject,
                                          groundTileMap.GetCellCenterWorld(propPos) + prop.GetOffset());
        }
    }

    // Places ground tiles
    void PlaceGround(ref TileBase[,] readMap)
    {
        // Go through all of the stored tiles and place corresponding tiles onto the map
        Vector3Int index = new Vector3Int(0, 0, 0);
        for (int y = 0; y < readMap.GetLength(1); y++)
        {
            for (int x = 0; x < readMap.GetLength(0); x++)
            {
                index.x = x;
                index.y = y;

                // Ground tile
                if (IsGround(readMap[x, y]))
                {
                    groundTileMap.SetTile(index, groundTiles[Random.Range(0, groundTiles.Count)]);
                }
                // Wall tile
                else if (readMap[x, y] == editorWallTile)
                {
                    // Store the wall tile locations to add once the entire ground is added
                    // Keep the locations relative to the room
                    wallLocations.Add(index);
                }
            }
        }
    }

    // Chooses a direction at random and updates the current position
    // Returns false if failed to randomise direction
    // Update directions available beforehand
    bool RandomiseAndUpdatePosition(ref Globals.Grid2D roomPos)
    {
        if (directionsAvailable.Count > 0)
        {
            newDirection = (Globals.Direction)Random.Range(0, directionsAvailable.Count);
            roomPos      = PositionInDirection(roomPos, newDirection);

            return true;
        }

        return false;
    }

    // Updates directions available based on the current room position
    void UpdateDirectionsAvailable(Globals.Grid2D roomPos, bool addEmptySpaces = true, bool addFullSpaces = false)
    {
        directionsAvailable.Clear();

        tmpRoomPosition = PositionInDirection(roomPos, Globals.Direction.north);
        if (CheckRoomSpace(tmpRoomPosition, addEmptySpaces, addFullSpaces))
        {
            directionsAvailable.Add(Globals.Direction.north);
        }

        tmpRoomPosition = PositionInDirection(roomPos, Globals.Direction.east);
        if (CheckRoomSpace(tmpRoomPosition, addEmptySpaces, addFullSpaces))
        {
            directionsAvailable.Add(Globals.Direction.east);
        }

        tmpRoomPosition = PositionInDirection(roomPos, Globals.Direction.south);
        if (CheckRoomSpace(tmpRoomPosition, addEmptySpaces, addFullSpaces))
        {
            directionsAvailable.Add(Globals.Direction.south);
        }

        tmpRoomPosition = PositionInDirection(roomPos, Globals.Direction.west);
        if (CheckRoomSpace(tmpRoomPosition, addEmptySpaces, addFullSpaces))
        {
            directionsAvailable.Add(Globals.Direction.west);
        }
    }

    // Returns the position in the specified direction, does not check array bounds or if the space is empty
    Globals.Grid2D PositionInDirection(Globals.Grid2D position, Globals.Direction chosenDirection)
    {
        switch (chosenDirection)
        {
            case Globals.Direction.north:
            {
                position.y++;
                break;
            }
            case Globals.Direction.east:
            {
                position.x++;
                break;
            }
            case Globals.Direction.south:
            {
                position.y--;
                break;
            }
            case Globals.Direction.west:
            {
                position.x--;
                break;
            }
        }

        return position;
    }

    // Returns true if the room space is within boundaries
    bool CheckRoomSpace(Globals.Grid2D roomPos, bool canBeEmpty = true, bool canBeFilled = false)
    {
        if (roomPos.x < Globals.MAP_GRID_SIZE &&
            roomPos.x >= 0 &&
            roomPos.y < Globals.MAP_GRID_SIZE &&
            roomPos.y >= 0)
        {
            if (mapGrid[roomPos.x, roomPos.y].prefabNo == -1 && canBeFilled)
            {
                return true;
            }
            else if (mapGrid[roomPos.x, roomPos.y].prefabNo == -1 && canBeEmpty)
            {
                return true;
            }
        }

        return false;
    }
}