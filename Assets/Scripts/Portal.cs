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
        animation = FindObjectOfType<FollowingCamera>().fadeAnimation;
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
            _spriteRenderer.color = Color.clear;
            _player.gameObject.SetActive(false);
            Instantiate(animation);

            if (_swapSprite) _swapSprite.Swap();
            Invoke(nameof(Teleport), animDuration);
            

        }
    }

    private void Teleport()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EnterScene(destination);
    }

    public static void TeleportTo(Globals.Scenes dest)
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.EnterScene(dest);
    }

}
