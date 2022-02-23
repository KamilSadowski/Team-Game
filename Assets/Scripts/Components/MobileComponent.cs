using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileComponent : MovementComponent
{
    // Properties
    [SerializeField] protected int DodgingLayerID;
    protected int BaseLayerID;


    [SerializeField] protected float movementSpeed = 1.0f;
    [SerializeField] protected float drag = 0.5f;
    protected SpriteRenderer spriteRenderer;

    //Movement noises
    [SerializeField] protected float DashSpeed = 10.0f;
    [SerializeField] protected float DashDistance = 10.0f;
    [SerializeField] protected readonly float FOOTSTEP_INTERVAL = 1.0f;
    [SerializeField] ContactFilter2D MovementContactData;
    [SerializeField] protected float DashCooldown = 0.25f;
    protected bool HasDashCooldown = true;
    protected SoundManager footStepRandomizer;
    protected float TimeSinceLastInterval = 0f;

    // Movement variables
    public Vector3 velocity;
    protected Vector3 position;
    protected float minVelocity = 0.0001f;
    protected Vector3 currentInput;

    protected bool isFacingRight = true;
    protected List<RaycastHit2D> hits;
    protected bool isDashing = false;

    MortalHealthComponent HealthRef;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        footStepRandomizer = gameObject.GetComponent<SoundManager>();
        hits = new List<RaycastHit2D>();
        HealthRef = GetComponent<MortalHealthComponent>();
        BaseLayerID = gameObject.layer;
    }

    public override void Teleport(Vector3 teleportTo)
    {
        if (rb == null)
        {
            Create();
            if (rb == null)
            {
                return;
            }
        }
        teleportTo.z = Globals.SPRITE_Z;
        rb.position = teleportTo;
    }



    IEnumerator DashOngoing(Vector3 input)
    {
        if (!(Mathf.Abs(input.x) < 0.05 && Mathf.Abs(input.y) < 0.05))
        {

            if (HealthRef == null)
                HealthRef = GetComponent<MortalHealthComponent>();

            HealthRef.SetInvincible(true);
            gameObject.layer = DodgingLayerID;
            float distanceTravelled = 0;
            float stepDistance;


            // Add velocity based on input
            velocity.x += input.x * DashSpeed;
            velocity.y += input.y * DashSpeed;
            velocity = velocity * drag;// Apply drag


            stepDistance = Vector3.Distance(position, position + velocity);

            //Should never occur but minor error checking in case of "IsMoveCollision" or other similar functions using Hit at this time. 
            while (hits.Count > 0)
            {
                yield return new WaitForFixedUpdate();
            }

            if (stepDistance > DashSpeed * 0.25f)
                while (hits.Count == 0 && distanceTravelled < DashDistance)
                {

                    distanceTravelled += stepDistance * Time.deltaTime;

                    rb.Cast(new Vector2(velocity.x, velocity.y), MovementContactData, hits, stepDistance * Time.deltaTime);
                    if (hits.Count == 0)
                    {
                        // Update the movmement
                        rb.transform.Translate(velocity * Time.deltaTime, Space.World);
                        position = rb.transform.position;
                        position.z = Globals.SPRITE_Z;

                        rb.transform.position = position;

                    }
                    else
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            rb.velocity = Vector3.zero;
            hits.Clear();
            velocity = new Vector3(0, 0, 0);



            isDashing = false;
            HealthRef.SetInvincible(false);
            gameObject.layer = BaseLayerID;

            yield return new WaitForSecondsRealtime(DashCooldown);
            HasDashCooldown = true;
        }
        yield return null;
    }

    public override bool isMoveCollision(Vector3 input)
    {
        if (isDashing || Mathf.Abs(input.magnitude) <= minVelocity) 
            return false;
  
        rb.Cast
            (
            new Vector2(input.x * movementSpeed * Time.deltaTime, input.y * movementSpeed * Time.deltaTime), 
            MovementContactData, 
            hits, 
            movementSpeed * Time.deltaTime
            );

        if (hits.Count > 0)
        {
            hits.Clear();
            return true;
        }

        return false;
    }

    public override void Move(Vector3 input, bool isDash = false)
    {
        //Player can only move if not dashing. 
        if (!isDashing)
        {
            if (isDash && HasDashCooldown)
            {
                isDashing = true;
                HasDashCooldown = false;
                StartCoroutine(DashOngoing(input));
                return;
            }

            if (footStepRandomizer != null)
            {
                if (Mathf.Abs(velocity.x) > 0.01 ||
                    Mathf.Abs(velocity.y) > 0.01 ||
                    Mathf.Abs(velocity.z) > 0.01)
                {
                    TimeSinceLastInterval += Time.deltaTime;

                    if (TimeSinceLastInterval > FOOTSTEP_INTERVAL / movementSpeed)
                    {
                        TimeSinceLastInterval = 0;
                        footStepRandomizer.PlaySound(0);
                    }
                }
                else
                    TimeSinceLastInterval = 0;
            }


            // currentInput = input;
            if (rb == null)
            {
                Create();
                if (rb == null)
                {
                    return;
                }
            }

            // Add velocity based on input
            velocity.x += input.x * movementSpeed * Time.deltaTime;
            velocity.y += input.y * movementSpeed * Time.deltaTime;

            // Apply drag
            velocity = velocity * drag;

            // Stop the movement if below a threshold
            if (Mathf.Abs(velocity.magnitude) <= minVelocity)
            {
                velocity = Vector3.zero;
            }

            // Update the movmement
            rb.transform.Translate(velocity, Space.World);
            position = rb.transform.position;
            position.z = Globals.SPRITE_Z;

            rb.transform.position = position;
            rb.velocity = Vector3.zero;

            if (spriteRenderer == null)
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                if (isFacingRight)
                {
                    if (input.x < 0f)
                    {
                        spriteRenderer.flipX = true;
                        isFacingRight = false;
                    }
                }
                else
                {
                    if (input.x > 0f)
                    {
                        spriteRenderer.flipX = false;
                        isFacingRight = true;
                    }

                }
        }
    }
}
