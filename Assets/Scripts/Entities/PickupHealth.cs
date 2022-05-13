using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup
{
    public int healAmount = 5;
    private MortalHealthComponent health;

    private void Start()
    {
        RandomiseColours();
    }

    protected override void ActivatePickup()
    {
        health = PriorityChar_Manager.instance.getPlayer().GetComponent<MortalHealthComponent>();


        if (health)
            health.Heal(healAmount);
        //Overwrite only this function
        base.ActivatePickup();
    }
}
