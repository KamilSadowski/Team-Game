using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
	GameManager gameManager;

    // A list of room prefabs that can be used
    [SerializeField] List<Room> roomPrefabs = new List<Room>();

    // List of rooms created on a 2D grid
    bool[,] hasRoom = new bool[Globals.MAP_GRID_SIZE, Globals.MAP_GRID_SIZE];

    // Room positions on a 2D grid
    Globals.Grid2D currentRoomPosition = new Globals.Grid2D();
    Globals.Grid2D tmpRoomPosition     = new Globals.Grid2D();

    Globals.Direction       newDirection;
    List<Globals.Direction> directionsAvailable = new List<Globals.Direction>();

    List<Room>           roomsCreated      = new List<Room>();
    List<Globals.Grid2D> roomGridPositions = new List<Globals.Grid2D>();

    // Room creation variables
    Vector3 roomPosition = Vector3.zero;
    Vector3 roomRotation = new Vector3(0.0f, 0.0f, 90.0f);

    bool reloadMap = true;

	private void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update()
	{
		while (reloadMap)
		{
			reloadMap = !CreateMap();
			if (!reloadMap)
			{
				Debug.Log(hasRoom);
			}
		}
	}


	// Returns true if finished creating a map
	bool CreateMap()
	{
		// Create the first room
		if (roomsCreated.Count == 0)
		{
			// Starting room should be in the middle of the map rather than in a corner
			currentRoomPosition.x = Globals.MAP_GRID_SIZE / 2;
			currentRoomPosition.y = Globals.MAP_GRID_SIZE / 2;
			CreateRoom(roomPrefabs[0], currentRoomPosition);
			return false;
		}		
		// Create a random room near one of the rooms that were already placed
		else if (roomsCreated.Count < Globals.MAX_ROOM_NO - 1)
		{
			currentRoomPosition = roomGridPositions[Random.Range(0, roomsCreated.Count)];

			UpdateDirectionsAvailable(currentRoomPosition);
			if (RandomiseAndUpdatePosition(ref currentRoomPosition) && CheckRoomSpace(currentRoomPosition))
			{
				CreateRoom(roomPrefabs[0], currentRoomPosition);
			}

			return false;
		}
		// Go through each room and give them corridors
		else
		{
			for (int room = 0; room < roomsCreated.Count; room++)
			{
				// Check which doors lead to a room and enable them
				UpdateDirectionsAvailable(roomGridPositions[room], false, true);
				for (int door = 0; door < directionsAvailable.Count; door++)
				{
					//roomsCreated[room].CreateCorridor(directionsAvailable[door], 
					//							  PositionInDirection(roomGridPositions[room], 
					//							  directionsAvailable[door]));
				}
			}
		}

		gameManager.TeleportPlayer(roomsCreated[0].transform.position);

		return true;
	}

	// Creates a room based on a position in a 2D room grid
	Room CreateRoom(Room roomPrefab, Globals.Grid2D roomPos)
	{
		roomPosition.x = Globals.ROOM_SIZE * roomPos.x;
		roomPosition.y = Globals.ROOM_SIZE * roomPos.y;
		Debug.Log("Creating room at: ");
		Debug.Log(roomPos.x);
		Debug.Log(roomPos.y);
		hasRoom[roomPos.x, roomPos.y] = true;

		roomsCreated.Add(Instantiate<Room>(roomPrefab));
		roomGridPositions.Add(roomPos);
		roomsCreated[roomsCreated.Count - 1].transform.position = roomPosition;
		roomsCreated[roomsCreated.Count - 1].transform.parent = transform;
		roomsCreated[roomsCreated.Count - 1].transform.rotation = Quaternion.Euler(roomRotation);

		return roomsCreated[roomsCreated.Count - 1];
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
