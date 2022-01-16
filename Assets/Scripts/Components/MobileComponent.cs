using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileComponent : MovementComponent
{
    // Properties
    [SerializeField] protected float movementSpeed = 1.0f;
    [SerializeField] protected float drag = 0.5f;

    // Movement variables
    public Vector3 velocity;
    protected Vector3 position;
    protected float minVelocity = 0.0001f;
    protected Vector3 currentInput;

    protected bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        Create();
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

        rb.position = teleportTo;
    }

    public override void Move(Vector3 input)
    {
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
        position.z = 0.0f;
        rb.transform.position = position;
        rb.velocity = Vector3.zero;


        if (isFacingRight)
        {
            if (input.x < 0f)
            {
                transform.Rotate(Vector3.up, 180f);
                isFacingRight = false;
            }
        }
        else
        {
            if (input.x > 0f)
            {
                transform.Rotate(Vector3.up, 180f);
                isFacingRight = true;
            }

        }

    }
}
