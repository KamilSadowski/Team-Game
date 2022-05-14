using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleWeapon : Weapon
{
    protected override void CollisionEvent(Collider2D collision)
    {
        base.CollisionEvent(collision);
        DestroyEntity();
    }
}
