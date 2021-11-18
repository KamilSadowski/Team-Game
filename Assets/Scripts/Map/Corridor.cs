using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    // Corridor properties
    [SerializeField] protected Globals.Direction direction;
    public Globals.Grid2D roomLinked;

    // Corridor components
    Door[] doors;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject walls;

    // Start is called before the first frame update
    void Awake()
    {
        // Corridor is invisible by default and made visible through CreateCorridor
        ground.SetActive(false);
        walls.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Globals.Direction GetDirection()
    {
        return direction;
    }

    // Makes the corridor visible
    public void CreateCorridor()
    {
        doors = GetComponentsInChildren<Door>();
        ground.SetActive(true);
        walls.SetActive(true);
        foreach (Door door in doors)
        {
            door.CreateDoor();
        }

    }

    public void OpenDoors()
    {
        if (doors != null)
        {
            foreach (Door door in doors)
            {
                if (door != null) door.OpenDoor();
            }
        }
    }

    public void CloseDoors()
    {
        if (doors != null)
        {
            foreach (Door door in doors)
            {
                if (door != null) door.CloseDoor();
            }
        }
    }
}
