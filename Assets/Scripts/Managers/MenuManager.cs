using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject editCharacterMenu;
    [SerializeField] GameObject optionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        MainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        gameManager.EnterScene(Globals.Scenes.HubWorld);
    }

    public void Settings()
    {
        mainMenu.SetActive(false);
        editCharacterMenu.SetActive(false);
        optionsMenu.SetActive(true);
        gameManager.UpdateSettingsSliders();
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
}
