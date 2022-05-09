using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // Get colours from stored
        string c = PlayerPrefs.GetString(Globals.PLAYER_COLOUR_SAVE);
        if (c is { Length: > 0 })
        {
            PlayerColourSave s = JsonUtility.FromJson<PlayerColourSave>(c);
            if (s != null)
            {
                var colours = s.Colours;

                var renderer = GetComponent<SpriteRenderer>();

                var material = new MaterialPropertyBlock();

                renderer.GetPropertyBlock(material);
                material.SetColor("_PrimaryColor", colours[0]);
                material.SetColor("_SecondaryColor", colours[1]);
                material.SetColor("_TertiaryColor", colours[2]);
                renderer.SetPropertyBlock(material);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
