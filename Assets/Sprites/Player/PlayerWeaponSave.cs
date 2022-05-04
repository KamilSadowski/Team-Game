using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlayerWeaponSaves
{
    //Transform 
    public Vector3 Scale;

    //These are private variables in classes.
    //EntityManager
    public int WeaponTemplateID;

    //Weapon Cont
    public float WeaponCD;
    public float WeaponCost;
    public float WeaponPickupReward;

    //Weapon
    public float sharpness;
    public float damage;
    public float bounceStr;

    //Mobile Component
    public float MovementSpeed;
    public float Drag;
    public bool isFacingRight;

    //Colours
    public Color PrimaryColoursRGB;
    public Color SecondaryColoursRGB;
    public Color TetiaryColoursRGB;
}
