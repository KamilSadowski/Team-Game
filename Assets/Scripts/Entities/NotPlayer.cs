using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotPlayer : Character
{
    // Store the room the AI was created in, on death, the room will be notified that one of the entities has been defeated
    Room room;
    [SerializeField] int HealthDropChance = 10;

    static bool isDifficultyMod = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!isDifficultyMod && healthComponent)
        {
            isDifficultyMod = true;
            healthComponent.applyDifficultyModifier();

        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateEntity();
    }
    public override bool DestroyEntity()
    {
        if (entityManager)
            if(Random.Range(0,100) < HealthDropChance)
            entityManager.TryCreateRandomListedPickup(transform.position);

        return base.DestroyEntity();
    }

    public override void OnRemove()
    {
        if (room != null) room.EntityRemoved();
    }

    // Adds the room that owns this entity
    public void AddRoom(Room roomToAdd)
    {
        room = roomToAdd;
    }

}
