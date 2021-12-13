using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Which tile is currently held in the given position
enum TileState { empty, ground, wall }

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

public class Dungeon : MonoBehaviour
{
	GameManager gameManager;

	// Tilemap loader variables
	[SerializeField] Tilemap wallTileMap;
	[SerializeField] Tilemap groundTileMap;
	[SerializeField] TileBase editorGroundTile;
	[SerializeField] TileBase editorWallTile;
	[SerializeField] TileBase editorPropTile;
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

	[SerializeField] List<TileBase> mapPropTiles;
	List<Vector3Int> wallLocations = new List<Vector3Int>();

	TileGenData currentWall;

	// A list of room prefabs that can be used
	[SerializeField] List<Room> roomPrefabs = new List<Room>();

	// List of rooms created on a 2D grid
	bool[,] hasRoom = new bool[Globals.MAP_GRID_SIZE, Globals.MAP_GRID_SIZE];

	// Room positions on a 2D grid
	Globals.Grid2D currentRoomPosition = new Globals.Grid2D();
	Globals.Grid2D tmpRoomPosition = new Globals.Grid2D();

	Globals.Direction newDirection;
	List<Globals.Direction> directionsAvailable = new List<Globals.Direction>();

	List<Room> roomsCreated = new List<Room>();
	List<Globals.Grid2D> roomGridPositions = new List<Globals.Grid2D>();

	// Room creation variables
	Vector3 roomPosition = Vector3.zero;
	Vector3 roomRotation = new Vector3(0.0f, 0.0f, 90.0f);

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
		CreateDungeon();
	}

    // Reads the prefab and returns a 2D vector of tiles
    TileBase[,] ReadMap(Tilemap tileMap, int maxHeight, int maxWidth)
    {
		Vector3Int position = new Vector3Int();
		TileBase[,] map = new TileBase[maxWidth, maxHeight];
		for (int y = 0; y < maxHeight; y++)
		{
			for (int x = 0; x < maxWidth; x++)
			{
				// Starts at center so the position needs to be offset by 50% in each direction
				position.y = maxWidth / 2 - x;
				position.x = -maxHeight / 2 + y;
				map[x, y] = tileMap.GetTile(position);
			}
		}

		return map;

	}

	// Returns the tile type for the given tile
	TileState GetTileState(TileBase tile)
    {
		if (tile == editorWallTile) return TileState.wall;
		else if (tile == editorGroundTile) return TileState.ground;
		else return TileState.empty;
	}


	// Returns true if finished creating a dungeon
	bool CreateDungeon()
	{
		// Create a room
		Tilemap editorTileMap = roomPrefabs[0].gameObject.GetComponentInChildren<Tilemap>();
		Vector3Int roomOffset = new Vector3Int(10, 10, 0);

		CreateRoom(roomPrefabs[0].gameObject.GetComponentInChildren<Tilemap>(), roomOffset);

		roomOffset.x = -10;
		CreateRoom(roomPrefabs[1].gameObject.GetComponentInChildren<Tilemap>(), roomOffset);

		TilemapCollider2D collider = wallTileMap.gameObject.GetComponent<TilemapCollider2D>();
		collider.ProcessTilemapChanges();
		
		CompositeCollider2D compositeCollider = wallTileMap.gameObject.GetComponent<CompositeCollider2D>();
		compositeCollider.GenerateGeometry();
		
		wallTileMap.RefreshAllTiles();


		//// Create the first room
		//if (roomsCreated.Count == 0)
		//{
		//	// Starting room should be in the middle of the map rather than in a corner
		//	currentRoomPosition.x = Globals.MAP_GRID_SIZE / 2;
		//	currentRoomPosition.y = Globals.MAP_GRID_SIZE / 2;
		//	CreateRoom(roomPrefabs[0], currentRoomPosition);
		//	return false;
		//}		
		//// Create a random room near one of the rooms that were already placed
		//else if (roomsCreated.Count < Globals.MAX_ROOM_NO - 1)
		//{
		//	currentRoomPosition = roomGridPositions[Random.Range(0, roomsCreated.Count)];
		//
		//	UpdateDirectionsAvailable(currentRoomPosition);
		//	if (RandomiseAndUpdatePosition(ref currentRoomPosition) && CheckRoomSpace(currentRoomPosition))
		//	{
		//		CreateRoom(roomPrefabs[0], currentRoomPosition);
		//	}
		//
		//	return false;
		//}
		//// Go through each room and give them corridors
		//else
		//{
		//	for (int room = 0; room < roomsCreated.Count; room++)
		//	{
		//		// Check which doors lead to a room and enable them
		//		UpdateDirectionsAvailable(roomGridPositions[room], false, true);
		//		for (int door = 0; door < directionsAvailable.Count; door++)
		//		{
		//			roomsCreated[room].CreateCorridor(directionsAvailable[door], 
		//										  PositionInDirection(roomGridPositions[room], 
		//										  directionsAvailable[door]));
		//		}
		//	}
		//}
		//
		//gameManager.TeleportPlayer(roomsCreated[0].transform.position);
		//gameManager.GivePlayerEquipment();
		//
		return true;
	}

	// Creates a room based on a position in a 2D room grid and a prefab
	void CreateRoom(Tilemap roomPrefab, Vector3Int roomOffset)
	{
		wallLocations.Clear();
		TileBase[,] readMap = ReadMap(roomPrefab, 50, 50);

		// Go through all of the stored tiles and place corresponding tiles onto the map
		Vector3Int index = new Vector3Int(0, 0, 0);
		for (int y = 0; y < readMap.GetLength(1); y++)
		{
			for (int x = 0; x < readMap.GetLength(0); x++)
			{
				index.x = x;
				index.y = y;
				index += roomOffset;

				// Ground tile
				if (readMap[x, y] == editorGroundTile)
				{
					groundTileMap.SetTile(index, groundTiles[Random.Range(0, groundTiles.Count)]);
				}
				// Wall tile
				else if (readMap[x, y] == editorWallTile)
				{
					// Store the wall tile locations to add once the entire ground is added
					// Keep the locations relative to the room
					wallLocations.Add(index - roomOffset);
				}

			}
		}

		// Fill in the wall tiles based on ground tiles
		for (int i = 0; i < wallLocations.Count; i++)
		{
			// Update the tile
			currentWall.topLeft = GetTileState(readMap[wallLocations[i].x - 1, wallLocations[i].y + 1]);
			currentWall.top = GetTileState(readMap[wallLocations[i].x, wallLocations[i].y + 1]);
			currentWall.topRight = GetTileState(readMap[wallLocations[i].x + 1, wallLocations[i].y + 1]);
			currentWall.left = GetTileState(readMap[wallLocations[i].x - 1, wallLocations[i].y]);
			currentWall.currentTile = GetTileState(readMap[wallLocations[i].x, wallLocations[i].y]);
			currentWall.right = GetTileState(readMap[wallLocations[i].x + 1, wallLocations[i].y]);
			currentWall.bottomLeft = GetTileState(readMap[wallLocations[i].x - 1, wallLocations[i].y - 1]);
			currentWall.bottom = GetTileState(readMap[wallLocations[i].x, wallLocations[i].y - 1]);
			currentWall.bottomRight = GetTileState(readMap[wallLocations[i].x + 1, wallLocations[i].y - 1]);

			// Normal wall
			if (currentWall.bottom == TileState.ground &&
				currentWall.left != TileState.empty &&
				currentWall.right != TileState.empty)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, centerWallTiles[Random.Range(0, centerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition + roomOffset, topTopWallTiles[Random.Range(0, topTopWallTiles.Count)]);
				}
			}

			// Bottom
			else if (currentWall.top == TileState.ground &&
					 currentWall.left == TileState.wall &&
					 currentWall.right == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, bottomWallTiles[Random.Range(0, bottomWallTiles.Count)]);
			}

			// Bottom left
			else if (currentWall.top != TileState.empty &&
					 currentWall.left == TileState.wall &&
					 currentWall.right != TileState.wall)
			{
				// Check whether to use a corner wall tile
				if (currentWall.bottom != TileState.wall)
				{
					wallTileMap.SetTile(wallLocations[i] + roomOffset, bottomLeftWallTiles[Random.Range(0, bottomLeftWallTiles.Count)]);
				}
				else
				{
					wallTileMap.SetTile(wallLocations[i] + roomOffset, bottomLeftCornerWallTiles[Random.Range(0, bottomLeftCornerWallTiles.Count)]);
				}
			}

			// Bottom right
			else if (currentWall.top != TileState.empty &&
					 currentWall.right == TileState.wall &&
					 currentWall.left != TileState.wall)
			{
				// Check whether to use a corner wall tile
				if (currentWall.bottom != TileState.wall)
				{
					wallTileMap.SetTile(wallLocations[i] + roomOffset, bottomRightWallTiles[Random.Range(0, bottomRightWallTiles.Count)]);
				}
				else
				{
					wallTileMap.SetTile(wallLocations[i] + roomOffset, bottomRightCornerWallTiles[Random.Range(0, bottomRightCornerWallTiles.Count)]);
				}
			}

			// Left
			else if (currentWall.right != TileState.empty &&
					 currentWall.left != TileState.wall &&
					 currentWall.bottom == TileState.wall &&
					 currentWall.bottomLeft != TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, leftWallTiles[Random.Range(0, leftWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition + roomOffset, topLeftWallTiles[Random.Range(0, topLeftWallTiles.Count)]);
				}
			}

			// Right
			else if (currentWall.left != TileState.empty &&
					 currentWall.right != TileState.wall &&
					 currentWall.bottom == TileState.wall &&
					 currentWall.bottomRight != TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, rightWallTiles[Random.Range(0, rightWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition + roomOffset, topRightWallTiles[Random.Range(0, topRightWallTiles.Count)]);
				}
			}

			// Top right corner
			else if (currentWall.bottom == TileState.wall &&
					 currentWall.bottomLeft == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, topRightCornerWallTiles[Random.Range(0, topRightCornerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition + roomOffset, topLeftWallTiles[Random.Range(0, topLeftWallTiles.Count)]);
				}
			}

			// Top left corner
			else if (currentWall.bottom == TileState.wall &&
					 currentWall.bottomRight == TileState.wall)
			{
				wallTileMap.SetTile(wallLocations[i] + roomOffset, topLeftCornerWallTiles[Random.Range(0, topLeftCornerWallTiles.Count)]);

				// Check if to add a top wall
				if (currentWall.top != TileState.wall)
				{
					Vector3Int topWallPosition = new Vector3Int(wallLocations[i].x, wallLocations[i].y + 1);
					wallTileMap.SetTile(topWallPosition + roomOffset, topRightWallTiles[Random.Range(0, topRightWallTiles.Count)]);
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
			if (hasRoom[roomPos.x, roomPos.y] && canBeFilled)
			{
				return true;
			}
			else if (!hasRoom[roomPos.x, roomPos.y] && canBeEmpty)
			{
				return true;
			}
		}
		return false;
	}
}
