using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity2D
{
    protected float strength = 5.0f;
    protected BaseHealthComponent playerHealth;
    SpriteManager spriteManager;

    [SerializeField] protected List<Sprite> splashes;
    [SerializeField] protected Color splashColour;
    protected float splatterDistance = 0.1f;
    protected float randomThreshold = 0.05f;

    // Start is called before the first frame update
    public float GetStrength()
    {
        return strength;
    }

    public float GetHealthPercentage()
    {
        if (playerHealth)
            return playerHealth.GetHealthPercentage();

        return 1.0f;
    }

    public BaseHealthComponent GetPlayerHealth()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<MortalHealthComponent>();

        if (playerHealth != null)
            return playerHealth;
        return null;
    }

    public override void TakeDamage(float damage, Vector3 sourcePosition, Vector3 sourceVelocity)
    {
        if (isFlashing) return;
        base.TakeDamage(damage, sourcePosition, sourceVelocity);
        if (!spriteManager)
        {
            spriteManager = FindObjectOfType<SpriteManager>();
        }

        float randomOffset = Random.Range(0.0f, randomThreshold);
        Vector3 splatterOffset = (sourcePosition + sourceVelocity - transform.position).normalized * (splatterDistance + randomOffset);
        splatterOffset.z = 0.0f;

        spriteManager.AddSprite(transform.position - splatterOffset, splashes[Random.Range(0, splashes.Count)], splashColour);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEntity();
        if (playerHealth == null)
        {
            playerHealth = healthComponent;
        }
    }
}
