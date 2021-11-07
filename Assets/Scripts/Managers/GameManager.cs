using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Player player;
    FollowingCamera camera;

    // Start is called before the first frame update
    void Start()
    {
        // Game manager cannot be destroyed
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (camera != null)
        {
            if (camera.targetToFollow == null)
            {
                if (player != null)
                {
                    camera.SetTarget(player);
                }
                else
                {
                    player = FindObjectOfType<Player>();
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
        else
        {
            player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.transform.position = teleportTo;
            }
        }
    }

    public void EnterScene(Globals.Scenes scene)
    {
        SceneManager.LoadScene(Globals.SceneNames[(int)scene]);
    }

}
