using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponController : ProjectileController
{
    [SerializeField] float MAX_WEAPON_COOLDOWN = 2.5f;
    [SerializeField] float WEAPON_THROW_COST = 1.0f;
    [SerializeField] float WEAPON_PICKUP_REWARD = 1.0f;
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
    }

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

    public void PlayerPickup()
    { //Since the two layers do not interact, it will be checking if the two bounding boxes are overlayed. 
        if(playerRef)
        if (playerRef.IsSameWeapon(boundPlayerWeapon))
        {
            currentWeaponCooldown -= WEAPON_PICKUP_REWARD;
              
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
            currentWeaponCooldown -= WEAPON_THROW_COST * PlayerThrowCostMod;

            spawnedWeaponEntityReference =
         entityManager.GetEntity(entityManager.TryCreateListedProjectile(1, playerRef.transform.position,
                 (transform.position - crosshair.transform.position).normalized, force));
            spawnedWeaponWeaponReference = spawnedWeaponEntityReference.GetComponent<Weapon>();
            if (spawnedWeaponWeaponReference)
            {
               float rotation =  (float)((System.Math.Atan2(transform.position.x - crosshair.transform.position.x, transform.position.y - crosshair.transform.position.y))/ System.Math.PI) * 180.0f;

                spawnedWeaponEntityReference.gameObject.transform.rotation = new Quaternion(0,0, rotation,1.0f);
                spawnedWeaponWeaponReference.SetThrowing(force, direction);
                spawnedWeaponEntityReference.transform.rotation = new Quaternion(0, 0, Vector3.Dot(crosshair.transform.position, transform.position),1.0f);
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



    //Bit of recursion but it should be fine as it will only be a single level, unless something has gone wrong.
    public bool GetIsEquipped()
    {
        return boundPlayerWeapon != null;
    }

}
