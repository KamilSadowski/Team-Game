using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{

    const float MIN_DISTANCE_TRAVELLED = 7.5f;

    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.



    protected Vector3 nDirection;
    protected Vector3 oldPos;


    protected int entityID;
    protected EntityManager entityMan;

    // Start is called before the first frame update
    void Start()
    {
        entityID = GetComponent<Entity>().entityID;
        entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
        oldPos = transform.position;
        nDirection = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProjFixedUpdate();
    }

    //The above two functions will be the same within every level of inheritance. 



    public void SetThrowing(float inputForce, Vector3 direction)
    {

        nDirection = (direction - transform.position).normalized;
        nDirection *= inputForce;

        oldPos = this.transform.position;
    }
    protected bool MoveWithMin(float min)
    {
        return ((Mathf.Abs(nDirection.x) > min || Mathf.Abs(nDirection.y) > min));
    }

    //So this is stupid, basically, floats aren't precise. 
    protected void updateDirection()
    {
        for (int i = 0; i < 3; ++i)
            if (Mathf.Abs(transform.position[i]) <= Mathf.Abs(oldPos[i]) + .0005f && Mathf.Abs(transform.position[i]) >= Mathf.Abs(oldPos[i]) - .0005f)
                nDirection[i] = 0;


        nDirection -= transform.position - oldPos;
        oldPos = transform.position;
    }
    protected bool ProjFixedUpdate()
    {
        if (entityMoveComp != null)
        {

            float minDistanceTravelled = MIN_DISTANCE_TRAVELLED * Time.deltaTime;
            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (Input.GetKey(KeyCode.P))
            {
                if (entityMan != null)
                    entityMan.DeleteEntity(entityID);
            }
            else if (MoveWithMin(minDistanceTravelled)) 
            {
                if (entityMoveComp.ConfirmedMove(nDirection))
                {
                    updateDirection();
                }
            }
            else
            {
                minDistanceTravelled = (MIN_DISTANCE_TRAVELLED * .25f) * Time.deltaTime;
                if (MoveWithMin(minDistanceTravelled))
                    if (entityMoveComp.ConfirmedMove(nDirection))
                    {
                        updateDirection();
                    }
                //if there's no force then why have a projectile?
                return false;
            }
          
        }
        else
        {
            //If the projectile does not have a movement component then it is likely an error and will be removed.
            entityMoveComp = GetComponent<MovementComponent>();

            if (entityMoveComp == null)
            {
                entityMan.DeleteEntity(entityID);
            }
        }
        return true;
    }
}
