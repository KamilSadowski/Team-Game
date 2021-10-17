using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    protected Player player;

   // protected bool[] isCharging = { false, false }; //Potentially use this if "Update" is not fast enough and stutters.

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (entityMoveComp != null)
        {
            Vector3 temp = Vector3.zero;

            temp.x += Input.GetAxis("Horizontal");
            temp.y += Input.GetAxis("Vertical");

            entityMoveComp.Move(temp);

        }
        else
        {
            entityMoveComp = this.GetComponent<MovementComponent>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (Input.GetMouseButtonDown(i))
            {
                player.chargeWeapon(i);
            }
            else if (Input.GetMouseButtonUp(i))
            {
                player.ReleaseWeapon(i);
            }
        }
    }
}
