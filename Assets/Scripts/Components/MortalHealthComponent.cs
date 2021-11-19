using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalHealthComponent : BaseHealthComponent
{
    [SerializeField] float maxHealth = 100;
    protected float currentHealth;
    protected UI_ChargingBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        int i = 0;

        healthBar = GetComponentInChildren<UI_ChargingBar>();

    }
    public override float getHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    // Update is called once per frame
    public override bool TakeDamage(float damage)
    {
        //Remove chance of healing via damage
        currentHealth -= Mathf.Abs(damage);
        Debug.Log(this.gameObject.name + "; Damage recieved: " + currentHealth + "/" + maxHealth);
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
