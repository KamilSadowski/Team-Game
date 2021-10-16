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
    // Start is called before the first frame update
    void Start()
    {
        entitySpawner = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReleaseWeapon(int index)
    {
        entitySpawner.TryCreateListedProjectile(1, this.transform.position, Vector3.up, strength);
        WeaponCharge[index] = 0.01f;
    }

    public void chargeWeapon(int index)
    {
        if(WeaponCharge[index] < strength * MAX_FORCE_MOD)
        WeaponCharge[index] += strength;
    }
}
