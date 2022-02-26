using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SwapSprite : MonoBehaviour
{
    [SerializeField] private Sprite target;

    private SpriteRenderer sp;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    
    public void Swap()
    {
        if (sp) sp.sprite = target;
    }
}
