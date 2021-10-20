using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Controller
{
    [SerializeField] public float drag;


 
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.

    protected float force;
    protected Vector3 nDirection;

    protected int entityID;
    protected EntityManager entityMan;

    // Start is called before the first frame update
    void Start() 
    {
        entityID = GetComponent<Entity>().entityID;
        entityMan = GameObject.FindWithTag("GameController").GetComponent<EntityManager>();
    }

    public void SetThrowing(float inputForce, Vector3 direction)
    {
        force = inputForce;
        nDirection = (direction - transform.position).normalized;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (entityMoveComp != null)
        {


            //**Here you will likely need to grab the data from the enemy itself, giving you an attack range and an attack type, potentially changing attack style based on the range, i.e. a false ally could be friendly at a distance.**
            if (Input.GetKey(KeyCode.P))
            {
                if (entityMan != null)
                    entityMan.DeleteEntity(entityID);
            }
            else if (force >= 0.025f)
            {
                //As the mobileComponent does not use Velocity, and the projectiles themselves rely on a single input of force, it should be simulated here without
                //Creating multiple Movement components, which may overcomplicate the process for any future changes.
  
                entityMoveComp.Move(force * nDirection);

                //Drag is designed to work as "0 is no drag. 1 is immobile"
                force *= 1.0f - drag;
            }
            else
            {
                //if there's no force then why have a projectile?
                entityMan.DeleteEntity(entityID);
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
    }
}
