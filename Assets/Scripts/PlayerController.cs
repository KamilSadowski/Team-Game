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
        if (entityMoveComp != null)
        {
            Vector3 temp = Vector3.zero;

            temp.x += Input.GetAxis("Horizontal");
            temp.z += Input.GetAxis("Vertical");

            entityMoveComp.Move(temp);
        }
        else
        {
            entityMoveComp = this.GetComponent<MovementComponent>();
        }
    }
}
