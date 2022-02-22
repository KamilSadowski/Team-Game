using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileComponent : MovementComponent
{
    // Properties
    [SerializeField] protected float movementSpeed = 1.0f;
    [SerializeField] protected float drag = 0.5f;
    protected SpriteRenderer spriteRenderer;

    //Movement noises
    [SerializeField] protected float DashModifier = 10.0f;
    [SerializeField] protected readonly float FOOTSTEP_INTERVAL = 1.0f;
    protected SoundManager footStepRandomizer;
    protected float timeSinceLastInterval = 0f;

    // Movement variables
    public Vector3 velocity;
    protected Vector3 position;
    protected float minVelocity = 0.0001f;
    protected Vector3 currentInput;

    protected bool isFacingRight = true;
    protected List<RaycastHit2D> hits;

    MortalHealthComponent HealthRef;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        footStepRandomizer = gameObject.GetComponent<SoundManager>();
        hits = new List<RaycastHit2D>();
        HealthRef = GetComponent<MortalHealthComponent>();
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

    public void dash()
    {
        if (rb == null)
        {
            return;
        }
        velocity = velocity * DashModifier;

        if (HealthRef == null)
            HealthRef = GetComponent<MortalHealthComponent>();

        HealthRef.SetInvincible(true);

    }

    public override void Move(Vector3 input)
    {
        rb.Cast(new Vector2(velocity.x, velocity.y), hits, Vector3.Distance(position, position + velocity));
        if (hits.Count > 0)
        {
            hits.Clear();
            velocity = new Vector3(0, 0, 0);

            if(HealthRef == null)
                HealthRef = GetComponent<MortalHealthComponent>();

            HealthRef.SetInvincible(false);

            return;
        }


        if (footStepRandomizer != null)
        {
            if (Mathf.Abs(velocity.x) > 0.01 ||
                Mathf.Abs(velocity.y) > 0.01 ||
                Mathf.Abs(velocity.z) > 0.01)
            {
                timeSinceLastInterval += Time.deltaTime;

                if (timeSinceLastInterval > FOOTSTEP_INTERVAL / movementSpeed)
                {
                    timeSinceLastInterval = 0;
                    footStepRandomizer.PlaySound(0);
                }
            }
            else
                timeSinceLastInterval = 0;
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
