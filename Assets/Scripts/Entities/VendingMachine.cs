using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class VendingMachine : Prop
{
    private enum EType { PickUp, Weapon };

    [SerializeField] private GameObject[] DroppableItems;
    [SerializeField] int cost = 1;
    [SerializeField] GameObject vendingMachineHole;
    [SerializeField] AudioClip sound;
    AudioSource audioSource;
    static GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Use()
    {
        if (DroppableItems.Length > 0)
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            if (gameManager.TrySpendCoins(cost))
            {
                audioSource.PlayOneShot(sound);
                StartCoroutine(SpawnItem());
            }
        }
    }

    IEnumerator SpawnItem()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        EntityManager.instance.TryCreateEntity(DroppableItems[Random.Range(0, DroppableItems.Length)], vendingMachineHole.transform.position);
    }

}

