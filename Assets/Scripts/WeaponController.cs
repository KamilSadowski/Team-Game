using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ProjectileController
{

    //entityMan.DeleteEntity(entityID);
    protected string WeaponName;

    protected bool isPickup = true;
    protected bool isHeld = false;
    [SerializeField] bool isDropped = false;

    protected CapsuleCollider2D playerCollision;
    protected BoxCollider2D weaponCollision;

    // Start is called before the first frame update
    void Start()
    {
        entityID = GetComponent<Entity>().entityID;
        entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();

        collisionSetup();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (playerCollision == null || weaponCollision == null)
        {
            collisionSetup();
        }
        else
        if (!isHeld)
            //This is for when it has just spawned or is placed. 
            if (isDropped)
            {
                playerPickup();
            }
            else
            {
                //Once "IsPickup" is false, this function will have "Used up" its force and so it's disabled and the player can pick up the weapon.
                if (isPickup)
                {
                    isPickup = ProjFixedUpdate();
                }
                else
                {
                    isHeld = false;
                    //This could set "IsDropped" to true but it would effectively do the same thing. 
                    playerPickup();
                }
            }
    }

    protected void collisionSetup()
    {
        //This function is called whenever an asset is not found. Nothing should run while this is "False"
        playerCollision = FindObjectOfType<Player>().GetComponent<CapsuleCollider2D>();
        weaponCollision = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollision, weaponCollision);
    }
    protected void playerPickup()
    { //Since the two layers do not interact, it will be checking if the two bounding boxes are overlayed. 
       if (!isHeld) 
       if (playerCollision.bounds.Intersects(weaponCollision.bounds))
       {
           isDropped = false;
           transform.position = Vector3.zero;
           if (!FindObjectOfType<Player>().PickupWeapon(gameObject))
           {
               //No where to put entity. Delete it, for now. 
               entityMan.DeleteEntity(entityID);
           }
           else
           {
               //Weapon has been picked up.
               isHeld = true;
           }
       }
    }

    public void ThrowWeapon()
    {
        isHeld = false;
    }


    public bool GetIsEquipped()
    {
        return (isHeld);
    }

    public string getName()
    {
        if (WeaponName == null || WeaponName.Length == 0)
            return "";

        return WeaponName;
    }
}
