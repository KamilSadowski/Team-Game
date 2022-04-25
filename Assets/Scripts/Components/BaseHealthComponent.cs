using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealthComponent : MonoBehaviour
{
    public virtual bool TakeDamage(float damage) {return false;}
    public virtual float GetHealthPercentage()   {return 1.0f;}
    public virtual float GetHealth()   {return 0.0f;}
    public virtual void applyDifficultyModifier() { return; }
}
