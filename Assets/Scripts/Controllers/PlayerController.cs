using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;




public class PlayerController : Controller
{

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip InventoryNoise;
    [SerializeField] float InventoryVolume;


    protected Player player;
    protected GameObject playerObject;
    protected UI_ChargingBar healthBarRef;
    protected UI inventoryRef;
    public bool[] isCharging;

    protected bool isUsingInterface = false;
    private bool isFacingRight = true;

    protected bool isDashing = false;

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

            //This should give access to the UI manager.
            //if (inventoryRef == null)
            //{
            //    inventoryRef = GameObject.FindWithTag("UI").GetComponent<UI>();
            //}



            if (!isUsingInterface)
            {
                FollowingCamera.instance.CameraUpdata();

                //Generic Unity-provided WASD/Arrow-key based input used as an input for movement. 
                if (entityMoveComp != null)
                {

                    Vector3 temp = Vector3.zero;

                    temp.x += Input.GetAxis("Horizontal");
                    temp.y += Input.GetAxis("Vertical");


                    entityMoveComp.Move(temp, isDashing);

                    player.GetComponent<Animator>().SetBool("IsWalking", Mathf.Abs(temp.magnitude) > .1f);

                }
                else
                {
                    //The player should always have a movement component. If it doesn't then it should loop until it does, because it is a problem which can't be removed. 
                    entityMoveComp = PriorityChar_Manager.instance.getPlayer().GetComponent<MovementComponent>();
                }


                // TODO: Rotate the player according to mouse position 
                var mousePos = Input.mousePosition;
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, player.transform.position.z));
                var mousePlayerV = mouseWorldPos - player.transform.position;
                var mousePlayerVNomalized = Vector3.Normalize(mousePlayerV);



                //See. "Update"
                for (int i = 0; i < 2; ++i)
                {
                    if (isCharging[i])
                    {
                        player.ChargeWeapon(i);
                    }
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
        if (Input.GetKeyDown(KeyCode.Space))
            isDashing = true;
        else if (Input.GetKeyUp(KeyCode.Space))
            isDashing = false;

        if (inventoryRef != null && Input.GetKeyDown(KeyCode.E))
        {
            if (InventoryNoise)
            {
                if (isUsingInterface)
                    audioSource.PlayOneShot(InventoryNoise, InventoryVolume);
                else
                    audioSource.PlayOneShot(InventoryNoise, InventoryVolume * 0.75f);
            }
            isUsingInterface = inventoryRef.ToggleMenu();
        }
        if (player == null)
        {
            playerObject = PriorityChar_Manager.instance.getPlayer();
            if (playerObject)
            {
                player = playerObject.GetComponent<Player>();
            }
        }



        if (Debug.isDebugBuild)
        {



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

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    Portal.TeleportTo(Globals.Scenes.Dungeon);
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    player.ToggleInvincible();
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
