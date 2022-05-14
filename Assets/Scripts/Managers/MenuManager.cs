using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameManager gameManager;
    AudioSettings audioSettings;
    Hud hud;
    [SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup editCharacterMenu;
    [SerializeField] CanvasGroup optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        audioSettings = FindObjectOfType<AudioSettings>();
        gameManager = FindObjectOfType<GameManager>();
        hud = FindObjectOfType<Hud>();
        hud.HideHud();

        var c = PlayerPrefs.GetString(Globals.PLAYER_COLOUR_SAVE);
        if (c is { Length: > 0 })
        {
            MainMenu();
        }
        else
        {
            EditCharacter();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        hud.ShowHud();
        gameManager.EnterScene(Globals.Scenes.HubWorld);
    }

    public void Settings()
    {
        mainMenu.alpha = 0.0f;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;

        editCharacterMenu.alpha = 0.0f;
        editCharacterMenu.interactable = false;
        editCharacterMenu.blocksRaycasts = false;

        optionsMenu.alpha = 1.0f;
        optionsMenu.interactable = true;
        optionsMenu.blocksRaycasts = true;

        audioSettings.UpdateSettingsSliders();
    }

    public void MainMenu()
    {
        mainMenu.alpha = 1.0f;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;

        editCharacterMenu.alpha = 0.0f;
        editCharacterMenu.interactable = false;
        editCharacterMenu.blocksRaycasts = false;

        optionsMenu.alpha = 0.0f;
        optionsMenu.interactable = false;
        optionsMenu.blocksRaycasts = false;
    }

    public void EditCharacter()
    {
        mainMenu.alpha = 0.0f;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;

        editCharacterMenu.alpha = 1.0f;
        editCharacterMenu.interactable = true;
        editCharacterMenu.blocksRaycasts = true;

        optionsMenu.alpha = 0.0f;
        optionsMenu.interactable = false;
        optionsMenu.blocksRaycasts = false;
    }

    public void ResetProgress()
    {
        //PlayerPrefs.DeleteKey(Globals.PLAYER_COLOUR_SAVE);
        //PlayerPrefs.DeleteKey(Globals.PLAYER_WEAPON_SAVE);
        //PlayerPrefs.DeleteKey(Globals.PLAYER_CURRENCY_SAVE);
        //PlayerPrefs.DeleteKey(Globals.PLAYER_DIFFICULTY_SAVE);
        PlayerPrefs.DeleteAll();
        ExitGame();
    }

    public void ExitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}
