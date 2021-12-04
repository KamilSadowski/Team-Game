using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : Controller
{
    public const float interactionRadius = .25f;
    GameObject playerRef;
    // Start is called before the first frame update
    void Start()
    {

    }

    protected bool IsPlayerWithinRadius()
    {
        float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));
       


        //Doesn  float collRad = interactionRadius * ((transform.localScale.x + transform.localScale.y));'t have to be accurate. Just a general sphere that scales with the object.
        //If distance is less than the stored radius.
        if (collRad >= Vector3.Distance(transform.position, playerRef.transform.position))
        {
            Debug.Log("Alert");
           
        }
        else
        {

        }
        return true;

    }
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius * ((transform.localScale.x + transform.localScale.y)));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isValidReferences())
        {
            BindVariables();
        }

        if (!playerRef)
        {
            playerRef = PriorityChar_Manager.instance.getPlayer();
        }
        else
        {
            IsPlayerWithinRadius();
        }




    }
}
