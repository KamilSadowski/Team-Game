using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Player player;
    FollowingCamera camera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }


        if (camera != null)
        {
            if (camera.targetToFollow == null)
            {
                if (player != null)
                {
                    camera.SetTarget(player);
                }
            }
        }
        else
        {
            camera = FindObjectOfType<FollowingCamera>();
        }

    }

    public void TeleportPlayer(Vector3 teleportTo)
    {
        if (player != null)
        {
            player.transform.position = teleportTo;
        }
    }
}
