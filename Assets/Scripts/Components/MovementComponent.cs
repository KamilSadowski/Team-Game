using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component is a base class for components allowing movmement but does not actually allow movement
// Use Mobile component instead if you need the object to move
public class MovementComponent : MonoBehaviour
{
    protected Rigidbody rb;

    // Start is called before the first frame update
    public virtual void Create()
    {
        rb = FindObjectOfType<Rigidbody>();
    }

    private void Start()
    {
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Move(Vector3 input)
    {

    }
}
