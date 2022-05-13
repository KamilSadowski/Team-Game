using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The reflection looks for a sprite renderer in its parent and then reflects its current sprite every frame
public class Reflection : MonoBehaviour
{
    SpriteRenderer rendererToReflect;
    SpriteRenderer renderer;
    Entity parent;
    [SerializeField] float reflectionRotation = 180.0f;

    // Start is called before the first frame update
    void Start()
    {
        rendererToReflect = transform.parent.gameObject.GetComponent<SpriteRenderer>();
  
        // Things like weapons might have a sprite that is a child and not a part of the base object
        if (!rendererToReflect)
        {
            rendererToReflect = transform.parent.gameObject.GetComponentInChildren<SpriteRenderer>();
        }
        transform.parent.gameObject.TryGetComponent<Entity>(out parent);
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = rendererToReflect.color;
        renderer.material = rendererToReflect.material;

    }

    // Update is called once per frame
    void Update()
    {
        renderer.flipX = !rendererToReflect.flipX;
        renderer.flipY = rendererToReflect.flipY;
        Sprite sprite = rendererToReflect.sprite;
        renderer.sprite = sprite;
        renderer.color = rendererToReflect.color;

        transform.localPosition = new Vector3(0, -(rendererToReflect.bounds.size.y / rendererToReflect.gameObject.transform.localScale.y), 0);
        Vector3 rotation = rendererToReflect.transform.rotation.eulerAngles;
        transform.rotation.SetEulerRotation(rotation.x, rotation.y, rotation.z + reflectionRotation);

        if (parent && !parent.colourChecked)
        {
            renderer.SetPropertyBlock(parent.material);
            parent.colourChecked = true;
        }
    }
}
