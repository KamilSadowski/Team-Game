using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{



    //This is the interface for the button, to start the process of informing the 
    //Controller that a target should be selected.
    public void ButtonActivate()
    {
        
    }


    public bool AttemptActiveAbility()
    {
        return false;
    }


    //This is the ability output and what will be overwritten when new abilities are made.
    protected virtual void ActivateAbility()
    {
     
    }

}
