using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity
{
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Globals.SPRITE_Z);

        if (entityID == -1)
        {
            entityManager = FindObjectOfType<EntityManager>();
            entityManager.TryCreateEntity(gameObject, transform.position);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateEntity();
    }

    [SerializeField] protected Vector3 positionOffset;

    public Vector3 GetOffset() { return positionOffset; }
}
