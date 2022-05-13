using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourSliders : MonoBehaviour
{
    [SerializeField] Slider redSlider;
    [SerializeField] Slider greenSlider;
    [SerializeField] Slider blueSlider;
    [SerializeField] ChangeColours changeColours;

    Color[] colour = new Color[3];
    int currentColour = 0;

    // Start is called before the first frame update
    void Start()
    {
        string c = PlayerPrefs.GetString(Globals.PLAYER_COLOUR_SAVE);
        PlayerColourSave s = JsonUtility.FromJson<PlayerColourSave>(c);
        if (c is { Length: > 0 })
        {
            colour[0].r = s.Colours[0].r;
            colour[0].g = s.Colours[1].g;
            colour[0].b = s.Colours[2].b;
            colour[0].a = 1.0f;
        }
        else
        {
            DefaultColour();
        }
    }

    public void RedSlider()
    {
        if (colour[currentColour].r != redSlider.value)
        {
            colour[currentColour].r = redSlider.value;
            changeColours.UpdateColour(colour);
        }

    }

    public void GreenSlider()
    {
        if (colour[currentColour].g != greenSlider.value)
        {
            colour[currentColour].g = greenSlider.value;
            changeColours.UpdateColour(colour);
        }
    }

    public void BlueSlider()
    {
        if (colour[currentColour].b != blueSlider.value)
        {
            colour[currentColour].b = blueSlider.value;
            changeColours.UpdateColour(colour);
        }
    }

    public void DefaultColour()
    {
        for (int i = 0; i < Globals.COLOURS_PER_SHADER; ++i)
        {
            colour[i] = FindObjectOfType<ChangeColours>().defaultColours[i];
        }
        redSlider.value = colour[currentColour].r;
        greenSlider.value = colour[currentColour].g;
        blueSlider.value = colour[currentColour].b;
    }

    public void SelectSlot(int index)
    {
        currentColour = index;
    }

    public void Save()
    {
        PlayerColourSave c = new PlayerColourSave();
        c.Colours = colour;

        // Save it
        string json = JsonUtility.ToJson(c);
        PlayerPrefs.SetString(Globals.PLAYER_COLOUR_SAVE, json);
    }
}
