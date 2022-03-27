using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; } = -1;
    protected Vector3 AccelerationEvent;
    protected bool isMobile = true;
    protected EntityManager entityManager;
    protected bool isInvincible = false;

    protected MovementComponent movementComponent;
    protected BaseHealthComponent healthComponent;

    protected MaterialPropertyBlock material;
    protected SpriteRenderer renderer;

    //HealthComponent healthComponent;
    //InteractComponent interactComponent;

    [SerializeField] Color[] colours = new Color[3];

    protected virtual void UpdateEntity()
    {
        if (entityManager)
        {
            if (entityID == -1)
            {
                entityManager.TryCreateEntity(this);
            }
        }
        else
        {
            entityManager = FindObjectOfType<EntityManager>();
        }


    }

    public void Create(int id)
    {
        entityID = id;
        movementComponent = GetComponent<MovementComponent>();

        renderer = GetComponent<SpriteRenderer>();
        material = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(material);
        material.SetColor("_PrimaryColor", colours[0]);
        material.SetColor("_SecondaryColor", colours[1]);
        material.SetColor("_TertiaryColor", colours[2]);

        renderer.SetPropertyBlock(material);
    }

    public MovementComponent GetMovementComponent()
    {
        return movementComponent;
    }


    public bool DestroyEntity()
    {
        if (entityManager)
        {
            entityManager.DeleteEntity(entityID);
            return true;
        }


        entityManager = FindObjectOfType<EntityManager>();
        return false;
    }
    // Called when the entity is to be removed
    public virtual void OnRemove()
    {
       
    }

    public virtual void Teleport(Vector3 teleportTo)
    {
        movementComponent.Teleport(teleportTo);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isInvincible) return;
        if (!healthComponent)
        {
            healthComponent = GetComponent<BaseHealthComponent>();
        }
        if (healthComponent)
        {
            if (healthComponent.TakeDamage(damage))
            {
                DestroyEntity();
            }
        }
    }

    public virtual void Invincible(bool makeInvincible)
    {
        isInvincible = makeInvincible;
    }

    public virtual void ToggleInvincible()
    {
        isInvincible = !isInvincible;
    }
}
