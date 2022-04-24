using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCoin : Pickup
{
    [SerializeField] int value = 1;
    protected static GameManager gameManager;

    protected void Start()
    {
        if (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        ChangeColours(colours[0], colours[1], colours[2]);
    }

    protected override void ActivatePickup()
    {
        gameManager.AddCoins(value);
        value = 0;

        //Overwrite only this function
        entityManager.DeleteEntity(entityID);
    }
}
