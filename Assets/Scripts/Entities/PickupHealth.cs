using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealth : Pickup
{
    public int healAmount = 5;
    private MortalHealthComponent health;

    protected override void activatePickup()
    {
        health = PriorityChar_Manager.instance.getPlayer().GetComponent<MortalHealthComponent>();


        if (health)
            health.heal(healAmount);
        //Overwrite only this function
        entityManager.DeleteEntity(entityID);
    }
}
