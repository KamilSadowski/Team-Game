using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperController : NpcController
{
    VendingMachine ControlledObject;



    // Start is called before the first frame update
    protected override void ActivateInteraction()
    {
        if (ControlledObject)
            ControlledObject.Use();
    }
    protected override void EndInteraction()
    {

    }

    protected override void InheritedUpdate()
    {
        if (ControlledObject == null)
            ControlledObject = gameObject.GetComponent<VendingMachine>();
        base.InheritedUpdate();
    }
}
