using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        input.x += Input.GetAxis("Horizontal");
        input.y += Input.GetAxis("Vertical");
        if (controlledEntity != null)
        {
            controlledEntity.GetMovementComponent().Move(input);
        }
        else
        {
            controlledEntity = FindObjectOfType<Player>();
        }
    }
}
