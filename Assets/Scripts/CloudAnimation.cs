using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudAnimation : MonoBehaviour
{
    Image image;
    [SerializeField] Sprite[] cloudSprites;
    float speed = 0.4f;
    float currentSprite = 0;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        currentSprite = Random.RandomRange(0.0f, cloudSprites.Length);
    }

    // Update is called once per frame
    void Update()
    {
        int spriteIndex = (int)currentSprite;
        if (spriteIndex < cloudSprites.Length)
        {
            image.sprite = cloudSprites[spriteIndex];
            currentSprite += Time.unscaledDeltaTime * speed;
        }
        else
        {
            currentSprite = 0.0f;
        }
    }
}
