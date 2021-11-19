using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealthComponent : MonoBehaviour
{
    public virtual bool TakeDamage(float damage) {return false;}
    public virtual float getHealthPercentage()   {return 1.0f;}
}
