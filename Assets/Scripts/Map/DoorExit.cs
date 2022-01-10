using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorExit : MonoBehaviour
{
    Door door;

    // Start is called before the first frame update
    void Start()
    {
        door = GetComponentInParent<Door>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!door)
            {
                door = GetComponentInParent<Door>();
            }
            door.room.ExitRoom();
        }
    }
}
