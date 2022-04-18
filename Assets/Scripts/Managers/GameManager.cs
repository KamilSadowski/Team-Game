using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Player player;

    EntityManager entityManager;

    // Volume variables
    Slider effectsSlider;
    Slider musicSlider;
    [SerializeField] AudioMixer effectsMixer;
    [SerializeField] AudioMixer musicMixer;

    // Weapon data
    Globals.WeaponData weaponsToGive;
    WeaponController weaponControllers;
    int weaponsToGiveIDs;
    bool weaponsGiven = false;

    // Start is called before the first frame update
    void Start()
    {
        weaponsToGive = new Globals.WeaponData();
        weaponsToGive.prefabID = 1;


        // Game manager cannot be destroyed
        DontDestroyOnLoad(gameObject);

        // Check if player prefs were set, if not, give default values
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.5f);
        }
        if (!PlayerPrefs.HasKey("FX"))
        {
            PlayerPrefs.SetFloat("FX", 0.5f);
        }

        // Update variables based on player prefs data
        musicMixer.SetFloat("masterVolume", Mathf.Log10(PlayerPrefs.GetFloat("Music") * 20));
        effectsMixer.SetFloat("masterVolume", Mathf.Log10(PlayerPrefs.GetFloat("FX") * 20));
    }

    // Update is called once per frame
    void LateUpdate()
    {
            if (FollowingCamera.instance && FollowingCamera.instance.targetToFollow == null)
            {
                if (player != null)
                {
                    FollowingCamera.instance.SetTarget(player);
                }
                else
                {
                    player = FindObjectOfType<Player>();
                }
            }
    }

    public void GivePlayerEquipment()
    {
        // Give the player weapons 
        if (!weaponsGiven)
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();
            }

            if (entityManager == null)
            {
                entityManager = FindObjectOfType<EntityManager>();
            }


            if (player && entityManager)
            {

                    if(weaponsToGive.prefabID > -1)
                    {
                        if (entityManager) weaponsToGiveIDs = entityManager.TryCreateListedWeapon(weaponsToGive.prefabID, Vector3.zero);
                    }

               
                
                    if (weaponsToGiveIDs != -1)
                    {
                        weaponControllers = entityManager.GetEntity(weaponsToGiveIDs).GetComponent<WeaponController>();
                        weaponControllers.PlayerPickup();
                        player.PickupNewWeapon(weaponControllers);
                    }
                

                    weaponsGiven = true;
            }
        }
    }

    // Returns false if no player
    public bool TeleportPlayer(Vector3 teleportTo)
    {
        if (player != null)
        {
            player.Teleport(teleportTo);
            return true;
        }
        else
        {
            player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.Teleport(teleportTo);
                return true;
            }
        }
        return false;
    }

    public void EnterScene(Globals.Scenes scene)
    {
        // Update weapons to give
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (player)
        {
            weaponsToGive.prefabID = 1;
            weaponsGiven = false;
            player = null;
        }
        
        // Load the new scene
        SceneManager.LoadScene(Globals.SceneNames[(int)scene]);
    }

    public void UpdateMusicSlider()
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        musicMixer.SetFloat("masterVolume", Mathf.Log10(PlayerPrefs.GetFloat("Music") * 20));
    }

    public void UpdateFXSlider()
    {
        PlayerPrefs.SetFloat("FX", effectsSlider.value);
        effectsMixer.SetFloat("masterVolume", Mathf.Log10(PlayerPrefs.GetFloat("FX") * 20));
    }

    public void UpdateSettingsSliders()
    {
        if (!effectsSlider)
        {
            effectsSlider = GameObject.FindGameObjectWithTag("FXSlider").GetComponent<Slider>();
        }
        if (!musicSlider)
        {
            musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
        }
        effectsSlider.value = PlayerPrefs.GetFloat("FX");
        musicSlider.value = PlayerPrefs.GetFloat("Music");
    }
}
