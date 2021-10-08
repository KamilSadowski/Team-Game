using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; }
    protected Vector3 AccelerationEvent;
    protected bool isMobile = true;

    //HealthComponent healthComponent;
    //InteractComponent interactComponent;

    public void Create(int id, bool mobile)
    {
        entityID = id;
        isMobile = mobile;
    }

    public void Move(Vector3 input)
    {

    }


}
