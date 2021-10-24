using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Corridor[] corridors;

    // Start is called before the first frame update
    void Awake()
    {
        corridors = GetComponentsInChildren<Corridor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateCorridor(Globals.Direction direction, Globals.Grid2D linkedRoom)
    {
        for (int i = 0; i < corridors.Length; i++)
        {
            if (corridors[i].GetDirection() == direction)
            {
                corridors[i].roomLinked = linkedRoom;
                corridors[i].CreateCorridor();
                return;
            }
        }
    }
}
