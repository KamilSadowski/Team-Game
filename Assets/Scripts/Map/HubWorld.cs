using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubWorld : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.GivePlayerEquipment();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
