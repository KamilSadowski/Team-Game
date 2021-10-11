using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
	enum DoorDirection { north, east, south, west };
	DoorDirection openingDirection;

	// A list of rooms going in one of the 4 directions
	[SerializeField] List<Room>[] roomPrefabs = new List<Room>[4];

	// List of rooms created
	List<Room> roomsCreated;

	int randomRoom;

	void Start()
	{

	}


	void SpawnRoom()
	{
		if (openingDirection == DoorDirection.north)
		{
			randomRoom = Random.Range(0, roomPrefabs[(int)DoorDirection.north].Count);
			Instantiate(roomPrefabs[(int)DoorDirection.north][randomRoom], transform.position, 
				roomPrefabs[(int)DoorDirection.north][randomRoom].transform.rotation);
		}
		else if (openingDirection == DoorDirection.east)
			{
			randomRoom = Random.Range(0, roomPrefabs[(int)DoorDirection.east].Count); 
			Instantiate(roomPrefabs[(int)DoorDirection.east][randomRoom], transform.position, 
				roomPrefabs[(int)DoorDirection.east][randomRoom].transform.rotation);
		}
		else if (openingDirection == DoorDirection.south)
		{
			randomRoom = Random.Range(0, roomPrefabs[(int)DoorDirection.south].Count);
			Instantiate(roomPrefabs[(int)DoorDirection.south][randomRoom], transform.position, 
				roomPrefabs[(int)DoorDirection.south][randomRoom].transform.rotation);
		}
		else if (openingDirection == DoorDirection.west)
		{
			randomRoom = Random.Range(0, roomPrefabs[(int)DoorDirection.west].Count);
			Instantiate(roomPrefabs[(int)DoorDirection.west][randomRoom], transform.position, 
				roomPrefabs[(int)DoorDirection.west][randomRoom].transform.rotation);
		}
	}
}
