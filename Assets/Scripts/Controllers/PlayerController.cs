using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




public class PlayerController : Controller
{
    protected Player player;
    protected GameObject playerObject;
    protected UI_ChargingBar healthBarRef;
    public bool[] isCharging;

    // protected bool[] isCharging = { false, false }; //Potentially use this if "Update" is not fast enough and stutters.

    // Start is called before the first frame update
    void Start()
    {
        isCharging = new bool[2];
        isCharging[0] = false;
        isCharging[1] = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            if (healthBarRef == null)
            {
                healthBarRef = GameObject.FindWithTag("HealthBar").GetComponent<UI_ChargingBar>(); 
            }
            else
            {
                healthBarRef.UpdateProgBar(player.GetHealthPercentage());
            }


            //Generic Unity-provided WASD/Arrow-key based input used as an input for movement. 
            if (entityMoveComp != null)
            {

                Vector3 temp = Vector3.zero;

                temp.x += Input.GetAxis("Horizontal");
                temp.y += Input.GetAxis("Vertical");

                entityMoveComp.Move(temp);

                player.GetComponent<Animator>().SetBool("IsWalking", Mathf.Abs(temp.magnitude) > .1f);
                
            }
            else
            {
                //The player should always have a movement component. If it doesn't then it should loop until it does, because it is a problem which can't be removed. 
                entityMoveComp = PriorityChar_Manager.instance.getPlayer().GetComponent<MovementComponent>();
            }
            

            //// TODO: Rotate the player according to mouse position 
            //var mousePos = Input.mousePosition;
            //var mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y, player.transform.position.z));

            //var mouseDotPlayer = Vector2.Dot(mouseWorldPos, player.transform.position);
            
            //if (mouseDotPlayer < 0f)
            //{
            //    player.transform.Rotate(Vector3.up,180f);
            //}

            //See. "Update"
            for (int i = 0; i < 2; ++i)
            {
                if (isCharging[i])
                {
                    player.ChargeWeapon(i);
                }
            }
        }
        else
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }


       
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {

            if (player == null)
            {
                playerObject = PriorityChar_Manager.instance.getPlayer();
                if (playerObject)
                {
                    player = playerObject.GetComponent<Player>();
                }
            }

            if (player)
            {

                if (Input.GetKeyDown(KeyCode.L))
                {
                    player.SpawnWeaponPickup();
                }


                if (Input.GetKeyDown(KeyCode.K))
                {
                    player.SpawnEnemyTarget();
                }


                if (Input.GetKeyDown(KeyCode.I))
                {
                    player.SpawnRandomPickup();
                }
            }

        }


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
