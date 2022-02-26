using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Globals.Scenes destination;
    [SerializeField] private FadeAnimation animation;
    [SerializeField] private float animDuration;
    SpriteRenderer renderer;
    Collider2D collider;
    private SwapSprite _swapSprite;
    private Player _player;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _spriteRenderer = _player.GetComponent<SpriteRenderer>();
        _swapSprite = GetComponent<SwapSprite>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _spriteRenderer.color = Color.clear;
            var anim = Instantiate(animation);
            if (_swapSprite) _swapSprite.Swap();
            Invoke(nameof(Teleport), animDuration);
        }
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
            Invoke(nameof(Teleport), animDuration);
            var anim = Instantiate(animation);
        }
    }

    private void Teleport()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EnterScene(destination);
    }

}
