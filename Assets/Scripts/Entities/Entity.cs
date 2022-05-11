using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int entityID { get; protected set; } = -1;
    public int templateID { get; protected set; } = -1;
    protected Vector3 AccelerationEvent;
    protected EntityManager entityManager;
    protected bool isInvincible = false;

    protected MovementComponent movementComponent;
    protected BaseHealthComponent healthComponent;
    protected ParticleSystem onHitParticles;

    public MaterialPropertyBlock material;
    public SpriteRenderer renderer;
    
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

    public void Create(int id, int templateIndex = -1)
    {
        entityID = id;
        templateID = templateIndex;
        movementComponent = GetComponent<MovementComponent>();

        renderer = GetComponent<SpriteRenderer>();
        if (!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
        multiplyColour = renderer.color;

        ChangeColours(colours[0], colours[1], colours[2]);

        transform.position = new Vector3(transform.position.x, transform.position.y, Globals.SPRITE_Z);
        onHitParticles = GetComponentInChildren<ParticleSystem>();
    }
    public void UpdateColour(Color[] newColours)
    {
        colours = newColours;
        ChangeColours(colours[0], colours[1], colours[2]);
    }

    public Color[] GetColours()
    {
        return colours;
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
            
            transform.rotation.Set(0,0,0,1);

            var dissolveEffect = gameObject.AddComponent<Dissolve>();

            Collider2D collider;

            TryGetComponent<Collider2D>(out collider);

            if (collider)
            {
                collider.enabled = false;
            }

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
        transform.position = teleportTo;
    }

    public virtual void TakeDamage(float damage, Vector3 sourcePosition, Vector3 sourceVelocity)
    {       
        // Flashing of the entity indicates the invincibility frame after getting hit
        if (isFlashing) return;
        onHitParticles.Play();
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
