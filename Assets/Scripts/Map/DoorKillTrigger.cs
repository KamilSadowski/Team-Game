using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKillTrigger : MonoBehaviour
{
    Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    public void OpenDoor()
    {
        if (!collider)
        {
            collider = GetComponent<Collider2D>();
        }
        collider.enabled = false;
    }


    public void CloseDoor()
    {
        if (!collider)
        {
            collider = GetComponent<Collider2D>();
        }
        collider.enabled = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Player player;
        if (collision.gameObject.TryGetComponent<Player>(out player))
        {
            player.ForceKill();
        }
    }
}
