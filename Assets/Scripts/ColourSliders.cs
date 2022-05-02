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
        for (int i = 0; i < 3; ++i)
        {
            colour[0].r = 1.0f;
            colour[0].g = 1.0f;
            colour[0].b = 1.0f;
            colour[0].a = 1.0f;
        }
    }

    public void RedSlider()
    {
        if (colour[currentColour].r != redSlider.value)
        {
            colour[currentColour].r = redSlider.value;
            changeColours.ChangeColour(colour);
        }

    }

    public void GreenSlider()
    {
        if (colour[currentColour].g != greenSlider.value)
        {
            colour[currentColour].g = greenSlider.value;
            changeColours.ChangeColour(colour);
        }
    }

    public void BlueSlider()
    {
        if (colour[currentColour].b != blueSlider.value)
        {
            colour[currentColour].b = blueSlider.value;
            changeColours.ChangeColour(colour);
        }
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
        PlayerPrefs.SetString("PlayerColours",json);
    }
}
