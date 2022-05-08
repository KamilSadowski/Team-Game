using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTop : MonoBehaviour
{
    private Player playerRef;

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponentInParent<Player>();
        var colours = playerRef.GetColours();

        var renderer = GetComponent<SpriteRenderer>();
        
        var material = new MaterialPropertyBlock();
    
        renderer.GetPropertyBlock(material);
        material.SetColor("_PrimaryColor", colours[0]);
        material.SetColor("_SecondaryColor", colours[1]);
        material.SetColor("_TertiaryColor", colours[2]);
        renderer.SetPropertyBlock(material);

    }

    // Update is called once per frame
    void Update()
    {
    }
}
