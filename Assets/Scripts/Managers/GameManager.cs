using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Player player;

    EntityManager entityManager;

    // Player data
    int coins = 20;

    // Weapon data
    Globals.WeaponData weaponsToGive;
    WeaponController weaponControllers;
    int weaponsToGiveIDs;
    bool weaponsGiven = false;
    FollowingCamera camera;

    // Start is called before the first frame update
    void Start()
    {
        weaponsToGive = new Globals.WeaponData();
        weaponsToGive.prefabID = 0;


        // Game manager cannot be destroyed
        DontDestroyOnLoad(gameObject);

        UpdateCurrency(coins);
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
                if (weaponsToGive.prefabID > -1)
                {
                    string weaponClass = PlayerPrefs.GetString(Globals.PLAYER_WEAPON_SAVE);
                    
                    
                    if (weaponClass is { Length: > 0 }) {
                        //PlayerWeaponSaves s = JsonUtility.FromJson<PlayerWeaponSaves>(weaponClass);
                        //if (s != null)
                        //    weaponsToGiveIDs = entityManager.TryCreateEntity(new GameObject, Vector3.forward * 5.0f);
                        //else
                        weaponsToGiveIDs = entityManager.TryCreateListedWeapon(player.GetSavedWeaponTemplateID(), Vector3.up * 50.0f);
                        player.WeaponLoad(entityManager.GetEntity(weaponsToGiveIDs).gameObject);
                    }
                    else
                        weaponsToGiveIDs = entityManager.TryCreateListedWeapon(weaponsToGive.prefabID, Vector3.up * 50.0f);
                }

                if (weaponsToGiveIDs != -1)
                {
                    Entity entityRef = entityManager.GetEntity(weaponsToGiveIDs);
                    weaponControllers = entityRef.GetComponent<WeaponController>();
                    weaponControllers.PlayerPickup(entityRef.entityID);
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
            weaponsToGive.prefabID = 0;
            weaponsGiven = false;
            player = null;
        }

        int sceneIndex = (int)scene;

        // If the scene is a dungeon, the map will be randomised
        if (scene == Globals.Scenes.Dungeon)
        {
            sceneIndex += Random.Range(0, Globals.DUNGEON_NUMBER);
        }

        // Load the new scene
        SceneManager.LoadScene(Globals.SceneNames[sceneIndex]);
    }


    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCurrency(coins);
    }

    // Returns false if cannot afford
    public bool TrySpendCoins(int amount)
    {
        if (amount <= coins)
        {
            coins -= amount;
            UpdateCurrency(coins);
            return true;
        }
        return false;
    }

    public void UpdateCurrency(int newAmount)
    {
        if (!camera) camera = FindObjectOfType<FollowingCamera>();
        camera.UpdateCurrency(newAmount.ToString());
    }
}
