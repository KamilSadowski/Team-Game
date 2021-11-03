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

    protected const float MAX_FORCE_MOD = 3; //Discuss this in detail later- What factor limits the throwing power? 

    protected float strength = 2.5f;
    protected float[] WeaponCharge = {0,0};
    protected Vector3 SpawnPosition;

    Crosshair crosshair;

    //Direct reference to the progress bars held within the UI
    UI_ChargingBar[] weaponCharge_UI = new UI_ChargingBar[WEAPON_COUNT];

    // Start is called before the first frame update
    void Start()
    {
        crosshair = FindObjectOfType<Crosshair>();
        entitySpawner = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        equipment = new PWeapon[WEAPON_COUNT];

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
            entitySpawner.GetEntity(
                entitySpawner.TryCreateListedProjectile(1, SpawnPosition, (transform.position - crosshair.transform.position).normalized, WeaponCharge[index]))
                .GetComponent<ProjectileController>().SetThrowing(WeaponCharge[index], crosshair.GetPosition());


            WeaponCharge[index] = 0.01f;

            //Directly update the progress bar to avoid the usage of "Update" - Only updating it when a change is made.
            if (weaponCharge_UI[index] != null)
                weaponCharge_UI[index].updateProgBar(0);
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
                weaponCharge_UI[index].updateProgBar(WeaponCharge[index] / (MAX_FORCE_MOD * strength));
        }
    }

    public void PickupWeapon(GameObject weapon)
    {
        //Mostly safety checks to see if anything has not been set and if there is no weapon held, at the moment in time. 
        if (weapon != null)
            for (int i = 0; i < WEAPON_COUNT; ++i)
            {
                //This cannot use "IsWeaponAvailable" as it checks if the weapon is out of hand, rather than in hand. 
                //If "IsHeldWeapon" then it should not interact with the null class.
                if (equipment[i].weaponController == null || equipment[i].weaponController.GetIsEquipped())
                {
                    WeaponController inputWeapon = weapon.GetComponent<WeaponController>();

                    //Make sure there isn't an accidental duplicate. "This shouldn't be possible." (Programmer, 2021)
                    if (inputWeapon == equipment[0].weaponController || inputWeapon == equipment[1].weaponController)
                    {
                        break;
                    }
                    if (inputWeapon != null)
                    {
                        equipment[i].gameObject = weapon;
                        equipment[i].weaponController = inputWeapon;
                        break;
                    }
                }
            }
    }


    //If the equipment slot is empty, or the weapon has been "Thrown" it will be unavailable.
    private bool IsWeaponAvailable(int index)
    {
        return (equipment[index].gameObject != null && equipment[index].weaponController != null && equipment[index].weaponController.GetIsEquipped());
    }
}
