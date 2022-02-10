using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityChar_Manager : MonoBehaviour
{

    #region Singleton_PriorityChar

    public static PriorityChar_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion
    // Start is called before the first frame update

    private GameObject player;

    public GameObject getPlayer()
    {
        if(player)
        return player;
    
        return null;
    }
    public Vector3 getPlayerPosition()
    {
        if (player)
            return player.transform.position;

        return Vector3.zero;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (!player) { player = GameObject.FindWithTag("Player"); } 
    }
}
