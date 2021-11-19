using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected MovementComponent entityMoveComp;
    protected Character controlledObject;
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
        if (entityMoveComp == null)
        {
            entityMoveComp = GetComponent<MovementComponent>();
        }
        if (controlledObject == null)
        {
            controlledObject = GetComponent<Character>();
        }

    }
}
