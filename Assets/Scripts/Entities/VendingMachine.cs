using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VendingMachine : Prop
{
    private enum EType { PickUp, Weapon };

    [SerializeField] private GameObject[] DroppableItems;
    [SerializeField] int cost = 1;
    [SerializeField] GameObject vendingMachineHole;
    static GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Use()
    {
        if (DroppableItems.Length > 0)
        {
            var em = EntityManager.instance;

            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            if (!em) return;

            if (gameManager.TrySpendCoins(cost))
            {

                var i = em.TryCreateEntity(DroppableItems[Random.Range(0, DroppableItems.Length)], vendingMachineHole.transform.position);
            }
        }
    }
}

