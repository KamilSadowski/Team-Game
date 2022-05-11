using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLevel : MonoBehaviour
{
    [SerializeField] EditableText text;
    float difficulty;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePowerLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (difficulty != Globals.DifficultyModifier)
        {
            UpdatePowerLevel();
        }
    }

    void UpdatePowerLevel()
    {
        text.SetText("Level " + ((int)(Globals.DifficultyModifier * 10.0f - 9.0f)).ToString());
        difficulty = Globals.DifficultyModifier;
    }
}
