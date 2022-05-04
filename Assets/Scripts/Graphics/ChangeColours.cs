using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColours : MonoBehaviour
{


    public MaterialPropertyBlock material;
    protected SpriteRenderer renderer;

    protected Color multiplyColour;
    [SerializeField] protected Color[] colours = new Color[3];

    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

        multiplyColour = renderer.color;

        material = new MaterialPropertyBlock();
    }

    private void Start()
    {
        ChangeColour(colours); 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Color[] GetColours()
    {
        return colours;
    }

    public void ChangeColour(Color[] newColours)
    {
            colours = newColours;
            renderer.GetPropertyBlock(material);
            material.SetColor("_PrimaryColor", colours[0]);
            material.SetColor("_SecondaryColor", colours[1]);
            material.SetColor("_TertiaryColor", colours[2]);
            renderer.SetPropertyBlock(material);
        
    }
}
