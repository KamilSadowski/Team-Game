using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : UI
{
    private CanvasGroup canvas;
    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        menu = canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && t < 0)
        {
            t = .5f;
            
            TogglePause();
        }

        if(t >= 0)
            t -= Time.unscaledDeltaTime;
    }

    public void TogglePause()
    {
        if (Time.timeScale < .5f) Resume();
        else Pause();
            ToggleMenu();
    }

    public void Pause()
    {
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
