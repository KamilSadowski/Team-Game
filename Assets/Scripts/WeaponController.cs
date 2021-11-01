using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ProjectileController
{
    protected bool isPickup = true;

    // Start is called before the first frame update
    void Start() 
    {
        entityID = GetComponent<Entity>().entityID;
        entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isPickup)
        {



        }else
        isPickup = ProjFixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isPickup)
        {
            entityMan.DeleteEntity(entityID);
        }
    }
}
