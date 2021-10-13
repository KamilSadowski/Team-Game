using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Door[] doors;

    // Start is called before the first frame update
    void Awake()
    {
        doors = GetComponentsInChildren<Door>();

        // Disable all doors, the doors will be re-enabled by the map script once all rooms are generated
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDoor(Globals.Direction direction, Globals.Grid2D linkedRoom)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].GetDirection() == direction)
            {
                doors[i].roomLinked = linkedRoom;
                doors[i].gameObject.SetActive(true);
                return;
            }
        }
    }
}
