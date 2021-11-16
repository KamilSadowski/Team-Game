using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealthComponent : MonoBehaviour
{
    public virtual void TakeDamage(float damage) { return; }
}
