using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Entity
{
    protected WeaponController controller;
    protected GameObject weaponObj;
    protected int Weapon_Parent_ID = -1;
    protected CapsuleCollider2D playerCollision;
    protected BoxCollider2D weaponCollision;

    //Visual representation of weapon. Allows for custom movements to be applied.
    [SerializeField] GameObject VisualRef;

    //Weapon stats for calculating damage.
    [SerializeField] float weapon_sharpness = 0.25f;
    [SerializeField] float weapon_damage = 10;
    [SerializeField] string[] bounceLayers;
    [SerializeField] float bounceStrength = 0.4f;
    const float MIN_DISTANCE_TRAVELLED = 2.0f;

    // Decide if projectile is in the air based on speed
    const float MIN_AIR_SPEED = .45f; // Speed for the projectile to hit the ground, should be higher than on ground
    const float MIN_GROUND_SPEED = 0.3f; // Speed for the projectile to fully stop, should be less than in air

    // Drag of the projectile based on the state/speed
    const float IN_AIR_DRAG = 0.4f;
    const float ON_GROUND_DRAG = 0.9999f;
    //Store the players transform. If the enemies targeted more than one enemy then this might be an issue but assuming
    //That only the player is a viable target, this can be used to calculate if they're within range and where they are, in comparison.

    protected Vector3 nDirection;
    protected Vector3 oldPos;
    protected States currentState = States.Dropped;

    protected bool hasBounced = false;
    protected Vector3 previousDirection;

    [SerializeField] const float MAX_LIFESPAN = 5.0f;
    protected float currentLifespawn = MAX_LIFESPAN;
    protected float currentProjectileLifespawn = 0;
    protected enum States
    {
        Dropped,
        Thrown,
        inactive
    }


    // Start is called before the first frame update
    void Start()
    {
        oldPos = gameObject.transform.position;
        controller = GetComponent<WeaponController>();

        CollisionSetup();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {


        UpdateEntity();
        if (playerCollision == null || weaponCollision == null)
        {
            CollisionSetup();
        }
        else
            switch (currentState)
            {
                case States.Dropped:
                    if (currentLifespawn <= 0)
                    {
                        DestroyEntity();

                    }
                    nDirection = Vector3.zero;
                    DroppedStateUpdate();
                    break;
                case States.Thrown:

                    ProjectileUpdate();
                    break;

                //Once the state is inactive that means it is being used as a reference for other weapons. What this means is that every weapon that is created as a copy needs to know
                //If it's a copy and so making this id accessible is a must
                case States.inactive:

                    Weapon_Parent_ID = entityID;
                    break;
                default:
                    //Safety check
                    break;
            }
    }

    //Getters
    public float getSharpness()
    {
        return weapon_sharpness;
    }
    public float getDamage()
    {
        return weapon_damage;
    }
    public float getBounceStr()
    {
        return bounceStrength;
    }


    //else Setters
    public void setSharpness(float input)
    {
        weapon_sharpness = input;
    }
    public void setDamage(float input)
    {
        weapon_damage = input;
    }
    public void setBounceStr(float input)
    {
        bounceStrength = input;
    }


    public void SetThrowing(float inputForce, Vector3 direction)
    {

        nDirection = (direction - transform.position).normalized;
        nDirection *= inputForce;

        oldPos = this.transform.position;
        currentState = States.Thrown;

    }
    public void SetParentID(int ID)
    {
        Weapon_Parent_ID = ID;
    }
    public void SetInactive()
    {
        currentState = States.inactive;
    }

    public void SetDropped()
    {
        currentState = States.Dropped;
    }

    public bool IsActive()
    {

        return !(currentState == States.inactive);

    }


    public bool IsChildOf(int ID)
    {
        return (Weapon_Parent_ID != -1 && ID == Weapon_Parent_ID);
    }


    protected bool MoveWithMin(float min)
    {
        return ((Mathf.Abs(nDirection.x) > min || Mathf.Abs(nDirection.y) > min));
    }


    protected void UpdateDirection()
    {
        for (int i = 0; i < 3; ++i)
            if (Mathf.Abs(transform.position[i]) <=
                Mathf.Abs(oldPos[i]) + .0005f && Mathf.Abs(transform.position[i]) >=
                Mathf.Abs(oldPos[i]) - .0005f)
            {
                nDirection[i] = 0;
            }

        // Drag depends on whether the projectile is still in the air (depends on current speed of the projecitle)
        if (nDirection.magnitude > MIN_AIR_SPEED)
        {
            if (!movementComponent.IsMoveCollision(nDirection*0.75f))
                nDirection -= (transform.position - oldPos) * IN_AIR_DRAG;
        }
        else
        {
            SetDropped();
            if (nDirection.magnitude > MIN_GROUND_SPEED)
            {
                nDirection -= (transform.position - oldPos) * ON_GROUND_DRAG;
            }
            // Manual stop of projectiles to prevent slow sliding and issues with picking them up
            else
            {

                nDirection = Vector3.zero;
            }
        }
        oldPos = transform.position;

    }

    protected void DroppedStateUpdate()
    {
        currentLifespawn -= Time.deltaTime;

        if (playerCollision.bounds.Intersects(weaponCollision.bounds))
        {
            SetInactive(); //Player should deal with it. If it's picked up then this is jsut a safety net. 
            controller.PlayerPickup(Weapon_Parent_ID);
        }
    }

    protected void CheckBounce(Collision2D collision)
    {
        //Checks for wall collisions using the raycast as it's the safest method of doing so.
        if (!hasBounced)
            previousDirection = nDirection;
#if (RAYCASTBOUNCE)
        //New nDirection would be calculated here.
        nDirection = movementComponent.ReflectCollisionDirection(nDirection);
        if (!hasBounced && nDirection != previousDirection)
            hasBounced = true;

        //Props and enemy collisions. 
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Prop"))//.collider.bounds.Intersects(weaponCollision.bounds))
        {
            hasBounced = true;
            nDirection = new Vector3((nDirection.x + collision.collider.bounds.center.x), (nDirection.y + collision.collider.bounds.center.y), nDirection.z);
            nDirection.x *= -.6f; nDirection.y *= -.6f;
        }
#else
        //Props and enemy collisions. 
        for(int i = 0; i < bounceLayers.Length; ++i)
        if (collision.gameObject.layer == LayerMask.NameToLayer(bounceLayers[i]))//.collider.bounds.Intersects(weaponCollision.bounds))
        {
            hasBounced = true;
            nDirection = -nDirection * bounceStrength;
        }
#endif
    }

    protected void ProjectileUpdate()
    {
        if (movementComponent != null)
        {
            if (VisualRef)
            {
                VisualRef.transform.Rotate(Vector3.forward * currentProjectileLifespawn);
                currentProjectileLifespawn += Time.deltaTime * (nDirection.magnitude * 4.0f);
            }

            float minDistanceTravelled = MIN_DISTANCE_TRAVELLED * Time.deltaTime;

            if (hasBounced && playerCollision.bounds.Intersects(weaponCollision.bounds))// || weaponCollision.Distance(playerCollision).distance < 1.0f)
            {
                SetInactive(); //Player should deal with it. If it's picked up then this is jsut a safety net. 
                controller.PlayerPickup(Weapon_Parent_ID);
                return;
            }

           
            if (MoveWithMin(minDistanceTravelled))
            {
              
                    if (movementComponent.ConfirmedMove(nDirection))
                {

                    UpdateDirection();
                }
            }
            else
            {
                minDistanceTravelled = (MIN_DISTANCE_TRAVELLED * .25f) * Time.deltaTime;
                if (MoveWithMin(minDistanceTravelled))
                    if (movementComponent.ConfirmedMove(nDirection))
                    {

                        UpdateDirection();
                    }
                //if there's no force then why have a projectile?
                return;
            }

        }
        else
        {
            //If the projectile does not have a movement component then it is likely an error and will be removed.
            movementComponent = GetComponent<MovementComponent>();

            if (movementComponent == null)
            {
                this.DestroyEntity();
            }
        }
        return;
    }

    protected void CollisionSetup()
    {
        if (PriorityChar_Manager.instance.getPlayer() == null)
            return;
        //This function is called whenever an asset is not found. Nothing should run while this is "False"
        if (playerCollision && weaponCollision)
            return;

        playerCollision = PriorityChar_Manager.instance.getPlayer().GetComponent<CapsuleCollider2D>();
        weaponCollision = GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollision, weaponCollision);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == States.Thrown)
        {

            CheckBounce(collision);

            float output = weapon_damage * Vector3.Dot(nDirection, nDirection);

            output *= weapon_sharpness * transform.localScale.magnitude;

            Entity tempRef = collision.gameObject.GetComponent<Entity>();

            if (tempRef != null)
                tempRef.TakeDamage(output, transform.position, Vector3.zero);


        }
    }
}
