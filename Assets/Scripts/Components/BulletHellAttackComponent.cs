using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The particle system will shoot out the bullets and each one will check for collisions with the player
public class BulletHellAttackComponent : AttackComponent
{

    [SerializeField] List<Attack> attackList;
    Attack currentAttack;
    float damage = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        if (currentAttack == null || !currentAttack.IsAttacking())
        {
            currentAttack = attackList[Random.Range(0, attackList.Count - 1)];
            currentAttack.StartAttack(damage);
        }
    }

}
