using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Which tile is currently held in the given position
enum TileState { empty, ground, wall, noTile } // No tile means null

// Holds the data of surrounding tiles to use in map generation
struct TileGenData
{
	public TileState topLeft;
	public TileState top;
	public TileState topRight;
	public TileState left;
	public TileState currentTile;
	public TileState right;
	public TileState bottomLeft;
	public TileState bottom;
	public TileState bottomRight;
}

// Holds the door position and the room it belongs to
struct DoorData
{
	public Vector3Int position; // Position of the door
	public Vector3Int corridorStart; // Position to link a corridor to (has to be offset to avoid it clipping with the room)
	public int roomNumber; // Room the door belongs to
	public bool exists; // Used as a flag to check if the door has been created
	public Globals.Direction direction; // Which direction is the door facing

	public DoorData(Vector3Int position, int roomNo, Globals.Direction doorDirection)
    {
		this.position = position;
		roomNumber = roomNo;
		exists = false;
		direction = doorDirection;

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
	public int prefabNo { get { return prefabNoNullable ?? -1; } set { prefabNoNullable = value; } } // Index of the prefab to use (-1 means no room)

	int? roomIndexNullable;
	public int roomIndex { get { return prefabNoNullable ?? -1; } set { prefabNoNullable = value; } } // Index in the created rooms array
	public bool isBossRoom;
	public Vector3 roomPosition;
}

public class Dungeon : MonoBehaviour
{
	GameManager gameManager;
	EntityManager entityManager;

	// Tilemap loader variables
	[SerializeField] Tilemap wallTileMap;
	[SerializeField] Tilemap groundTileMap;

	[SerializeField] TileBase editorGroundTile;
	[SerializeField] TileBase editorWallTile;
	[SerializeField] TileBase editorPropTile;
	[SerializeField] TileBase editorDoorTile;
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

	[SerializeField] List<Prop> nearTopWallProps;


	[SerializeField] Room emptyRoomPrefab;

	// Door prefabs
	[SerializeField] List<Door> northDoors;
	[SerializeField] List<Door> southDoors;
	[SerializeField] List<Door> eastDoors;
	[SerializeField] List<Door> westDoors;

	List<Vector3Int> wallLocations = new List<Vector3Int>();

	TileGenData currentWall;

	// A list of room prefabs that can be used
	[SerializeField] List<Room> roomPrefabs = new List<Room>();
	[SerializeField] List<Room> bossRoomPrefabs = new List<Room>();

	// Represents generated rooms on a 2d grid
	MapGridData[,] mapGrid = new MapGridData[Globals.MAP_GRID_SIZE, Globals.MAP_GRID_SIZE]; 

	// Room positions on a 2D grid
	Globals.Grid2D currentRoomPosition = new Globals.Grid2D(Globals.MAP_GRID_SIZE / 2, Globals.MAP_GRID_SIZE / 2); // Starting in the middle of the grid
	Globals.Grid2D tmpRoomPosition = new Globals.Grid2D();

	Globals.Direction newDirection;
	List<Globals.Direction> directionsAvailable = new List<Globals.Direction>();

	List<List<DoorData>> doorsCreationData = new List<List<DoorData>>(); // Index 1 is room number, index 2 is door number
	List<Room> roomsCreated = new List<Room>();
	List<Globals.Grid2D> roomGridPositions = new List<Globals.Grid2D>();

	// Room creation variables
	Vector3 roomPosition = Vector3.zero;
	Vector3 roomRotation = new Vector3(0.0f, 0.0f, 90.0f);

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
		entityManager = FindObjectOfType<EntityManager>();
		CreateDungeon();
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

	// Reads the prefab and writes it to the map array, adds a room instance with no doors, door data for the room is added to the doors list
	void WriteRoom(ref TileBase[,] map, GameObject roomPrefab, Vector3Int roomOffset, 
				   ref List<Room> rooms, ref List<List<DoorData>> doors, int roomNo, 
				   ref List<Vector3Int> prefabs)
    {
		Vector3Int prefabPosition = new Vector3Int(); // Position within the prefab
		Vector3Int globalPosition = new Vector3Int(); // Position on the map

		rooms.Add(Instantiate<Room>(emptyRoomPrefab));
		Room roomPrefabScript = roomPrefab.GetComponent<Room>();
		Tilemap roomPrefabTilemap = roomPrefab.GetComponent<Tilemap>();
		roomPrefabTilemap.CompressBounds();
		rooms[rooms.Count - 1].Initialise(roomPrefabTilemap.cellBounds.size, this, roomPrefabScript.GetEnemyNo());
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
						}
						// South
						else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x + 1, prefabPosition.y, prefabPosition.z))))
						{
							doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.south));
						}
						// East
						else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x, prefabPosition.y + 1, prefabPosition.z))))
						{
							doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.east));
						}
						// West
						else if (IsGround(roomPrefabTilemap.GetTile(new Vector3Int(prefabPosition.x, prefabPosition.y - 1, prefabPosition.z))))
						{
							doors[roomNo].Add(new DoorData(globalPosition, roomNo, Globals.Direction.west));
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
	void ConnectRooms(ref TileBase[,] map, ref List<Room> rooms, ref List<List<DoorData>> roomDoors, int room1, int room2)
	{
		// If no doors, return
		if (roomDoors[room1].Count == 0 || roomDoors[room2].Count == 0) { return; }

		// Find out which of the doors are the closest
		int closestRoom1Door = 0;
		int closestRoom2Door = 0;
		float closestDistance = float.MaxValue;
		float tmpDist = 0;

		for (int room1Door = 0; room1Door < roomDoors[room1].Count; ++room1Door)
		{
			for (int room2Door = 0; room2Door < roomDoors[room2].Count; ++room2Door)
			{
				tmpDist = Vector3Int.Distance(roomDoors[room1][room1Door].corridorStart,
											  roomDoors[room2][room2Door].corridorStart);
				if (tmpDist < closestDistance)
				{
					closestRoom1Door = room1Door;
					closestRoom2Door = room2Door;
					closestDistance = tmpDist;
				}
			}
		}

		// Create a door if it does not exist
		if (!roomDoors[room1][closestRoom1Door].exists)
		{
			Vector3 pos = groundTileMap.GetCellCenterWorld(roomDoors[room1][closestRoom1Door].position);	
			roomDoors[room1][closestRoom1Door].CreateDoor();

			// Choose the door prefab to instantiate based on the door direction
			switch (roomDoors[room1][closestRoom1Door].direction)
			{
				case Globals.Direction.north:
					{
						rooms[room1].AddDoor(Instantiate<Door>(northDoors[Random.Range(0, northDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.south:
					{
						rooms[room1].AddDoor(Instantiate<Door>(southDoors[Random.Range(0, southDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.east:
					{
						rooms[room1].AddDoor(Instantiate<Door>(eastDoors[Random.Range(0, eastDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.west:
					{
						rooms[room1].AddDoor(Instantiate<Door>(westDoors[Random.Range(0, westDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
			}
			
			rooms[room1].doors[rooms[room1].doors.Count - 1].SetRoom(rooms[room1]);

			// Door needs a small corridor, rooms will link to the end of it
			WriteCorridor(ref map, roomDoors[room1][closestRoom1Door].position, roomDoors[room1][closestRoom1Door].direction, 
						  (int)Vector3Int.Distance(roomDoors[room1][closestRoom1Door].corridorStart, roomDoors[room1][closestRoom1Door].position) + 1);
		}

		if (!roomDoors[room2][closestRoom2Door].exists)
		{
			Vector3 pos = groundTileMap.GetCellCenterWorld(roomDoors[room2][closestRoom2Door].position);
			roomDoors[room2][closestRoom2Door].CreateDoor();

			switch (roomDoors[room2][closestRoom2Door].direction)
			{
				case Globals.Direction.north:
					{
						rooms[room2].AddDoor(Instantiate<Door>(northDoors[Random.Range(0, northDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.south:
					{
						rooms[room2].AddDoor(Instantiate<Door>(southDoors[Random.Range(0, southDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.east:
					{
						rooms[room2].AddDoor(Instantiate<Door>(eastDoors[Random.Range(0, eastDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
				case Globals.Direction.west:
					{
						rooms[room2].AddDoor(Instantiate<Door>(westDoors[Random.Range(0, westDoors.Count - 1)], pos, Quaternion.identity));
						break;
					}
			}

			rooms[room2].doors[rooms[room2].doors.Count - 1].SetRoom(rooms[room2]);

			// Door needs a small corridor, rooms will link to the end of it
			WriteCorridor(ref map, roomDoors[room2][closestRoom2Door].position, roomDoors[room2][closestRoom2Door].direction,
						  (int)Vector3Int.Distance(roomDoors[room2][closestRoom2Door].corridorStart, roomDoors[room2][closestRoom2Door].position) + 1);
		}

		// Connect the rooms together
		Vector3Int currentPosition = roomDoors[room1][closestRoom1Door].corridorStart;
		while (true)
        {
			// Right
			if (currentPosition.x < roomDoors[room2][closestRoom2Door].corridorStart.x)
            {
				int distance = roomDoors[room2][closestRoom2Door].corridorStart.x - currentPosition.x;
				WriteCorridor(ref map, currentPosition, Globals.Direction.east, distance);
				currentPosition.x += distance;
			}

			// Left
			else if (currentPosition.x > roomDoors[room2][closestRoom2Door].corridorStart.x)
			{
				int distance = currentPosition.x - roomDoors[room2][closestRoom2Door].corridorStart.x;
				WriteCorridor(ref map, currentPosition, Globals.Direction.west, distance);
				currentPosition.x -= distance;
			}

			// Up
			else if (currentPosition.y < roomDoors[room2][closestRoom2Door].corridorStart.y)
			{
				int distance = roomDoors[room2][closestRoom2Door].corridorStart.y - currentPosition.y;
				WriteCorridor(ref map, currentPosition, Globals.Direction.north, distance);
				currentPosition.y += distance;
			}

			// Down
			else if (currentPosition.y > roomDoors[room2][closestRoom2Door].corridorStart.y)
			{
				int distance = currentPosition.y - roomDoors[room2][closestRoom2Door].corridorStart.y;
				WriteCorridor(ref map, currentPosition, Globals.Direction.south, distance);
				currentPosition.y -= distance;
			}
			else
            {
				break;
            }
		}
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
		Vector3Int roomPosition = new Vector3Int(500, 500, 0);
		TileBase[,] readMap = new TileBase[1000, 1000];

		List<Vector3Int> props = new List<Vector3Int>();

		// Used for checking the biggest room on each row and column
		int[] biggestX = new int[Globals.MAP_GRID_SIZE];
		int[] biggestY = new int[Globals.MAP_GRID_SIZE];

		// Map grid creation
		// Create the first room
		mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo = Random.Range(0, roomPrefabs.Count);
		Tilemap chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
		if (chosenPrefab.cellBounds.size.y > biggestX[currentRoomPosition.y])
		{
			biggestX[currentRoomPosition.y] = chosenPrefab.cellBounds.size.y;
		}

		chosenPrefab = roomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
		if (chosenPrefab.cellBounds.size.x > biggestY[currentRoomPosition.x])
		{
			biggestY[currentRoomPosition.x] = chosenPrefab.cellBounds.size.x;
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
					mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo = Random.Range(0, bossRoomPrefabs.Count);
					mapGrid[currentRoomPosition.x, currentRoomPosition.y].isBossRoom = true;
					chosenPrefab = bossRoomPrefabs[mapGrid[currentRoomPosition.x, currentRoomPosition.y].prefabNo].GetComponent<Tilemap>();
				}


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
			}
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
					posY[i] = biggestY[i - 1];
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
			
			Room roomPrefabScript = currentRoomGameObject.GetComponent<Room>();
			Tilemap roomPrefabTilemap = currentRoomGameObject.GetComponent<Tilemap>();

			roomPosition.x = posX[roomGridPositions[i].y];

			roomPosition.y = posY[roomGridPositions[i].x];

			WriteRoom(ref readMap, currentRoomGameObject, roomPosition, ref roomsCreated, ref doorsCreationData, i, ref props);

			// Update the map grid with the index of the created room
			mapGrid[roomGridPositions[i].x, roomGridPositions[i].y].roomIndex = i;

		}

		// Go through the entire map
		for (int y = 0; y < Globals.MAP_GRID_SIZE; ++y)
		{
			for (int x = 0; x < Globals.MAP_GRID_SIZE; ++x)
			{
				// Linking every room to the one above and to the left will connect all of the rooms together
				if (x > 0)
                {
					if (mapGrid[x - 1, y].roomIndex != -1 && mapGrid[x, y].roomIndex != -1)
					{
						ConnectRooms(ref readMap, ref roomsCreated, ref doorsCreationData,
									 mapGrid[x, y].roomIndex,
									 mapGrid[x - 1, y].roomIndex);
					}
				}
				if (y > 0)
                {
					if (mapGrid[x, y].roomIndex != -1 && mapGrid[x, y - 1].roomIndex != -1)
                    {
						ConnectRooms(ref readMap, ref roomsCreated, ref doorsCreationData, 
														 mapGrid[x, y].roomIndex, 
														 mapGrid[x, y - 1].roomIndex);
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
		StartCoroutine(roomsCreated[0].EnterRoom());
		
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
		// Check all of the ground edges and see if a wall needs to be inserted
		Vector3Int index = new Vector3Int(0, 0, 0);
		Vector3Int newWallIndex = new Vector3Int(0, 0, 0);
		for (int y = 0; y < readMap.GetLength(1); y++)
		{
			for (int x = 0; x < readMap.GetLength(0); x++)
			{
				index.x = x;
				index.y = y;

				// Check the ground tile's surrounding tiles for future wall locations
				currentWall.currentTile = GetTileState(readMap, index.x, index.y);
				if (currentWall.currentTile == TileState.ground)
				{
					currentWall.topLeft = GetTileState(readMap, index.x - 1, index.y + 1);
					currentWall.top = GetTileState(readMap, index.x, index.y + 1);
					currentWall.topRight = GetTileState(readMap, index.x + 1, index.y + 1);
					currentWall.left = GetTileState(readMap, index.x - 1, index.y);

					currentWall.right = GetTileState(readMap, index.x + 1, index.y);
					currentWall.bottomLeft = GetTileState(readMap, index.x - 1, index.y - 1);
					currentWall.bottom = GetTileState(readMap, index.x, index.y - 1);
					currentWall.bottomRight = GetTileState(readMap, index.x + 1, index.y - 1);

					// Top left
					if (currentWall.topLeft == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x -= 1;
						newWallIndex.y += 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Top
					if (currentWall.top == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.y += 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Top right
					if (currentWall.topRight == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x += 1;
						newWallIndex.y += 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Left
					if (currentWall.left == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x -= 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Right
					if (currentWall.right == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x += 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Bottom left
					if (currentWall.bottomLeft == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x -= 1;
						newWallIndex.y -= 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Bottom
					if (currentWall.bottom == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.y -= 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}

					// Bottom right
					if (currentWall.bottomRight == TileState.empty)
					{
						newWallIndex = index;
						newWallIndex.x += 1;
						newWallIndex.y -= 1;
						readMap[newWallIndex.x, newWallIndex.y] = editorWallTile;
						wallLocations.Add(newWallIndex);
					}
				}
			}
		}
	}

	// Goes through found walls and fills them in with the corresponding tiles based on their surrounding tiles
	void FillInWalls(TileBase[,] readMap)
    {
		// Fill in the wall tiles based on ground tiles
		for (int i = 0; i < wallLocations.Count; i++)
		{
			// Update the tile
			currentWall.topLeft = GetTileState(readMap, wallLocations[i].x - 1, wallLocations[i].y + 1);
			currentWall.top = GetTileState(readMap, wallLocations[i].x, wallLocations[i].y + 1);
			currentWall.topRight = GetTileState(readMap, wallLocations[i].x + 1, wallLocations[i].y + 1);
			currentWall.left = GetTileState(readMap, wallLocations[i].x - 1, wallLocations[i].y);
			currentWall.currentTile = GetTileState(readMap, wallLocations[i].x, wallLocations[i].y);
			currentWall.right = GetTileState(readMap, wallLocations[i].x + 1, wallLocations[i].y);
			currentWall.bottomLeft = GetTileState(readMap, wallLocations[i].x - 1, wallLocations[i].y - 1);
			currentWall.bottom = GetTileState(readMap, wallLocations[i].x, wallLocations[i].y - 1);
			currentWall.bottomRight = GetTileState(readMap, wallLocations[i].x + 1, wallLocations[i].y - 1);

			// Normal wall
			if (currentWall.bottom == TileState.ground &&
				currentWall.left != TileState.noTile &&
				currentWall.left != TileState.empty &&
				currentWall.right != TileState.noTile &&
				currentWall.right != TileState.empty)
			{
				wallTileMap.SetTile(wallLocations[i], centerWallTiles[Random.Range(0, centerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition, topTopWallTiles[Random.Range(0, topTopWallTiles.Count)]);
				}
			}

			// Bottom
			else if (currentWall.top == TileState.ground &&
					 currentWall.left == TileState.wall &&
					 currentWall.right == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i], bottomWallTiles[Random.Range(0, bottomWallTiles.Count)]);
			}

			// Bottom left
			else if (currentWall.top != TileState.empty &&
					 currentWall.top != TileState.noTile &&
					 currentWall.left == TileState.wall &&
					 currentWall.right != TileState.wall)
			{
				// Check whether to use a corner wall tile
				if (currentWall.bottom != TileState.wall)
				{
					wallTileMap.SetTile(wallLocations[i], bottomLeftWallTiles[Random.Range(0, bottomLeftWallTiles.Count)]);
				}
				else if (currentWall.right != TileState.empty && currentWall.right != TileState.noTile)
				{
					wallTileMap.SetTile(wallLocations[i], bottomLeftCornerWallTiles[Random.Range(0, bottomLeftCornerWallTiles.Count)]);
				}
				else
				{
					wallTileMap.SetTile(wallLocations[i], rightWallTiles[Random.Range(0, rightWallTiles.Count)]);
				}
			}

			// Bottom right
			else if (currentWall.top != TileState.empty &&
					 currentWall.top != TileState.noTile &&
					 currentWall.right == TileState.wall &&
					 currentWall.left != TileState.wall)
			{
				// Check whether to use a corner wall tile
				if (currentWall.bottom != TileState.wall)
				{
					wallTileMap.SetTile(wallLocations[i], bottomRightWallTiles[Random.Range(0, bottomRightWallTiles.Count)]);
				}
				else if (currentWall.left != TileState.empty && currentWall.left != TileState.noTile)
					{
					wallTileMap.SetTile(wallLocations[i], bottomRightCornerWallTiles[Random.Range(0, bottomRightCornerWallTiles.Count)]);
				}
				else
				{
					wallTileMap.SetTile(wallLocations[i], leftWallTiles[Random.Range(0, leftWallTiles.Count)]);
				}
			}

			// Left
			else if (currentWall.right != TileState.empty &&
					 currentWall.right != TileState.noTile &&
					 currentWall.left != TileState.wall &&
					 currentWall.bottom == TileState.wall &&
					 currentWall.bottomLeft != TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i], leftWallTiles[Random.Range(0, leftWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition, topLeftWallTiles[Random.Range(0, topLeftWallTiles.Count)]);
				}
			}

			// Right
			else if (currentWall.left != TileState.empty &&
					 currentWall.left != TileState.noTile &&
					 currentWall.right != TileState.wall &&
					 currentWall.bottom == TileState.wall &&
					 currentWall.bottomRight != TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i], rightWallTiles[Random.Range(0, rightWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition, topRightWallTiles[Random.Range(0, topRightWallTiles.Count)]);
				}
			}

			// Top right corner
			else if (currentWall.bottom == TileState.wall &&
					 currentWall.bottomLeft == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i], topRightCornerWallTiles[Random.Range(0, topRightCornerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition, topLeftWallTiles[Random.Range(0, topLeftWallTiles.Count)]);
				}
			}

			// Top left corner
			else if (currentWall.bottom == TileState.wall &&
					 currentWall.bottomRight == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i], topLeftCornerWallTiles[Random.Range(0, topLeftCornerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition, topRightWallTiles[Random.Range(0, topRightWallTiles.Count)]);
				}
			}
		}
	}

	// Places random props in the specified positions
	void PlaceProps(TileBase[,] readMap, List<Vector3Int> propPositons)
    {
		foreach (Vector3Int propPos in propPositons)
		{
			if (readMap[propPos.x, propPos.y])
            {
				int propIndex = Random.Range(0, nearTopWallProps.Count);
				entityManager.TryCreateEntity(nearTopWallProps[propIndex].gameObject,
											  groundTileMap.GetCellCenterWorld(propPos) + nearTopWallProps[propIndex].GetOffset());
			}
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
			roomPos = PositionInDirection(roomPos, newDirection);

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
