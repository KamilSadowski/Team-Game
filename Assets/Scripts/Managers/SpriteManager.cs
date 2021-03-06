using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{

    [SerializeField] GameObject emptySprite;
    List<GameObject> spriteObjectList = new List<GameObject>();
    List<SpriteRenderer> spriteList = new List<SpriteRenderer>();
    int currentIndex = 0;

    #region Singleton_SpriteManager
    public static SpriteManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Globals.MAX_SPRITES; ++i)
        {
            spriteObjectList.Add(Instantiate(emptySprite));
            spriteList.Add(spriteObjectList[i].GetComponent<SpriteRenderer>());
        }
    }

    public void AddSprite(Vector2 position, Sprite image, Color color, float scaleX = 1.0f, float scaleY = 1.0f)
    {
        if (currentIndex == Globals.MAX_SPRITES)
        {
            currentIndex = 0;
        }
        spriteObjectList[currentIndex].transform.position = position;
        spriteObjectList[currentIndex].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
        spriteList[currentIndex].sprite = image;
        spriteList[currentIndex].color = color;
        spriteList[currentIndex].transform.localScale = 
            new Vector3(scaleX, scaleY,1.0f);

        currentIndex++;
    }


}
