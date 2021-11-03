using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ProjectileController
{
    public bool isDropped = true;
    //entityMan.DeleteEntity(entityID);
    protected string WeaponName;
    protected bool isThrown = true;
    protected bool isPickup = true;

    //This will always be false until a player picks up the main reference.
    protected bool isHeld = false;

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
        if(!isHeld)
        //This is for when it has just spawned or is placed. 
        if (isDropped)
        {
            playerPickup();
        }
        else
        if (isThrown)
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
        if(!isHeld)
        if (playerCollision.bounds.Intersects(weaponCollision.bounds))
        {
            isDropped = false;
            isThrown = false;
            isHeld = true;


            transform.position = Vector3.zero;
            FindObjectOfType<Player>().PickupWeapon(gameObject);
        }
    }

    public void ThrowWeapon()
    {
        isThrown = true;
    }

    public bool GetIsThrown()
    {
        return isThrown;
    }
    public bool GetIsEquipped()
    {
        return (!isThrown && isHeld) ;
    }

    public string getName()
    {
        if (WeaponName == null || WeaponName.Length == 0)
            return "";

        return WeaponName;
    }
}
