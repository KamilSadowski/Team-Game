using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionsComponent : MonoBehaviour
{
    Entity entity;
    ParticleSystem CollidableSystems;
    List<ParticleCollisionEvent> CollisionEvents;
    ParticleSystem.Particle[] allParticles;
    // Start is called before the first frame update
    void Awake()
    {
        CollisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
   
            CollidableSystems = gameObject.GetComponent<ParticleSystem>();
        // On entity collision
        if (CollidableSystems)
            if (other.TryGetComponent<Entity>(out entity))
            {
         
            EntityCollision(CollidableSystems, other);
             //   StartCoroutine(EntityCollision(CollidableSystems, other));
            }
            else
            {//Map collision

                StartCoroutine(WallCollisions(CollidableSystems, other));
            }
    }

    void EntityCollision(ParticleSystem system, GameObject other)
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

                        // Damages the entity
                        entity.TakeDamage(10, allParticles[i].position, allParticles[i].velocity);
                        system.SetParticles(allParticles);
                        break;
                        }
                    }
               
                // Updates particle system
                }
            }
        
     //   yield return null;
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
