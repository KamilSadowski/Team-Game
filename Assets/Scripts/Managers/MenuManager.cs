using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameManager gameManager;
    AudioSettings audioSettings;
    Hud hud;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject editCharacterMenu;
    [SerializeField] GameObject optionsMenu;

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
        mainMenu.SetActive(false);
        editCharacterMenu.SetActive(false);
        optionsMenu.SetActive(true);
        audioSettings.UpdateSettingsSliders();
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        editCharacterMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void EditCharacter()
    {
        mainMenu.SetActive(false);
        editCharacterMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Globals.PLAYER_COLOUR_SAVE);
        PlayerPrefs.DeleteKey(Globals.PLAYER_WEAPON_SAVE);
        PlayerPrefs.DeleteKey(Globals.PLAYER_CURRENCY_SAVE);
        PlayerPrefs.DeleteKey(Globals.PLAYER_DIFFICULTY_SAVE);
        ExitGame();
    }

    public void ExitGame()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }
}
