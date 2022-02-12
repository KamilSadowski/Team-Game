using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float timerDuration { get; private set; }
    public float timePassed { get; private set; }


    public Timer(float duration)
    {
        timePassed = 0.0f;
        timerDuration = duration;
    }

    // Returns true when finished
    public bool Update(float frameTime)
    {
        timePassed += frameTime;
        return timePassed >= timerDuration;
    }

    public void Reset()
    {
        timePassed = 0.0f;
    }

    public void Reset(float newDuration)
    {
        timerDuration = newDuration;
        Reset();
    }
}
