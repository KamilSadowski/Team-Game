using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity
{
    [SerializeField] protected Vector3 positionOffset;

    public Vector3 GetOffset() { return positionOffset; }
}
