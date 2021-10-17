using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    protected EntityManager entitySpawner;
    [SerializeField] Weapon[] NPCList;

    protected const float MAX_FORCE_MOD = 3; //Discuss this in detail later- What factor limits the throwing power? 

    protected float strength;
    protected float[] WeaponCharge = {0,0};

    Crosshair crosshair;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = FindObjectOfType<Crosshair>();
        entitySpawner = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReleaseWeapon(int index)
    {
        entitySpawner.TryCreateListedProjectile(1, transform.position, (transform.position - crosshair.transform.position).normalized, strength);
        //In the future the '1' will instead inherit a projectile from the equipped weapon (First variable in function)
        //Vector3.up will, in the future, get the mouse position and create a normalized direction.
        WeaponCharge[index] = 0.01f;
    }

    public void chargeWeapon(int index)
    {
        if(WeaponCharge[index] < strength * MAX_FORCE_MOD)
        WeaponCharge[index] += strength;
    }
}
