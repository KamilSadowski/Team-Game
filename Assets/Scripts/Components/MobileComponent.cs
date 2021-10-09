using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileComponent : MovementComponent
{
    [SerializeField] float movementSpeed = 1.0f;
    protected Vector3 currentInput;

    // Start is called before the first frame update
    void Start()
    {
        Create();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.transform.Translate(currentInput * movementSpeed * Time.fixedDeltaTime);
    }

    public override void Move(Vector3 input)
    {
        currentInput = input;
    }
}
