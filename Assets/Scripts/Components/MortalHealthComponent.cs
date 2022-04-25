using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalHealthComponent : BaseHealthComponent
{
    [SerializeField] float maxHealth = 100;
    bool isInvincible = false;

    protected float currentHealth;
    protected UI_ChargingBar healthBar;
    public const bool IS_HEALTH_LOG_OUTPUT = false;
    protected SoundManager soundOutputComponent;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        int i = 0;

        healthBar = GetComponentInChildren<UI_ChargingBar>();

        if (!soundOutputComponent)
        {
            soundOutputComponent = gameObject.GetComponent<SoundManager>();
        }
    }

    public override void applyDifficultyModifier() 
    {
        maxHealth *= Globals.DifficultyModifier;
        currentHealth = maxHealth;
    }

    public override float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }  
    
    public override float GetHealth()
    {
        return currentHealth;
    }

    public void Heal(int input)
    {
        currentHealth += Mathf.Abs(input);

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void SetInvincible(bool isTrue)
    {
        isInvincible = isTrue;
    }
    // Update is called once per frame
    public override bool TakeDamage(float damage)
    {
        if (isInvincible) return false;

        if (soundOutputComponent)
            soundOutputComponent.PlaySound(1);
        else
        {
            soundOutputComponent = gameObject.GetComponent<SoundManager>();

            if (soundOutputComponent)
                soundOutputComponent.PlaySound(1);
        }

        //Remove chance of healing via damage
        currentHealth -= Mathf.Abs(damage);


        if (IS_HEALTH_LOG_OUTPUT)
            Debug.Log(Time.realtimeSinceStartup + ": " + gameObject.name + " , Damage taken:" + damage);


        if (healthBar != null)
        {
            healthBar.UpdateProgBar(currentHealth / maxHealth);
        }

        if (currentHealth <= 0)
        {
            return true;
        }
        return false;
    }
}
