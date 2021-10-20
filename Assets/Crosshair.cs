using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    Vector3 newPosition;
    public void FixedUpdate()
    {
        newPosition.z = 0.0f;
        transform.position = newPosition;
    }

    public void SetPosition(Vector3 WorldPosition)
    {
        WorldPosition.z = 0.0f;
        newPosition = WorldPosition;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}

