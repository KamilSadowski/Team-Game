using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTrailComponent : MonoBehaviour
{

    SpriteManager spriteManager;
    [SerializeField] float minTrailSpawnTimer = .5f;
    [SerializeField] float maxTrailSpawnTimer = 1.0f;

    [SerializeField] Sprite[] Trail_Sprites;
    [SerializeField] Color[] Trail_Colours;

    [SerializeField] float Min_Alpha = 150.0f;
    [SerializeField] float Max_Alpha = 255.0f;

    [SerializeField] float Min_Trail_Scale = 1.0f ;
    [SerializeField] float Max_Trail_Scale = 1.0f;

    Color OutputColour;
    float OutputScale;
    // Start is called before the first frame update
    void Start()
    {
        spriteManager = SpriteManager.instance;
        if(Trail_Sprites.Length > 0 && Trail_Colours.Length > 0)
        StartCoroutine(BleedThread());
    }

    IEnumerator BleedThread()
    {
        while (true)
        {
            if (!spriteManager)
            {
                spriteManager = SpriteManager.instance;
            }
            else 
            {
                OutputColour = Trail_Colours[Random.Range(0, Trail_Colours.Length)];
                OutputColour.a = Random.Range(Min_Alpha, Max_Alpha);
                OutputScale = Random.Range(Min_Trail_Scale, Max_Trail_Scale);

                spriteManager.AddSprite
                    (
                    gameObject.transform.position, 
                    Trail_Sprites[Random.Range(0, Trail_Sprites.Length)], 
                    OutputColour,
                    OutputScale, 
                    OutputScale

                    );
            }

            yield return new WaitForSecondsRealtime(Random.Range(minTrailSpawnTimer, maxTrailSpawnTimer));
        }
    }
}
