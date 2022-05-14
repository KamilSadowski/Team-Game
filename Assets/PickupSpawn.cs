using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawn : MonoBehaviour
{

    [SerializeField] float ChanceOfSpawn = 0.25f;
    [SerializeField] float ChanceRandomization = 0.2f;
    // Start is called before the first frame update


    //Placed in update to remove chance of missing anything. In theory this is disabled after one usage so it should be efficient.
    void LateUpdate()
    {
        if (Random.Range(0.0f, 1.0f) > ChanceOfSpawn + Random.Range(-ChanceRandomization, ChanceRandomization) || 
            EntityManager.instance.TryCreateRandomListedPickup(transform.position) > 0)
            this.enabled = false;
        
    }
}
