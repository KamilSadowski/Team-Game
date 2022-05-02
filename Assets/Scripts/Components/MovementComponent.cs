using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component is a base class for components allowing movmement but does not actually allow movement
// Use Mobile component instead if you need the object to move
public class MovementComponent : MonoBehaviour
{
    public Rigidbody2D rb { get; protected set; }

    // Start is called before the first frame update
    public virtual void Create()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void Start()
    {
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Move(Vector3 input, bool isDash = false)
    {

    }

    public virtual void Teleport(Vector3 teleportTo)
    {

    }
    public virtual bool IsMoveCollision(Vector3 input)
    {
        return false;
    }

    public virtual List<RaycastHit2D> GetHits(Vector3 input)
    {
        return new List<RaycastHit2D>();
    }
    //Unity will occasionally pass functions without any return. This will be called if the current movement change is important.
    public virtual bool ConfirmedMove(Vector3 input)
    {
        Move(input);
        return true;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public virtual void Redirect(float forceMultiplier, Vector3 direction)
    {

    }

}
