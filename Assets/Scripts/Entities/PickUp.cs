using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Entity2D
{
    protected Collider2D thisCollider;
    protected Collider2D playerCollider;
    AudioSource audioSource;
    [SerializeField] AudioClip pickUpSound;
    bool activated = false;

    protected bool BindVariables()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();

        bool output = false;

        if (playerCollider == null) playerCollider = PriorityChar_Manager.instance.getPlayer().GetComponent<CapsuleCollider2D>();
        if (thisCollider == null) thisCollider = GetComponent<Collider2D>();
        else output = true;

        return output;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateEntity();

        if (BindVariables())
            if (IsPlayerCollision())
            {
                ActivatePickup();
            }
    }

    protected bool IsPlayerCollision()
    {
        return playerCollider.bounds.Intersects(thisCollider.bounds);
    }

    protected virtual void ActivatePickup()
    {
        // Deleting the object has to be delayed so the sound can be played
        if (activated) return;
        activated = true;
        audioSource.PlayOneShot(pickUpSound);
        entityManager.DeleteEntity(entityID, false);
        renderer.enabled = false;
        OnRemove();
        Destroy(gameObject, pickUpSound.length);
    }
}