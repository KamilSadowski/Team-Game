using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColours : MonoBehaviour
{


    public MaterialPropertyBlock material;
    protected SpriteRenderer renderer;

    protected Color multiplyColour;
    [SerializeField] public Color[] defaultColours = new Color[3];
    protected Color[] colours = new Color[3];

    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();

        multiplyColour = renderer.color;

        material = new MaterialPropertyBlock();

        // Check if to use default chef colours
        string c = PlayerPrefs.GetString(Globals.PLAYER_COLOUR_SAVE);
        if (c is { Length: > 0 })
        {
            PlayerColourSave s = JsonUtility.FromJson<PlayerColourSave>(c);
            if (s != null)
            {
                colours = s.Colours;
                UpdateColour(colours);
            }
        }
        else
        {
            DefaultColours();
        }
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

    public void DefaultColours()
    {
        UpdateColour(defaultColours);
    }


    public void UpdateColour(Color[] newColours)
    {
        colours = newColours;
        ChangeColour(newColours);
    }

    public void ChangeColour(Color[] newColours)
    {

        renderer.GetPropertyBlock(material);
        material.SetColor("_PrimaryColor", colours[0]);
        material.SetColor("_SecondaryColor", colours[1]);
        material.SetColor("_TertiaryColor", colours[2]);
        renderer.SetPropertyBlock(material);

    }
}
