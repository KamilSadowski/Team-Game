using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    // Global objects
    EntityManager entityManager;

    // Room components
    Corridor[] corridors;

    // Room properties
    int enemiesAlive;
    [SerializeField] int enemyNo = 2;

    // Room state
    bool wasEntered = false;
    bool wasCleared = false;

    // Temporary variables
    NotPlayer currentNPC;
    int currentEntityID;

    // Start is called before the first frame update
    void Awake()
    {
        corridors = GetComponentsInChildren<Corridor>();
        entityManager = FindObjectOfType<EntityManager>();
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
    
    // Close all the doors and spawn enemies
    public void EnterRoom()
    {
        CloseDoors();
        for (int i = 0; i < enemyNo; i++)
        {
            CreateEntity();
        }
        wasEntered = true;
    }

    public void CreateEntity()
    {
        currentEntityID = entityManager.TryCreateListedNPC(0, transform.position);
        if (currentEntityID != -1)
        {
            currentNPC = entityManager.GetEntity(currentEntityID) as NotPlayer;
            currentNPC.AddRoom(this);
            ++enemiesAlive;
        }
    }

    public void OpenDoors()
    {
        for (int corridor = 0; corridor < corridors.Length; corridor++)
        {
            if (corridors[corridor] != null) corridors[corridor].OpenDoors();
        }
    }

    public void CloseDoors()
    {
        for (int corridor = 0; corridor < corridors.Length; corridor++)
        {
            if (corridors[corridor] != null) corridors[corridor].CloseDoors();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !wasEntered)
        {
            EnterRoom();
        }
    }

    public void EntityRemoved()
    {
        --enemiesAlive;
        if (enemiesAlive <= 0)
        {
            OpenDoors();
            wasCleared = true;
        }
    }
}
