using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionsComponent : MonoBehaviour
{
    Entity entity;
    List<ParticleSystem> CollidableSystems;
    List<ParticleCollisionEvent> CollisionEvents;
    ParticleSystem.Particle[] allParticles;
    // Start is called before the first frame update
    void Awake()
    {
        CollidableSystems = new List<ParticleSystem>();
        CollisionEvents = new List<ParticleCollisionEvent>();
    }

    public void AddParticleSystem(ParticleSystem input)
    {
        CollidableSystems.Add(input);
    }

    private void OnParticleCollision(GameObject other)
    {
        foreach (ParticleSystem system in CollidableSystems)
        {
            // On entity collision
            if (other.TryGetComponent<Entity>(out entity))
            {
                StartCoroutine(EntityCollision(system, other));
            }
            else
            {//Map collision
                StartCoroutine(WallCollisions(system, other));
            }
        }

    }

    IEnumerator EntityCollision(ParticleSystem system, GameObject other)
    {
            allParticles = new ParticleSystem.Particle[system.particleCount];
            system.GetParticles(allParticles);

            ParticlePhysicsExtensions.GetCollisionEvents(system, other.gameObject, CollisionEvents);

            foreach (ParticleCollisionEvent colEvent in CollisionEvents)
            {
                if (colEvent.intersection != Vector3.zero)
                {
                    for (int i = 0; i < allParticles.Length; i++)
                    {
                        //If the collision was close enough to the particle position, destroy it
                        if (Vector3.Magnitude(allParticles[i].position - colEvent.intersection) < 0.05f)
                        {
                            allParticles[i].remainingLifetime = -1; // Kills the particle
                            entity.TakeDamage(10); // Damages the entity
                            system.SetParticles(allParticles); // Updates particle system
                            break;
                        }
                    }
                }
            }
        
        yield return null;
    }

    IEnumerator WallCollisions(ParticleSystem system,GameObject other)
    {

        allParticles = new ParticleSystem.Particle[system.particleCount];
        system.GetParticles(allParticles);

        ParticlePhysicsExtensions.GetCollisionEvents(system, other.gameObject, CollisionEvents);

        foreach (ParticleCollisionEvent colEvent in CollisionEvents)
        {
            if (colEvent.intersection != Vector3.zero)
            {
                for (int i = 0; i < allParticles.Length; i++)
                {
                    //If the collision was close enough to the particle position, destroy it
                    if (Vector3.Magnitude(allParticles[i].position - colEvent.intersection) < 0.05f)
                    {
                        allParticles[i].remainingLifetime = -1; //Kills the particle
                        system.SetParticles(allParticles); // Updates particle system
                        break;
                    }
                }
            }
        }

        yield return null;
    }

}
