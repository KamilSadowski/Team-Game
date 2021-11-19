using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ProjectileController
{
    const bool IS_WEAPON_LOG_OUTPUT = true;
    protected float counterGetIsEquipped = COUNTER_GET_IS_EQUIPPED_MAX;
    const float COUNTER_GET_IS_EQUIPPED_MAX = 1.0f;

    //entityMan.DeleteEntity(entityID);
    protected int parentID = -1;
    private Entity parentRef;
    private WeaponController parentWeapCont;

    protected bool isProjectile = true; //This will be set to false when picked up and to true when it moves.
    protected bool isPickup = true;
    protected bool isHeld = false;
    [SerializeField] bool isDropped = false;

    protected CapsuleCollider2D playerCollision;
    protected BoxCollider2D weaponCollision;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        BindVariables();
        CollisionSetup();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        BindVariables();
        if (playerCollision == null || weaponCollision == null)
        {
            CollisionSetup();
        }
        else
        if (!isHeld)
        {
            isProjectile = true;
            //This is for when it has just spawned or is placed. 
            if (isDropped)
            {
                PlayerPickup();
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
                    PlayerPickup();
                }
            }
        }
    }

    protected void CollisionSetup()
    {
        //This function is called whenever an asset is not found. Nothing should run while this is "False"
        playerCollision = FindObjectOfType<Player>().GetComponent<CapsuleCollider2D>();
        weaponCollision = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollision, weaponCollision);
    }

    protected void PlayerPickup()
    { //Since the two layers do not interact, it will be checking if the two bounding boxes are overlayed. 
        if (!isHeld)
            if (playerCollision.bounds.Intersects(weaponCollision.bounds))
            {
                isDropped = false;
                transform.position = Vector3.zero;

                if (GetParentRef())
                {
                    if (parentWeapCont == null) return;
                    if (IS_WEAPON_LOG_OUTPUT)
                    {
                       Debug.Log(Time.realtimeSinceStartup + ": " + gameObject.name + " , Parent ID:" + parentWeapCont.parentID + " OR " + parentWeapCont.controlledObject.entityID);
                    }
                    parentWeapCont.isHeld = true;
                    parentWeapCont.isProjectile = false;
                }

                if (!FindObjectOfType<Player>().PickupWeapon(gameObject) && !isHeld)
                {
                    if (IS_WEAPON_LOG_OUTPUT)
                    {
                        Debug.Log(Time.realtimeSinceStartup + ": " + gameObject.name + ", ID:" + controlledObject.entityID + " , Destroyed. Parent ID:" + parentID);
                    }
                    //No where to put entity. Delete it, for now. 
                    controlledObject.DestroyEntity();
                }
                else
                {
                    //Weapon has been picked up and is now a reference for cloning.
                    isProjectile = false;
                    isHeld = true;
                }
            }
    }

    public void ThrowWeapon()
    {
        isHeld = false;
    }

    public void SetParent(int ID)
    {
        parentID = ID;
    }
    public bool IsParent()
    {
        return (parentID == -1);
    }

    //Sets up parent references, returning false if it is the parent. 
    private bool GetParentRef()
    {
        if (parentID != -1)
        {
            if (parentRef != null && parentWeapCont != null)
            {
                return true;
            }
            //Generic safety checks
            parentRef = entityManager.GetEntity(parentID);
            if (parentRef != null)
            {
                if(!entityManager)
                    entityManager = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();

                parentWeapCont = parentRef.GetComponent<WeaponController>();
                if (parentWeapCont != null)
                {
                    if (IS_WEAPON_LOG_OUTPUT)
                    {
                        Debug.Log(Time.realtimeSinceStartup + ": Multiple weapon layers detected.");
                    }
                    //Will recursively repeat this function until the parentID is equal to -1.
                    return true;
                }

            }
            //Weapon is invalid. (Error has occured)
            controlledObject.DestroyEntity();
            return true;
        }
        return false;
    }



    //Bit of recursion but it should be fine as it will only be a single level, unless something has gone wrong.
    public bool GetIsEquipped()
    {
        if (GetParentRef())
        {
            //Will recursively repeat this function until the parentID is equal to -1.
            return parentWeapCont.GetIsEquipped();
        }

        if (IS_WEAPON_LOG_OUTPUT && counterGetIsEquipped > COUNTER_GET_IS_EQUIPPED_MAX)
        {
            counterGetIsEquipped = 0.0f;
            Debug.Log(Time.realtimeSinceStartup + ": " + gameObject.name + " , Weapon is equipped:" + (isHeld && !isProjectile));
        }
        else
        {
            counterGetIsEquipped += Time.deltaTime;
        }


        return (isHeld && !isProjectile);
    }

}
