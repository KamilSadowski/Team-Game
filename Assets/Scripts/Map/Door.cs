using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    // Door components
    SpriteRenderer door;
    Collider2D collider;
    Animator animator;
    public Room room { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        CheckDoorComponents();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Period))
        {
            OpenDoor();
        }
        if (Input.GetKey(KeyCode.Comma))
        {
            CloseDoor();
        }
    }

    public void SetRoom(Room owner)
    {
        room = owner;
    }

    

    // Makes the door visible and the wall invisible
    public void CreateDoor()
    {
        CheckDoorComponents();
        door.enabled = true;
    }

    public void OpenDoor()
    {
        animator.SetBool("Open", true);
        collider.enabled = false;
    }

    public void CloseDoor()
    {
        animator.SetBool("Open", false);
        collider.enabled = true;
    }

    public void CheckDoorComponents()
    {
        if (!door)
        {
            door = GetComponent<SpriteRenderer>();
        }
        if (!collider)
        {
            collider = GetComponentInChildren<Collider2D>();
        }
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }
}
