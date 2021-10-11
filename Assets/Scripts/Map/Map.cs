using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	// A list of room prefabs that can be used
	[SerializeField] List<Room> startingRoomPrefabs;
	[SerializeField] List<Room> normalRoomPrefabs = new List<Room>();
	[SerializeField] List<Room> endingRoomPrefabs;

	// List of rooms created
	List<Room> roomsCreated = new List<Room>();
	List<int> roomExitsLeft;

	// Room creation variables
	Room tempRoom;
	Vector3 roomPosition = Vector3.zero;
	Globals.Direction currentOpening;

	int randomRoom;
	bool reloadMap = true;

	void Update()
	{
		if (reloadMap)
		{
			reloadMap = !CreateMap();
		}
	}


	// Returns true if finished creating a map
	bool CreateMap()
	{
		// Create the first room
		if (roomsCreated.Count == 0)
		{
			CreateRoom(startingRoomPrefabs[0], 0, 0); // Starting room should always be at 0,0
			return false;
		}

		/////////////////////////////////////RANDOMISE THE ROOMS///////////////////////////////////////////////////////////////////

		// Create the final room
		else if (roomsCreated.Count == Globals.MAX_ROOM_NO - 1)
		{
			return false;
		}

		// Create a random room
		else if (roomsCreated.Count < Globals.MAX_ROOM_NO - 1)
		{
			return false;
		}

		return true;
	}

	// Creates a room based on a position in a 2D room grid
	Room CreateRoom(Room roomPrefab, int x, int z)
	{
		roomPosition.x = Globals.ROOM_SIZE * x;
		roomPosition.z = Globals.ROOM_SIZE * z;	

		roomsCreated.Add(Instantiate<Room>(roomPrefab));
		roomsCreated[roomsCreated.Count - 1].transform.position = roomPosition;
		roomsCreated[roomsCreated.Count - 1].transform.parent = transform;

		return roomsCreated[roomsCreated.Count - 1];
	}
}
