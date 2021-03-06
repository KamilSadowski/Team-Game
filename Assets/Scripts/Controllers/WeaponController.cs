using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponController : ProjectileController
{
    [SerializeField] float MAX_WEAPON_COOLDOWN = 2.5f;
    [SerializeField] float WEAPON_THROW_COST = 1.0f;
    [SerializeField] float WEAPON_PICKUP_REWARD = 1.0f;
    [SerializeField] float STAT_RANDOM_RANGE = 0.25f;
    protected float PlayerThrowCostMod = 1.0f;
    protected float PlayerCooldownMod = 1.0f;

    protected float currentWeaponCooldown = 0.0f;
    private Weapon boundPlayerWeapon;

    protected CapsuleCollider2D playerCollision;
    protected BoxCollider2D weaponCollision;

    protected Crosshair crosshair;

    protected Entity spawnedWeaponEntityReference;
    protected Weapon spawnedWeaponWeaponReference;

    UI_ChargingBar chargeBarRef;
    TMP_Text textOutput;
    protected Player playerRef;
    // Start is called before the first frame update
    void Start()
    {
        
        entityManager = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        BindVariables();

        crosshair = PriorityChar_Manager.instance.getCrosshair();
        boundPlayerWeapon = this.GetComponent<Weapon>();
        gameObject.layer = LayerMask.NameToLayer("Projectile");

        currentWeaponCooldown = MAX_WEAPON_COOLDOWN / WEAPON_THROW_COST;


    }

    //Getters for weppers
    public float getWeaponCooldown()
    {
        return MAX_WEAPON_COOLDOWN;
    }

    public float getWeaponCost()
    {
        return WEAPON_THROW_COST;
    }

    public float getWeaponReward()
    {
        return WEAPON_PICKUP_REWARD;
    }
    //else Setters
    public void setWeaponCooldown(float input)
    {
        MAX_WEAPON_COOLDOWN = input;
    }

    public void SetWeaponCost(float input)
    {
        WEAPON_THROW_COST = input;
    }

    public void SetWeaponReward(float input)
    {
        WEAPON_PICKUP_REWARD = input;
    }
    //end




    private void Update()
    {
        BindVariables();
    }

    private void BindVariables()
    {
        if (!playerRef)
        {
            GameObject tempRef = PriorityChar_Manager.instance.getPlayer();

            if (tempRef)
                playerRef = PriorityChar_Manager.instance.getPlayer().GetComponent<Player>();
        }
        if (!crosshair)
            crosshair = PriorityChar_Manager.instance.getCrosshair();
        if (MAX_WEAPON_COOLDOWN * PlayerCooldownMod > currentWeaponCooldown)
            currentWeaponCooldown += Time.deltaTime;
        
    }
    public GameObject GetBoundWeapon()
    {
        if(controlledObject)
        return controlledObject.gameObject;
        
        return null;
    }

    public void PlayerPickup(int weaponID)
    { //Since the two layers do not interact, it will be checking if the two bounding boxes are overlayed. 
        if (boundPlayerWeapon != null && boundPlayerWeapon.IsActive()) return;

        if(playerRef)
        if (playerRef.getEquippedWeapon() != null && playerRef.getEquippedWeapon().GetComponent<WeaponController>() != null && playerRef.getEquippedWeapon().entityID == weaponID)
        {
             playerRef.getEquippedWeapon().GetComponent<WeaponController>().currentWeaponCooldown += WEAPON_PICKUP_REWARD;
             boundPlayerWeapon.DestroyEntity();
        }
        else
        //Remove weapon from play, even if it cannot be picked up.
        if (boundPlayerWeapon && playerRef)
        {
            transform.position = new Vector3(1000, 1000, 1000);
           
            if (!playerRef.PickupNewWeapon(this))
            {
                    boundPlayerWeapon.DestroyEntity();
            }
        }

    }

    public void PlayerUpdate()
    {
        if (playerRef)
            boundPlayerWeapon.SetInactive();
        if (chargeBarRef == null)
        {
            chargeBarRef = GameObject.FindWithTag("ChargeBar").GetComponent<UI_ChargingBar>();
            if (chargeBarRef)
                textOutput = chargeBarRef.gameObject.GetComponentInChildren<TMP_Text>();
        }
        else
        {
            int i = 0;
            float percentage = currentWeaponCooldown / (WEAPON_THROW_COST * PlayerThrowCostMod);

            while (percentage > 1.0f)
            {
                percentage -= 1.0f;
                ++i;
            }

            textOutput.text = (i).ToString();
            chargeBarRef.UpdateProgBar(percentage);
        }
    }


    public void ThrowWeapon(float force, Vector3 direction)
    {
        if (WEAPON_THROW_COST * PlayerThrowCostMod < currentWeaponCooldown)
        {
            BindVariables();
            //Remove weapon cost from resource.
            currentWeaponCooldown -= WEAPON_THROW_COST * PlayerThrowCostMod;

            //Create moving entity in crosshair direction.
            spawnedWeaponEntityReference =
         entityManager.GetEntity(entityManager.TryCreateMovingEntity(playerRef.getEquippedWeapon().gameObject, playerRef.transform.position,
                 (transform.position - crosshair.transform.position).normalized, force));

            if(spawnedWeaponEntityReference)
            spawnedWeaponWeaponReference = spawnedWeaponEntityReference.GetComponent<Weapon>();
            if (spawnedWeaponWeaponReference)
            {
               float rotation =  (float)((System.Math.Atan2(transform.position.x - crosshair.transform.position.x, transform.position.y - crosshair.transform.position.y))/ System.Math.PI) * 180.0f;

                spawnedWeaponEntityReference.gameObject.transform.rotation = new Quaternion(0,0, rotation,1.0f);
                spawnedWeaponWeaponReference.SetThrowing(force, direction);
                spawnedWeaponWeaponReference.SetParentID(playerRef.getEquippedWeapon().entityID);
                spawnedWeaponEntityReference.transform.rotation = new Quaternion(0, 0, Vector3.Dot(crosshair.transform.position, transform.position),1.0f);
                spawnedWeaponWeaponReference = null;
            }
            //   spawnedWeaponClassReference
            /*
            if (!crosshair)
                crosshair = PriorityChar_Manager.instance.getCrosshair();

            SpawnPosition = (transform.position * .95f) + (crosshair.GetPosition() * .05f);
           
     
            currentWeaponCooldown += 0;
            boundPlayerWeapon.SetThrowing(force, direction);
            */


        }
    }


    public void DisableWeapon()
    {
        if (!boundPlayerWeapon)
            boundPlayerWeapon = GetComponent<Weapon>(); 
        if (boundPlayerWeapon)
            boundPlayerWeapon.SetInactive();
    }
    //Bit of recursion but it should be fine as it will only be a single level, unless something has gone wrong.
    public bool GetIsEquipped()
    {
        return boundPlayerWeapon != null;
    }

}
