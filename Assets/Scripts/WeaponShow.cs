using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShow : MonoBehaviour
{

    Player Player;
    private Sprite WeaponSprite;
    private Image Image;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<Player>();
        Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        if (time < 0f)
        {
            if (Player)
            {
                // Fetch the player's weapon sprite
                var weapon = Player.getEquippedWeapon();
                if (weapon)
                {
                    if (weapon.renderer)
                    {
                        WeaponSprite = weapon.renderer.sprite;

                        if (WeaponSprite)
                        {
                            // Change the weapon show sprite to match the player's current weapon
                            Image.sprite = WeaponSprite;
                        }
                    }
                }

            }
            else
            {
                Player = FindObjectOfType<Player>();
            }

            time = 2;
        }
    }
}
