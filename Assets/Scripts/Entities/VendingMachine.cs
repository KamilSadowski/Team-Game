using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VendingMachine : Entity
{
    private enum EType { PickUp, Weapon };

    [SerializeField] private EType type;
    [SerializeField] private GameObject[] things;

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
        if (em)
        {
            var i = em.TryCreateEntity(things[Random.Range(0, things.Length)], transform.position);
            var v = em.GetEntity(i);
            if (v)
            {
                v.GetMovementComponent().Move(Vector3.back, true);
            }
        }
    }
}

