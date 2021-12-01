using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public struct PWeapon
    {
        public GameObject gameObject;
        public WeaponController weaponController;
    }

    const int WEAPON_COUNT = 2;
    public PWeapon[] equipment;

    protected EntityManager entitySpawner;
    [SerializeField] Weapon[] NPCList;

    protected const float MAX_FORCE_MOD = 0.5f; //Discuss this in detail later- What factor limits the throwing power? 


    protected float[] WeaponCharge = { 0, 0 };
    protected Vector3 SpawnPosition;
    WeaponController spawnedWeaponReference;
    Crosshair crosshair;
    GameManager gameManager;

    //Direct reference to the progress bars held within the UI
    UI_ChargingBar[] weaponCharge_UI = new UI_ChargingBar[WEAPON_COUNT];

    // Start is called before the first frame update
    void Start()
    {
        crosshair = FindObjectOfType<Crosshair>();
        entitySpawner = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        if (equipment == null)
        {
            equipment = new PWeapon[WEAPON_COUNT];
        }

        int i = 0;
        foreach (Transform child in GameObject.FindWithTag("ChargeBar").GetComponent<Transform>())
        {
            if (i < WEAPON_COUNT)
            {
                weaponCharge_UI[i] = child.GetComponent<UI_ChargingBar>();
                ++i;
            }
        }
    }

    public void ReleaseWeapon(int index)
    {
        if (IsWeaponAvailable(index))
        {
            //Stops the projectile spawning directly under the entity (Currently 5% crosshair pos effect) //NOTE: LOOK INTO ALTERNATIVES
            SpawnPosition = (transform.position * .95f) + (crosshair.GetPosition() * .05f);

            //In the future the listed projectile (the 1) will instead inherit a projectile from the equipped weapon (First variable in function)
            //Set the force and direction within the projectile class for its own use.
            spawnedWeaponReference =
                entitySpawner.GetEntity(entitySpawner.TryCreateListedProjectile(1, SpawnPosition, (transform.position - crosshair.transform.position).normalized, WeaponCharge[index]))
                .GetComponent<WeaponController>();

            if (spawnedWeaponReference != null)
            {
                spawnedWeaponReference.SetThrowing(WeaponCharge[index], crosshair.GetPosition());
                spawnedWeaponReference.SetParent(equipment[index].gameObject.GetComponent<Entity>().entityID);
                //SetParent

                //This will tell the "Source" weapon that it has spawned a clone, to throw. 
                equipment[index].weaponController.ThrowWeapon();

                WeaponCharge[index] = 0.01f;

                //Directly update the progress bar to avoid the usage of "Update" - Only updating it when a change is made.
                if (weaponCharge_UI[index] != null)
                    weaponCharge_UI[index].UpdateProgBar(0);
            }
        }
    }

    public void ChargeWeapon(int index)
    {
        if (IsWeaponAvailable(index))
        {
            //If the current value is less than the max value (See. If Statement) then increase the stored "Charge" by your strength
            //Delta time is applied to avoid dividing anything by 50 (Fixed update should be done 50 times a second but it can be adjusted, hence the time calculation)
            if (WeaponCharge[index] < strength * MAX_FORCE_MOD) WeaponCharge[index] += strength * Time.deltaTime;

            //Directly update the progress bar to avoid the usage of "Update" - Only updating it when a change is made.
            if (weaponCharge_UI[index] != null)
                weaponCharge_UI[index].UpdateProgBar(WeaponCharge[index] / (MAX_FORCE_MOD * strength));
        }
    }

    public bool PickupWeapon(GameObject weapon)
    {
        if (equipment == null)
        {
            equipment = new PWeapon[WEAPON_COUNT];
        }

        //Mostly safety checks to see if anything has not been set and if there is no weapon held, at the moment in time. 
        if (weapon != null)
            for (int i = 0; i < WEAPON_COUNT; ++i)
            {
                //This cannot use "IsWeaponAvailable" as it checks if the weapon is out of hand, rather than in hand. 
                //If "IsHeldWeapon" then it should not interact with the null class.
                if (equipment[i].weaponController == null || !equipment[i].weaponController.GetIsEquipped()) //
                {
                    WeaponController inputWeapon = weapon.GetComponent<WeaponController>();
                    if (!inputWeapon.IsParent()) //If it is not a "Source" weapon.
                    {
                        break;
                    }

                    //Make sure there isn't an accidental duplicate. "This shouldn't be possible." (Programmer, 2021)
                    if (inputWeapon == equipment[0].weaponController || inputWeapon == equipment[1].weaponController)
                    {
                        return false;
                    }
                    if (inputWeapon != null)
                    {
                        equipment[i].gameObject = weapon;
                        equipment[i].weaponController = inputWeapon;
                        return true;
                    }
                }
            }
        return false;
    }


    //If the equipment slot is empty, or the weapon has been "Thrown" it will be unavailable.
    private bool IsWeaponAvailable(int index)
    {
        if (equipment == null)
        {
            equipment = new PWeapon[WEAPON_COUNT];
        }

        if (equipment[index].gameObject != null)
            if (equipment[index].weaponController.GetIsEquipped())
                return true;

        return false;
    }

    public void SpawnWeaponPickup()
    {
        entitySpawner.TryCreateListedWeapon(1, crosshair.GetPosition());
    }

    public void SpawnWeaponPickupAt(Vector3 position)
    {
        entitySpawner.TryCreateListedWeapon(1, position);
    }

    public void SpawnEnemyTarget()
    {
        entitySpawner.TryCreateListedNPC(0, crosshair.GetPosition());
    }

    public void SpawnRandomPickup()
    {

        entitySpawner.TryCreateRandomListedPickup(crosshair.GetPosition());
    }

    public override void TakeDamage(float damage)
    {
        if (!healthComponent)
        {
            healthComponent = GetComponent<BaseHealthComponent>();
        }
        if (healthComponent)
        {
            if (healthComponent.TakeDamage(damage))
            {
                // If player is killed, he is taken to the hubworld
                if (!gameManager)
                {
                    gameManager = FindObjectOfType<GameManager>();
                }
                if (gameManager)
                {
                    gameManager.EnterScene(Globals.Scenes.HubWorld);
                }

            }
        }
    }
}
