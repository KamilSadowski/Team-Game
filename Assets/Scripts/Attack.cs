using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Attack : MonoBehaviour
{
    Entity entity;
    ParticleSystem attackParticleSystem;
    List<ParticleCollisionEvent> collisionEvents;
    ParticleSystem.Particle[] allParticles;
    float damage;
    List<ParticleCollisionEvent> events;

    // Start is called before the first frame update
    void Start()
    {
        attackParticleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        events = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Need to check if is attacking before calling this
    public void StartAttack(float attackDamage)
    {
        
        attackParticleSystem.Play();
        damage = attackDamage;
    }

    public bool IsAttacking()
    {
        return attackParticleSystem.IsAlive();
    }


    private void OnParticleCollision(GameObject other)
    {
        // On entity collision
        if (other.TryGetComponent<Entity>(out entity))
        {
            allParticles = new ParticleSystem.Particle[attackParticleSystem.particleCount];
            attackParticleSystem.GetParticles(allParticles);

            ParticlePhysicsExtensions.GetCollisionEvents(attackParticleSystem, other.gameObject, events);

            foreach (ParticleCollisionEvent colEvent in events)
            {
                if (colEvent.intersection != Vector3.zero)
                {
                    for (int i = 0; i < allParticles.Length; i++)
                    {
                        //If the collision was close enough to the particle position, destroy it
                        if (Vector3.Magnitude(allParticles[i].position - colEvent.intersection) < 0.05f)
                        {
                            allParticles[i].remainingLifetime = -1; // Kills the particle
                            entity.TakeDamage(damage); // Damages the entity
                            attackParticleSystem.SetParticles(allParticles); // Updates particle system
                            break;
                        }
                    }
                }
            }
        }

        // On map collision
        else
        {
            allParticles = new ParticleSystem.Particle[attackParticleSystem.particleCount];
            attackParticleSystem.GetParticles(allParticles);

            ParticlePhysicsExtensions.GetCollisionEvents(attackParticleSystem, other.gameObject, events);

            foreach (ParticleCollisionEvent colEvent in events)
            {
                if (colEvent.intersection != Vector3.zero)
                {
                    for (int i = 0; i < allParticles.Length; i++)
                    {
                        //If the collision was close enough to the particle position, destroy it
                        if (Vector3.Magnitude(allParticles[i].position - colEvent.intersection) < 0.05f)
                        {
                            allParticles[i].remainingLifetime = -1; //Kills the particle
                            attackParticleSystem.SetParticles(allParticles); // Updates particle system
                            break;
                        }
                    }
                }
            }
        }

    }
}
