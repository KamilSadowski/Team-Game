using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
    protected Transform playerTransform; 
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.


    // Start is called before the first frame update
    void Start()
    {
        //Grab whatever is tagged as "Player" - This should be connected to the base component and thus update automatically. 
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (entityMoveComp != null)
        {
            //Calculate any required information about the player the AI might need. 
            Vector3 positionCalc = playerTransform.position - transform.position;
            float distance = positionCalc.magnitude;

            //Adjusts the position calculation to instead be a "Step" in the correct direction, which the movement speed should be able to automatically sort itself.
            Vector3 direction = positionCalc / distance; 

            //DELETE THIS COMMENT LATER
            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (distance < .5f)
            {
                
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
