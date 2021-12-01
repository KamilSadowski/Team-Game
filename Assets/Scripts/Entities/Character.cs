using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity2D
{
    protected float strength = 5.0f;
    protected BaseHealthComponent playerHealth;
    // Start is called before the first frame update
    public float GetStrength()
    {
        return strength;
    }

    public float GetHealthPercentage()
    {
        return playerHealth.GetHealthPercentage();
    }

    public BaseHealthComponent GetPlayerHealth()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<MortalHealthComponent>();

        if (playerHealth != null)
            return playerHealth;
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEntity();
        if(playerHealth == null)
            playerHealth = GetComponent<MortalHealthComponent>();
    }
}
