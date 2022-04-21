using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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

    public MaterialPropertyBlock material;
    protected SpriteRenderer renderer;
    
    public bool colourChecked = false; // Was the colour checked by the reflection

    //HealthComponent healthComponent;
    //InteractComponent interactComponent;

    protected Color multiplyColour;
    [SerializeField] protected Color[] colours = new Color[3];


    const int DAMAGE_COLOURS = 3;
    Timer flashTimer = new Timer(0.0f);
    protected bool isFlashing = false;
    int flashIndex = 0;

    [SerializeField] protected Color[] multiplyFlashColour = new Color[Globals.COLOURS_PER_SHADER];
    [SerializeField] Color[] damagedColours = new Color[Globals.COLOURS_PER_SHADER * DAMAGE_COLOURS];
    // This multiplied by colours will give you the invincibility duration after being hit
    [SerializeField] float damageFlashInterval = 0.01f;


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

        // Reset the hit colour effect after a delay
        if (isFlashing)
        {
            if (flashTimer.Update(Time.deltaTime))
            {
                ChangeColours(damagedColours[0 * Globals.COLOURS_PER_SHADER + flashIndex],
                              damagedColours[1 * Globals.COLOURS_PER_SHADER + flashIndex],
                              damagedColours[2 * Globals.COLOURS_PER_SHADER + flashIndex]);
                renderer.color = multiplyFlashColour[flashIndex];

                flashIndex++;
                if (flashIndex >= DAMAGE_COLOURS)
                {
                    isFlashing = false;
                    ChangeColours(colours[0], colours[1], colours[2]);
                    renderer.color = multiplyColour;
                }
                flashTimer.Reset(damageFlashInterval);
            }
        }

    }

    public void Create(int id)
    {
        entityID = id;
        movementComponent = GetComponent<MovementComponent>();

        renderer = GetComponent<SpriteRenderer>();

        multiplyColour = renderer.color;

        ChangeColours(colours[0], colours[1], colours[2]);
    }

    public void ChangeColours(Color colour1, Color colour2, Color colour3)
    {
        material = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(material);
        material.SetColor("_PrimaryColor", colour1);
        material.SetColor("_SecondaryColor", colour2);
        material.SetColor("_TertiaryColor", colour3);
        renderer.SetPropertyBlock(material);
        colourChecked = false;
    }

    public MovementComponent GetMovementComponent()
    {
        return movementComponent;
    }


    public virtual bool DestroyEntity()
    {
        if (!entityManager)
        {
            entityManager = FindObjectOfType<EntityManager>();
        }

        if (entityManager)
        {
            var dissolveEffect = gameObject.AddComponent<Dissolve>();

            StartCoroutine(DestroyEntityCoroutine());

            return true;
        }

        return false;
    }

    IEnumerator DestroyEntityCoroutine()
    {
        yield return new WaitForSeconds(1);

            entityManager.DeleteEntity(entityID);
    }


    // Called when the entity is to be removed
    public virtual void OnRemove()
    {
       
    }

    public virtual void Teleport(Vector3 teleportTo)
    {
        movementComponent.Teleport(teleportTo);
    }

    public virtual void TakeDamage(float damage, Vector3 sourcePosition, Vector3 sourceVelocity)
    {       
        // Flashing of the entity indicates the invincibility frame after getting hit
        if (isFlashing) return;
        isFlashing = true;
        flashIndex = 0;
        flashTimer.Reset(damageFlashInterval);

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
