using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Entity2D
{
    protected EntityManager entitySpawner;

    Collider2D thisCollider;
    Collider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        UpdateEntity();
    }
    protected bool BindVariables()
    {
        UpdateEntity();

        bool output = false;

        if (playerCollider == null) playerCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider2D>();
        if (thisCollider == null) thisCollider = GetComponent<Collider2D>();
        else output = true;

        return output;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BindVariables())
            if (isPlayerCollision())
            {
                activatePickup();
            }
    }

    protected bool isPlayerCollision()
    {
        return playerCollider.bounds.Intersects(thisCollider.bounds);
    }

    protected virtual void activatePickup()
    {
        entitySpawner.DeleteEntity(entityID);
    }
}