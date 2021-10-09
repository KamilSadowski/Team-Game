using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; }
    protected Vector3 AccelerationEvent;
    protected bool isMobile = true;

    [SerializeField] protected MovementComponent movementComponent;

    //HealthComponent healthComponent;
    //InteractComponent interactComponent;

    private void Start()
    {

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
