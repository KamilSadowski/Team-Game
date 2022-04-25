using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The particle system will shoot out the bullets and each one will check for collisions with the player
public class BulletHellAttackComponent : AttackComponent
{

    [SerializeField] List<GameObject> attackParent;
    [SerializeField] List<BaseParticleComponent> attackParticle;
    [SerializeField] List<bool> isTargettingPlayer;
    [SerializeField] float attackCooldown;
    float damage = 5.0f;
    System.Func<Vector3> targetPosition;


    float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = PriorityChar_Manager.instance.getPlayerPosition;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        time += Time.deltaTime;
    }


    public override void Attack()
    {
        if (time >= attackCooldown)
        {



                
                if (targetPosition == null)
                {

                    targetPosition = PriorityChar_Manager.instance.getPlayerPosition;
                }
                else   
                    //If everything is valid; spawn a particle system.
                    if (attackParent.Count == attackParticle.Count && attackParticle.Count != 0)
                    {
                        int index = Random.Range(0, attackParticle.Count);
                        if (attackParticle[index] != null && attackParent[index] != null)
                        {
                    time = 0.0f;
                    if (isTargettingPlayer[index])
                                attackParticle[index].StartParticleEffect(attackParent[index].transform, transform, targetPosition);
                            else
                                attackParticle[index].StartParticleEffect(attackParent[index].transform, transform);
                        }
                    }
            
        }

    }
}
