using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    // Door states
    bool open; // Is the door open
    bool hasDoor; // Used when a corridor is rendered but invisible, makes the wall behind the door the only thing that is rendered

    // Door components
    SpriteRenderer door;
    Collider2D collider;
    Animator animator;
    TilemapRenderer wall; // Blank used as background for when the doors are invisible

    // Start is called before the first frame update
    void Awake()
    {
        hasDoor = false;

        // Get door components
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        wall = GetComponentInChildren<TilemapRenderer>();

        // Door will be invisible by default
        door = GetComponent<SpriteRenderer>();
        door.enabled = false;
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

    // Makes the door visible and the wall invisible
    public void CreateDoor()
    {
        CheckDoorComponents();
        door.enabled = true;
        wall.enabled = false;
        hasDoor = true;
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (hasDoor)
        {
            animator.SetBool("Open", true);
            open = true;
            collider.enabled = false;
        }
    }

    public void CloseDoor()
    {
        if (hasDoor)
        {
            animator.SetBool("Open", false);
            open = false;
            collider.enabled = true;
        }
    }

    public void CheckDoorComponents()
    {
        if (!door)
        {
            door = GetComponent<SpriteRenderer>();
        }
        if (!wall)
        {
            wall = GetComponentInChildren<TilemapRenderer>();
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
