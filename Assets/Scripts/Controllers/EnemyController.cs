using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
    protected GameObject playerObject;
    protected BaseHealthComponent playerHealth;
    protected Character cControlledObject;
    protected Animator animator;
    protected bool IsWalking = false;
    protected bool IsFacingFront = true;


    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.

    // Start is called before the first frame update
    void Start()
    {
        //Grab whatever is tagged as "Player" - This should be connected to the base component and thus update automatically. 
        playerObject = GameObject.FindWithTag("Player");
        playerHealth = playerObject.GetComponent<Character>().GetPlayerHealth();
      

        BindVariables();
    }

    // Update is called once per frame
    void Update()
    {

        var prevIsWalking = IsWalking;
        var prevIsFacingFront = IsFacingFront;

        if (!cControlledObject)
        {
            cControlledObject = (Character)controlledObject;
        }

        if (isValidReferences() && playerObject != null && entityMoveComp != null && playerHealth != null)
        {

            //Calculate any required information about the player the AI might need. 
            Vector3 positionCalc = playerObject.transform.position - transform.position;
            float distance = positionCalc.magnitude;

            //Adjusts the position calculation to instead be a "Step" in the correct direction, which the movement speed should be able to automatically sort itself.
            Vector3 direction = positionCalc / distance;

            if (direction.x > 0f) IsFacingFront = true;
            else IsFacingFront = false;

            //DELETE THIS COMMENT LATER
            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (playerHealth != null && distance < .3f)
            {
                if (playerHealth.TakeDamage(cControlledObject.GetStrength() * Time.deltaTime))
                {
                    playerObject.GetComponent<Entity>().DestroyEntity();
                    IsWalking = false;
                }
            }
            else
            {
                IsWalking = true;
                entityMoveComp.Move(direction);
            }

        }
        else
        {
            //This will repeatedly try to find the movement component if it is missing. 
            playerObject = GameObject.FindWithTag("Player");
            entityMoveComp = GetComponent<MovementComponent>();
            playerHealth = playerObject.GetComponent<Character>().GetPlayerHealth();
            BindVariables();
        }

        if (prevIsWalking != IsWalking)
            GetComponent<Animator>().SetBool("IsWalking", IsWalking);
       
        if (prevIsFacingFront != IsFacingFront)
            GetComponent<Animator>().SetBool("Front", IsFacingFront);

    }

    public void DamageEntity(float input)
    {


        if (cControlledObject.GetPlayerHealth().TakeDamage(input))
        {
            controlledObject.GetComponent<Entity>().DestroyEntity();
        }
    }

}
