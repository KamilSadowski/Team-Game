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

    protected bool isUsingInterface = false;
    private bool isFacingRight = true;

    protected bool isDashing = false;

    // protected bool[] isCharging = { false, false }; //Potentially use this if "Update" is not fast enough and stutters.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BindVariables();
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

                    if (player && !player.isDead)
                    {
                        Vector3 temp = Vector3.zero;

                        temp.x += Input.GetAxis("Horizontal");
                        temp.y += Input.GetAxis("Vertical");


                        entityMoveComp.Move(temp, isDashing);

                        player.GetComponent<Animator>().SetBool("IsWalking", Mathf.Abs(temp.magnitude) > .1f);
                    }

                }
                else
                {
                    //The player should always have a movement component. If it doesn't then it should loop until it does, because it is a problem which can't be removed. 
                    entityMoveComp = PriorityChar_Manager.instance.getPlayer().GetComponent<MovementComponent>();
                }


           

//                player.ChargeWeapon();

            }
        }
        else
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }



    }

    private void Update()
    {


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

        if (player && !player.isDead)
        {

            if (Debug.isDebugBuild)
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

            if (Input.GetMouseButtonUp(0))
                player.ReleaseWeapon();
            if (Input.GetMouseButtonDown(0))
                player.ChargeWeapon();
        }
        if (Input.GetMouseButtonDown(1))
        {
            isDashing = true;
            entityMoveComp.Move(Vector3.zero, isDashing);
        }
        else
            isDashing = false;
        

    }

}
