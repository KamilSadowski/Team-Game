using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Controller
{
    protected GameObject playerObject;
    protected BaseHealthComponent playerHealth;
    protected AttackComponent attackComponent;
    protected Character controlledObject;
    protected Animator animator;
    protected bool isWalking = false;
    protected bool isFacingFront = true;
    protected float kitingDistance = 1.0f; // Space to maintain between the enemy and the player


    protected Vector3 positionCalc;
    protected float distance;
    Vector3 direction;
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.

    // Start is called before the first frame update
    void Start()
    {
        //Grab whatever is tagged as "Player" - This should be connected to the base component and thus update automatically. 
        playerObject = PriorityChar_Manager.instance.getPlayer();
        if (playerObject)
        {
            playerHealth = playerObject.GetComponent<Character>().GetPlayerHealth();
        }

        
        BindVariables();
        StartCoroutine(WalkingThread());
    }


    IEnumerator WalkingThread()
    {
        while (true)
        {
            direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Globals.SPRITE_Z);
            GetComponent<Animator>().SetBool("IsWalking", true);
            if (direction.y < 0f) isFacingFront = true;
            else isFacingFront = false;

            yield return new WaitForSecondsRealtime(Random.Range(0.25f,5.0f));
        }
    }
    // Update is called once per frame
    void Update()
    {

        var prevIsWalking = isWalking;
        var prevIsFacingFront = isFacingFront;

        isWalking = true;
        if (entityMoveComp && entityMoveComp.IsMoveCollision(direction))
        {
            direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Globals.SPRITE_Z);

            if (direction.y < 0f) isFacingFront = true;
            else isFacingFront = false;
        }
        else
            entityMoveComp.Move(direction);

        if (!controlledObject)
        {
            controlledObject = (Character)base.controlledObject;
        }

        if (isValidReferences() && playerObject != null && entityMoveComp != null && playerHealth != null)
        {



            //Adjusts the position calculation to instead be a "Step" in the correct direction, which the movement speed should be able to automatically sort itself.
            if (playerHealth != null)
            {
                isWalking = false;
                //if (playerHealth.TakeDamage(cControlledObject.GetStrength() * Time.deltaTime))
                //{
                //    playerObject.GetComponent<Entity>().DestroyEntity();              
                //}
                if (attackComponent)
                {
                    attackComponent.Attack();
                }
                else
                {
                    
                    attackComponent = gameObject.GetComponent<AttackComponent>();
                    if (attackComponent)
                    {
                        attackComponent.Attack();
                    }
                }
            }
            
        }
        else
        {
            //This will repeatedly try to find the movement component if it is missing. 
            playerObject = PriorityChar_Manager.instance.getPlayer();
            if (playerObject)
            {
                entityMoveComp = gameObject.GetComponent<MovementComponent>();
                playerHealth = playerObject.GetComponent<Character>().GetPlayerHealth();
            }
            BindVariables();
        }


            
       
        if (prevIsFacingFront != isFacingFront)
            GetComponent<Animator>().SetBool("Front", isFacingFront);
    }
}
