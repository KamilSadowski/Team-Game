using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity
{
    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, Globals.SPRITE_Z);
    }

    [SerializeField] protected Vector3 positionOffset;

    public Vector3 GetOffset() { return positionOffset; }
}
