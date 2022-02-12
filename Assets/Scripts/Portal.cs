using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Globals.Scenes destination;
    SpriteRenderer renderer;
    Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enable(bool show)
    {
        if (!renderer)
        {
            renderer = GetComponent<SpriteRenderer>();           
        }

        if (!collider)
        {
            collider = GetComponent<Collider2D>();

        }

        renderer.enabled = show;
        collider.enabled = show;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.EnterScene(destination);
        }
    }

}
