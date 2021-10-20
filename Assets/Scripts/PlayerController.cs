using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    protected Player player;

    protected bool[] isCharging;

    // protected bool[] isCharging = { false, false }; //Potentially use this if "Update" is not fast enough and stutters.

    // Start is called before the first frame update
    void Start()
    {
        isCharging = new bool[] { false, false };
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Generic Unity-provided WASD/Arrow-key based input used as an input for movement. 
        if (entityMoveComp != null)
        {

            Vector3 temp = Vector3.zero;

            temp.x += Input.GetAxis("Horizontal");
            temp.y += Input.GetAxis("Vertical");

            entityMoveComp.Move(temp);

        }
        else
        {
            //The player should always have a movement component. If it doesn't then it should loop until it does, because it is a problem which can't be removed. 
            entityMoveComp = GameObject.FindWithTag("Player").GetComponent<MovementComponent>();
        }

        //See. "Update"
        for (int i = 0; i < 2; ++i)
        {
            if (isCharging[i])
            {
                player.chargeWeapon(i);
            }
        }
    }

    private void Update()
    {


        //This will work as a "Toggle" disguised as a "While holding" which will activate when the mouse button is down, as there may potentially be complications with update speed
        //This should also (Very slightly) reduce the required processing time as it simply ignores the IF statement if the bool is not in the correct section.
        for (int i = 0; i < 2; ++i)
        {
            if (!isCharging[i] && Input.GetMouseButtonDown(i))
            {
                isCharging[i] = true;
            }
            else if (isCharging[i] && Input.GetMouseButtonUp(i))
            {
                isCharging[i] = false;
                player.ReleaseWeapon(i);
            }
        }
    }
}
