using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected Crosshair crosshair;
    protected MovementComponent entityMoveComp;
    protected Entity controlledObject;
    protected Vector3 input;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected bool isValidReferences()
    {
        return (controlledObject != null && entityMoveComp != null);
    }
    // Update is called once per frame
    protected void BindVariables()
    {
        if (crosshair)
            PriorityChar_Manager.instance.getCrosshair();
        if (entityMoveComp == null)
        {
            entityMoveComp = gameObject.GetComponent<MovementComponent>();
        }
        if (controlledObject == null)
        {
            controlledObject = gameObject.GetComponent<Entity>();
        }

    }
}
