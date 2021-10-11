using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{
    protected Vector3 targetPosition;
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.


    // Start is called before the first frame update
    void Start()
    {
        Plane mayonnaiseOnAnEscalator = new Plane(Vector3.up, 0);
        float dist;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mayonnaiseOnAnEscalator.Raycast(mouseRay, out dist))
        {
            targetPosition = mouseRay.GetPoint(dist);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (entityMoveComp != null)
        {
            //Calculate any required information about the mouse the projectile might need. 
            Vector3 positionCalc = targetPosition - transform.position;
            positionCalc.y = transform.position.y;

            float distance = positionCalc.magnitude;

            //Adjusts the position calculation to instead be a "Step" in the correct direction, which the movement speed should be able to automatically sort itself.
            Vector3 direction = positionCalc / distance; 

            //DELETE THIS COMMENT LATER - THE DELETE FUNCTION IS BROKEN
            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (false && distance > .15f)
            {
                int entityID = this.GetComponent<Entity>().entityID;

                EntityManager entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();

                if(entityMan != null)
                entityMan.DeleteEntity(entityID,true);
            }
            else
            entityMoveComp.Move(direction);
      
        }
        else
        {
            //This will repeatedly try to find the movement component if it is missing. 
            entityMoveComp = this.GetComponent<MovementComponent>();
        }
    }
}
