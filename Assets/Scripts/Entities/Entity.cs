using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; } = -1;
    protected Vector3 AccelerationEvent;
    protected bool isMobile = true;
    EntityManager entityManager;

    protected MovementComponent movementComponent;
    protected BaseHealthComponent healthComponent;
    //HealthComponent healthComponent;
    //InteractComponent interactComponent;

    protected virtual void UpdateEntity()
    {
        if (entityManager)
        {
            if (entityID == -1)
            {
                entityManager.TryCreateEntity(this);
            }
        }
        else
        {
            entityManager = FindObjectOfType<EntityManager>();
        }
    }

    public void Create(int id)
    {
        entityID = id;
        movementComponent = GetComponent<MovementComponent>();
    }

    public MovementComponent GetMovementComponent()
    {
        return movementComponent;
    }


    public bool DestroyEntity()
    {
        if (entityManager)
        {
            entityManager.DeleteEntity(entityID);
            return true;
        }


        entityManager = FindObjectOfType<EntityManager>();
        return false;
    }
    // Called when the entity is to be removed
    public virtual void OnRemove()
    {
       
    }

    public virtual void TakeDamage(float damage)
    {
        if (!healthComponent)
        {
            healthComponent = GetComponent<BaseHealthComponent>();
        }
        if (healthComponent)
        {
            if (healthComponent.TakeDamage(damage))
            {
                DestroyEntity();
            }
        }
    }
}
