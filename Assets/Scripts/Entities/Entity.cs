using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; } = -1;
    protected Vector3 AccelerationEvent;
    protected bool isMobile = true;
    EntityManager entityManager;

    [SerializeField] protected MovementComponent movementComponent;

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
    }

    public MovementComponent GetMovementComponent()
    {
        return movementComponent;
    }


}
