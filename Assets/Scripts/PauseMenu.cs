
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : UI
{
    [SerializeField] CanvasGroup main;
    [SerializeField] CanvasGroup controls;
    [SerializeField] private GameObject exitButton;

    public bool isVisible { get; private set; }
    public int sceneIndex { get; private set; }

    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (main != null)
        {
            main.alpha = 0.0f;
            main.interactable = false;
            main.blocksRaycasts = false;
        }


        if (controls != null)
        {
            controls.alpha = 0.0f;
            controls.interactable = false;
            controls.blocksRaycasts = false;
        }

        isVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Avoid to interact when in main menu
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        
        if (t >= 0)
            t -= Time.unscaledDeltaTime;
        else
        {
            t = .5f;

        }

        // Update the exit button text and behaviour
        UpdateExitButton();
    }

    public void Toggle()
    {
        TogglePause();
        ToggleMenu(main);
        if (controls && controls.alpha >= 1f) ToggleMenu(controls);
    }


    public void TogglePause()
    {
        if (Time.timeScale < .5f) Resume();
        else Pause();
    }

    public void Pause()
    {
        Cursor.visible = isVisible = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Cursor.visible = isVisible = false;
        Time.timeScale = 1f;
    }
    
    void UpdateExitButton()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        var text = exitButton.GetComponentInChildren<TextMeshProUGUI>();

        if (sceneIndex > 1)
        {
            text.SetText("Back to hub");
        }
        else
        {
            text.SetText("Exit");
        }
    }

    public void Kill()
    {
        if (sceneIndex == 1) return;

        Start();
        Resume();
        FindObjectOfType<Player>().DestroyEntity();
    }

    public void Exit()
    {
        if (sceneIndex == 1)
            Application.Quit();
    }

}
