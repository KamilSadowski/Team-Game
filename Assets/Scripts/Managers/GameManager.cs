using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Player player;
    FollowingCamera camera;

    // Volume variables
    Slider effectsSlider;
    Slider musicSlider;
    [SerializeField] AudioMixer effectsMixer;
    [SerializeField] AudioMixer musicMixer;

    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {
        if (camera != null)
        {
            if (camera.targetToFollow == null)
            {
                if (player != null)
                {
                    camera.SetTarget(player);
                }
                else
                {
                    player = FindObjectOfType<Player>();
                }
            }
        }
        else
        {
            camera = FindObjectOfType<FollowingCamera>();
        }

    }

    public void TeleportPlayer(Vector3 teleportTo)
    {
        if (player != null)
        {
            player.transform.position = teleportTo;
        }
        else
        {
            player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.transform.position = teleportTo;
            }
        }
    }

    public void EnterScene(Globals.Scenes scene)
    {
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
