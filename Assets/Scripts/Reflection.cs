using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    SpriteRenderer rendererToReflect;
    SpriteRenderer renderer;
    Entity parent;
    Prop prop;

    // Start is called before the first frame update
    void Start()
    {
        rendererToReflect = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        transform.parent.gameObject.TryGetComponent<Entity>(out parent);
        renderer = GetComponent<SpriteRenderer>();
        renderer.material = rendererToReflect.material;
        transform.parent.gameObject.TryGetComponent(out prop);
    }

    // Update is called once per frame
    void Update()
    {
        renderer.flipX = !rendererToReflect.flipX;
        renderer.flipY = rendererToReflect.flipY;
        Sprite sprite = rendererToReflect.sprite;
        renderer.sprite = sprite;


        transform.localPosition = new Vector3(0, -(rendererToReflect.bounds.size.y - UnityEngine.Sprites.DataUtility.GetPadding(sprite).y));


        if (parent && !parent.colourChecked)
        {
            renderer.SetPropertyBlock(parent.material);
            parent.colourChecked = true;
        }
    }
}
