using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_ChargingBar : MonoBehaviour
{
    public Image progBar;
    [SerializeField] float OverMaxSensitivity = 0.01f;

    // Start is called before the first frame update
    void Start()
    {

    }


    //Only needs to update when the value changes. Beyond that, it would be inefficient. 
    //Fill is a value of 0-1 and so it doesn't need any calculations to be done within this script/
    public void UpdateProgBar(float percentage)
    {
        progBar.fillAmount = percentage;
    }
}
