using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{
    protected Vector3 targetPosition;
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.

    float distance;
    Vector3 direction;

    int entityID;
    EntityManager entityMan;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (entityMoveComp != null)
        {
            distance = Vector3.Distance(targetPosition, transform.position);

            //Adjusts the position calculation to instead be a "Step" in the correct direction, which the movement speed should be able to automatically sort itself.
            direction = (targetPosition - transform.position).normalized;

            //DELETE THIS COMMENT LATER - THE DELETE FUNCTION IS BROKEN
            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (Input.GetKey(KeyCode.P))
            {
                entityID = GetComponent<Entity>().entityID;

                entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();

                if(entityMan != null)
                entityMan.DeleteEntity(entityID);
            }
            else
            entityMoveComp.Move(direction);
      
        }
        else
        {
            //This will repeatedly try to find the movement component if it is missing. 
            entityMoveComp = GetComponent<MovementComponent>();
        }
    }
}
