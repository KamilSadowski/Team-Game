using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VendingMachine : Entity
{
    private enum EType { PickUp, Weapon };

    [SerializeField] private EType type;
    [SerializeField] private GameObject[] things;
    [SerializeField] int cost = 1;
    [SerializeField] GameObject vendingMachineHole;
    static GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        isInvincible = true;
        isMobile = false;

        things = type switch
        {
            EType.PickUp => EntityManager.instance.PickupList,
            EType.Weapon => EntityManager.instance.WeaponList,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Use()
    {
        var em = EntityManager.instance;

        if (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (!em) return;

        if (gameManager.TrySpendCoins(cost))
        {
            var i = em.TryCreateEntity(things[Random.Range(0, things.Length)], vendingMachineHole.transform.position);
        }
    }
}

