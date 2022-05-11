using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private MapMenu mapMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mapMenu.isVisible)
            {
                mapMenu.Toggle();
            }
            else
            {
                pauseMenu.Toggle();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (pauseMenu.isVisible) return;

            mapMenu.Toggle();
        }
    }
}
